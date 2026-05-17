using System.Threading.Tasks;
using HarmonyLib;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Cards;

namespace Guzhenren.Scripts;

[HarmonyPatch]
internal static class TunHuoBurnPatch
{
    [HarmonyPatch(typeof(Burn), nameof(Burn.OnTurnEndInHand))]
    [HarmonyPrefix]
    private static bool OnTurnEndInHandPrefix(Burn __instance, PlayerChoiceContext choiceContext, ref Task __result)
    {
        if (__instance.Owner?.Creature.HasPower<TunHuoPower>() == true)
        {
            __result = Task.CompletedTask;
            return false;
        }

        return true;
    }
}

