using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Models;

namespace Guzhenren.Scripts;

[HarmonyPatch(typeof(CardModel), "get_CombatState")]
internal static class GuZhenRenCardCombatStatePatch
{
    [HarmonyPostfix]
    private static void Postfix(CardModel __instance, ref CombatState? __result)
    {
        if (__result != null)
        {
            return;
        }

        if (!CombatManager.Instance.IsInProgress)
        {
            return;
        }

        if (__instance is not AbstractGuZhenRenCard)
        {
            return;
        }

        var combatState = __instance.Owner?.Creature?.CombatState;
        if (combatState != null)
        {
            __result = combatState;
        }
    }
}
