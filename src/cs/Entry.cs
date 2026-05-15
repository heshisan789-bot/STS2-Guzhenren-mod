using BaseLib.Abstracts;
using Godot.Bridge;
using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Modding;
using MegaCrit.Sts2.Core.Saves.Managers;

namespace Guzhenren.Scripts;

/// <summary>
/// 塔二版蛊真人模组入口。
/// 当前阶段仅负责注册脚本与基础 Harmony 补丁，后续再逐步接入复杂系统。
/// </summary>
[ModInitializer("Init")]
public static class Entry
{
    private const string BuildStamp = "2026-05-15.2";

    public static void Init()
    {
        var harmony = new Harmony("sts2.guzhenren");
        harmony.PatchAll();

        ScriptManagerBridge.LookupScriptsInAssembly(typeof(Entry).Assembly);
        CombatManager.Instance.CombatSetUp += _ => BattleStateManager.PublishBattleStart();
        CombatManager.Instance.CombatEnded += _ => BattleStateManager.PublishPostBattle();
        Log.Info($"GuZhenRen initialized (build={BuildStamp}).");
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
