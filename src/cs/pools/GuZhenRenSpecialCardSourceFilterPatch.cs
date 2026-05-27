using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Random;
using MegaCrit.Sts2.Core.Runs;

namespace Guzhenren.Scripts;

[HarmonyPatch]
internal static class GuZhenRenSpecialCardSourceFilterPatch
{
    private static bool ShouldApply(Player player)
    {
        return player.Character is FangYuanCharacter;
    }

    private static bool IsBanned(CardModel card, GuZhenRenBannedCardSources source)
    {
        return card is AbstractGuZhenRenCard guzhenren && (guzhenren.BannedSources & source) != 0;
    }

    private static CardCreationOptions ApplyToCreationOptions(Player player, CardCreationOptions options, GuZhenRenBannedCardSources source)
    {
        var existing = options.CardPoolFilter;
        bool Filter(CardModel card)
        {
            if (existing != null && !existing(card))
            {
                return false;
            }

            return !IsBanned(card, source);
        }

        if (options.CardPools.Count > 0)
        {
            var poolsSnapshot = options.CardPools.ToArray();
            return options.WithCardPools(poolsSnapshot, Filter);
        }

        if (options.CustomCardPool != null)
        {
            var filtered = options.CustomCardPool.Where(Filter).ToArray();
            if (filtered.Length == 0)
            {
                return options;
            }

            var distinctRarities = filtered.Select(static c => c.Rarity).Distinct().Take(2).Count();
            var rarityOdds = distinctRarities <= 1 ? CardRarityOddsType.Uniform : options.RarityOdds;
            return options.WithCustomPool(filtered, rarityOdds);
        }

        return options;
    }

    private static IEnumerable<CardModel> FilterWithFallback(IEnumerable<CardModel> options, GuZhenRenBannedCardSources source)
    {
        var filtered = options.Where(card => !IsBanned(card, source)).ToArray();
        return filtered.Length == 0 ? options : filtered;
    }

    [HarmonyPatch(typeof(Hook), "ModifyCardRewardCreationOptions")]
    [HarmonyPostfix]
    private static void ModifyCardRewardCreationOptionsPostfix(IRunState runState, Player player, ref CardCreationOptions __result)
    {
        if (!ShouldApply(player))
        {
            return;
        }

        var source = __result.Source == CardCreationSource.Encounter
            ? GuZhenRenBannedCardSources.Reward
            : GuZhenRenBannedCardSources.Event;

        __result = ApplyToCreationOptions(player, __result, source);
    }

    [HarmonyPatch(typeof(Hook), "ModifyMerchantCardPool")]
    [HarmonyPostfix]
    private static void ModifyMerchantCardPoolPostfix(IRunState runState, Player player, ref IEnumerable<CardModel> __result)
    {
        if (!ShouldApply(player))
        {
            return;
        }

        __result = FilterWithFallback(__result, GuZhenRenBannedCardSources.Shop);
    }

    [HarmonyPatch(typeof(CardFactory), "GetDistinctForCombat")]
    [HarmonyPrefix]
    private static void GetDistinctForCombatPrefix(Player player, ref IEnumerable<CardModel> cards, int count, Rng rng)
    {
        if (!ShouldApply(player))
        {
            return;
        }

        cards = FilterWithFallback(cards, GuZhenRenBannedCardSources.PotionChoice);
    }

    [HarmonyPatch(typeof(CardFactory), "GetForCombat")]
    [HarmonyPrefix]
    private static void GetForCombatPrefix(Player player, ref IEnumerable<CardModel> cards, int count, Rng rng)
    {
        if (!ShouldApply(player))
        {
            return;
        }

        cards = FilterWithFallback(cards, GuZhenRenBannedCardSources.PotionChoice);
    }
}
