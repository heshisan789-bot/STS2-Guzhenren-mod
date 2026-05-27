using System.Reflection;
using HarmonyLib;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Ancients;
using MegaCrit.Sts2.Core.Models.Events;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Runs;

namespace Guzhenren.Scripts;

[HarmonyPatch]
internal static class TheArchitectWinRunNullDialoguePatch
{
    private static readonly FieldInfo DialogueField = AccessTools.Field(typeof(TheArchitect), "_dialogue");

    [HarmonyPatch(typeof(TheArchitect), "WinRun")]
    [HarmonyPrefix]
    private static bool WinRunPrefix(TheArchitect __instance, ref Task __result)
    {
        var dialogue = DialogueField?.GetValue(__instance) as AncientDialogue;
        if (dialogue != null)
        {
            return true;
        }

        __result = WinRunSafe(__instance);
        return false;
    }

    private static Task WinRunSafe(TheArchitect architect)
    {
        var owner = architect.Owner;
        if (owner == null || !LocalContext.IsMe(owner))
        {
            return Task.CompletedTask;
        }

        if (owner.RunState?.Players.Count > 1)
        {
            NCombatRoom.Instance?.SetWaitingForOtherPlayersOverlayVisible(visible: true);
        }

        RunManager.Instance.ActChangeSynchronizer.SetLocalPlayerReady();
        return Task.CompletedTask;
    }
}
