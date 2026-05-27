using System.Linq;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Models;

namespace Guzhenren.Scripts;

public sealed class GuZhenRenXianGuUniqueSystem : AbstractModel
{
    public override bool ShouldReceiveCombatHooks => false;

    public override bool ShouldAddToDeck(CardModel card)
    {
        if (card.Owner == null || card is not AbstractGuZhenRenCard guCard)
        {
            return true;
        }

        if (!IsXianGu(guCard))
        {
            return true;
        }

        return !card.Owner.Deck.Cards.Any(c => c.Id == card.Id && !ReferenceEquals(c, card));
    }

    public override async Task AfterAddToDeckPrevented(CardModel card)
    {
        if (card.Owner == null)
        {
            return;
        }

        var owner = card.Owner;
        if (owner.RunState.ContainsCard(card))
        {
            card.RemoveFromState();
            owner.RunState.RemoveCard(card);
        }

        if (CombatManager.Instance.IsInProgress)
        {
            return;
        }

        var relic = owner.GetRelic<XianGuCanHai>();
        if (relic == null)
        {
            await RelicCmd.Obtain<XianGuCanHai>(owner);
            return;
        }

        relic.Counter += 1;
        relic.Flash();
    }

    private static bool IsXianGu(AbstractGuZhenRenCard card)
    {
        if (card.IsXuYing)
        {
            return false;
        }

        return card.IsXianGu || card.Rank >= 6;
    }
}
