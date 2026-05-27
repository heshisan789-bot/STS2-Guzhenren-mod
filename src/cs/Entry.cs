using BaseLib.Abstracts;
using Godot.Bridge;
using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Modding;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.Saves.Managers;
using System;
using System.Runtime.ExceptionServices;

namespace Guzhenren.Scripts;

/// <summary>
/// 塔二版蛊真人模组入口。
/// 当前阶段仅负责注册脚本与基础 Harmony 补丁，后续再逐步接入复杂系统。
/// </summary>
[ModInitializer("Init")]
public static class Entry
{
    private const string BuildStamp = "2026-05-17.1";
    private static bool _loggedIndexOutOfRange;

    public static void Init()
    {
        var harmony = new Harmony("sts2.guzhenren");
        harmony.PatchAll();

        AppDomain.CurrentDomain.FirstChanceException += OnFirstChanceException;

        ScriptManagerBridge.LookupScriptsInAssembly(typeof(Entry).Assembly);
        ModHelper.SubscribeForRunStateHooks("guzhenren_shazhao_synthesis", _ =>
            new AbstractModel[]
            {
                ModelDb.GetById<GuZhenRenShaZhaoSynthesisSystem>(ModelDb.GetId(typeof(GuZhenRenShaZhaoSynthesisSystem))),
                ModelDb.GetById<GuZhenRenXianGuUniqueSystem>(ModelDb.GetId(typeof(GuZhenRenXianGuUniqueSystem)))
            });
        CombatManager.Instance.CombatSetUp += _ => BattleStateManager.PublishBattleStart();
        CombatManager.Instance.CombatEnded += _ => BattleStateManager.PublishPostBattle();
        RunManager.Instance.RunStarted += state =>
        {
            TaskHelper.RunSafely(BenMingGuEventSelectionCoordinator.TryInjectCurrentRoom());
            TaskHelper.RunSafely(GuFangStartingRecipeHelper.TryGrantStartingRecipe(state));
        };
        RunManager.Instance.RoomEntered += () =>
        {
            TaskHelper.RunSafely(BenMingGuEventSelectionCoordinator.TryInjectCurrentRoom());
        };

        Log.Info($"GuZhenRen initialized (build={BuildStamp}).");
    }

    private static void OnFirstChanceException(object? sender, FirstChanceExceptionEventArgs e)
    {
        if (_loggedIndexOutOfRange || e.Exception is not ArgumentOutOfRangeException)
        {
            return;
        }

        _loggedIndexOutOfRange = true;
        Log.Error($"[GuZhenRen][FirstChance] {e.Exception}");
    }

    /// <summary>
    /// 避免游戏对自定义角色套用原版角色解锁纪元逻辑。
    /// </summary>
    [HarmonyPatch(typeof(ProgressSaveManager), "ObtainCharUnlockEpoch")]
    private static class SkipObtainCharUnlockEpoch
    {
        [HarmonyPrefix]
        private static bool Prefix(Player localPlayer)
        {
            return localPlayer.Character is not ICustomModel;
        }
    }
}
