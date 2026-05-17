using HarmonyLib;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Runs;

namespace Guzhenren.Scripts;

[HarmonyPatch]
internal static class RunEndDebugPatch
{
    [HarmonyPatch(typeof(RunManager), nameof(RunManager.EnterNextAct))]
    [HarmonyPrefix]
    private static void EnterNextActPrefix()
    {
        var state = RunManager.Instance.GetStateForDebug();
        if (state == null)
        {
            Log.Info("[GuZhenRen][RunEndDebug] EnterNextAct: state=null");
            return;
        }

        Log.Info($"[GuZhenRen][RunEndDebug] EnterNextAct: actIdx={state.CurrentActIndex} room={state.CurrentRoom?.RoomType} isVictory={state.CurrentRoom?.IsVictoryRoom} roomCount={state.CurrentRoomCount}");
    }

    [HarmonyPatch(typeof(RunManager), nameof(RunManager.ProceedFromTerminalRewardsScreen))]
    [HarmonyPrefix]
    private static void ProceedFromTerminalRewardsScreenPrefix()
    {
        var state = RunManager.Instance.GetStateForDebug();
        if (state == null)
        {
            Log.Info("[GuZhenRen][RunEndDebug] ProceedFromTerminalRewardsScreen: state=null");
            return;
        }

        Log.Info($"[GuZhenRen][RunEndDebug] ProceedFromTerminalRewardsScreen: room={state.CurrentRoom?.RoomType} roomCount={state.CurrentRoomCount} resumeParentEvent={(state.CurrentRoom is CombatRoom cr ? cr.ShouldResumeParentEventAfterCombat : null)}");
    }

    private static RunState? GetStateForDebug(this RunManager rm)
    {
        var t = typeof(RunManager);
        var f = AccessTools.Field(t, "State");
        return f?.GetValue(rm) as RunState;
    }
}
