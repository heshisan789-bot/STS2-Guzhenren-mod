using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;

namespace Guzhenren.Scripts;

[HarmonyPatch(typeof(MonsterModel), nameof(MonsterModel.PerformMove))]
internal static class ZhuiMingHuoPatch
{
    [HarmonyPostfix]
    private static async Task Postfix(Task __result, MonsterModel __instance)
    {
        await __result;

        if (!__instance.IntendsToAttack)
        {
            return;
        }

        var combatState = __instance.CombatState;
        if (combatState == null)
        {
            return;
        }

        if (!combatState.HittableEnemies.Any(e => e.HasPower<ZhuiMingHuoPower>()))
        {
            return;
        }

        if (ZhuiMingHuoPower.SpreadThisTurn.Contains(__instance.Creature))
        {
            return;
        }

        ZhuiMingHuoPower.SpreadThisTurn.Add(__instance.Creature);
        await PowerCmd.Apply<ZhuiMingHuoPower>(__instance.Creature, 1m, __instance.Creature, null);
    }
}
