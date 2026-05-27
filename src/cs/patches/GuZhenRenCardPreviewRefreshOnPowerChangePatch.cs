using System;
using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Cards.Holders;
using MegaCrit.Sts2.Core.Nodes.Rooms;

namespace Guzhenren.Scripts;

[HarmonyPatch]
internal static class GuZhenRenCardPreviewRefreshOnPowerChangePatch
{
    [HarmonyPatch(typeof(Creature), nameof(Creature.ApplyPowerInternal))]
    [HarmonyPostfix]
    private static void ApplyPowerInternalPostfix(Creature __instance, PowerModel power)
    {
        TryRefreshLocalHand(__instance);
    }

    [HarmonyPatch(typeof(Creature), nameof(Creature.RemovePowerInternal))]
    [HarmonyPostfix]
    private static void RemovePowerInternalPostfix(Creature __instance, PowerModel power)
    {
        TryRefreshLocalHand(__instance);
    }

    [HarmonyPatch(typeof(Creature), nameof(Creature.InvokePowerModified))]
    [HarmonyPostfix]
    private static void InvokePowerModifiedPostfix(Creature __instance, PowerModel power, int change, bool silent)
    {
        if (!silent)
        {
            TryRefreshLocalHand(__instance);
        }
    }

    private static void TryRefreshLocalHand(Creature creature)
    {
        if (!CombatManager.Instance.IsInProgress)
        {
            return;
        }

        var player = creature.Player;
        if (player?.Character == null)
        {
            return;
        }

        if (!player.Character.Id.Entry.StartsWith("GUZHENREN-", StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        var hand = NCombatRoom.Instance?.Ui?.Hand;
        if (hand == null)
        {
            return;
        }

        foreach (var holder in hand.ActiveHolders)
        {
            holder.UpdateCard();
        }
    }
}
