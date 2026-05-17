using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Runs;

namespace Guzhenren.Scripts;

[HarmonyPatch]
internal static class GuZhenRenTokenCardExclusionPatch
{
    [HarmonyPatch(typeof(Hook), "ModifyCardRewardCreationOptions")]
    [HarmonyPostfix]
    private static void ModifyCardRewardCreationOptionsPostfix(IRunState runState, Player player, ref CardCreationOptions __result)
    {
        if (player.Character is not FangYuanCharacter)
        {
            return;
        }

        var existing = __result.CardPoolFilter;
        bool Filter(CardModel card)
        {
            if (existing != null && !existing(card))
            {
                return false;
            }

            return card.Rarity != CardRarity.Token;
        }

        if (__result.CardPools.Count > 0)
        {
            var poolsSnapshot = __result.CardPools.ToArray();
            __result = __result.WithCardPools(poolsSnapshot, Filter);
            return;
        }

        if (__result.CustomCardPool != null)
        {
            var filtered = __result.CustomCardPool.Where(Filter).ToArray();
            if (filtered.Length == 0)
            {
                return;
            }

            var distinctRarities = filtered.Select(static c => c.Rarity).Distinct().Take(2).Count();
            var rarityOdds = distinctRarities <= 1 ? CardRarityOddsType.Uniform : __result.RarityOdds;
            __result = __result.WithCustomPool(filtered, rarityOdds);
        }
    }

    [HarmonyPatch(typeof(Hook), "ModifyMerchantCardPool")]
    [HarmonyPostfix]
    private static void ModifyMerchantCardPoolPostfix(IRunState runState, Player player, ref IEnumerable<CardModel> __result)
    {
        if (player.Character is not FangYuanCharacter)
        {
            return;
        }

        __result = __result.Where(static card => card.Rarity != CardRarity.Token);
    }
}
