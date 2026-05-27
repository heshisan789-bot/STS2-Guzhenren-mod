using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Guzhenren.Scripts;

public sealed class YiXinErYongPower : AbstractGuZhenRenPower
{
    private sealed class Data
    {
        public bool IgnoreNextTrigger { get; set; }
    }

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override bool AllowNegative => false;

    protected override object InitInternalData()
    {
        return new Data();
    }

    public override Task AfterApplied(Creature? applier, CardModel? cardSource)
    {
        GetInternalData<Data>().IgnoreNextTrigger = true;
        return Task.CompletedTask;
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner.Creature != Owner || cardPlay.IsAutoPlay || Amount <= 0)
        {
            return;
        }

        var data = GetInternalData<Data>();
        if (data.IgnoreNextTrigger)
        {
            data.IgnoreNextTrigger = false;
            return;
        }

        Flash();
        await ConsumeCharge();

        var handPile = PileType.Hand.GetPile(cardPlay.Card.Owner);
        var candidates = handPile.Cards.Where(card =>
            !ReferenceEquals(card, cardPlay.Card)
            && card is not AbstractXuYingCard
            && card is not JianYing
            && !card.Keywords.Contains(CardKeyword.Unplayable)).ToList();

        var randomCard = cardPlay.Card.Owner.RunState.Rng.Shuffle.NextItem(candidates);
        if (randomCard == null)
        {
            return;
        }

        randomCard.SetToFreeThisTurn();
        await CardCmd.AutoPlay(context, randomCard, null);
    }

    public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if (side == Owner.Side)
        {
            await PowerCmd.Remove(this);
        }
    }

    private async Task ConsumeCharge()
    {
        if (Amount <= 1)
        {
            await PowerCmd.Remove(this);
            return;
        }

        await PowerCmd.SetAmount<YiXinErYongPower>(Owner, Amount - 1, Owner, null);
    }
}
