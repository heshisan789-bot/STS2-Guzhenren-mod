using System.Linq;
using System.Threading.Tasks;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;

namespace Guzhenren.Scripts;

[HarmonyPatch(typeof(CardPileCmd), nameof(CardPileCmd.AddGeneratedCardToCombat))]
internal static class XianGuUniquePreventCombatGenerationPatch
{
    [HarmonyPrefix]
    private static bool Prefix(CardModel card, ref Task<CardPileAddResult> __result)
    {
        if (card.Owner == null || card is not AbstractGuZhenRenCard guCard)
        {
            return true;
        }

        if (guCard.IsXuYing)
        {
            return true;
        }

        if (!(guCard.IsXianGu || guCard.Rank >= 6))
        {
            return true;
        }

        if (!card.Owner.Deck.Cards.Any(c => c.Id == card.Id && !ReferenceEquals(c, card)))
        {
            return true;
        }

        card.RemoveFromState();
        __result = Task.FromResult(new CardPileAddResult
        {
            success = false,
            cardAdded = card,
            oldPile = null,
            modifyingModels = null
        });
        return false;
    }
}
