using BaseLib.Abstracts;
using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Guzhenren.Scripts;

public sealed class JianQiaoPower : AbstractGuZhenRenPower
{
    private sealed class Data
    {
        public readonly HashSet<CardModel> StoredCards = [];
    }

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override bool AllowNegative => false;

    protected override object InitInternalData()
    {
        return new Data();
    }

    public void AddStoredCard(CardModel card)
    {
        AssertMutable();
        GetInternalData<Data>().StoredCards.Add(card);
        SetAmount(GetInternalData<Data>().StoredCards.Count, silent: true);
    }

    public override decimal ModifyDamageMultiplicative(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (dealer != Owner || cardSource == null || !props.IsPoweredAttack_())
        {
            return 1m;
        }

        return GetInternalData<Data>().StoredCards.Contains(cardSource) ? 2m : 1m;
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner != Owner.Player)
        {
            return;
        }

        var data = GetInternalData<Data>();
        if (!data.StoredCards.Remove(cardPlay.Card))
        {
            return;
        }

        SetAmount(data.StoredCards.Count, silent: true);
        if (data.StoredCards.Count == 0)
        {
            await PowerCmd.Remove(this);
        }
    }
}
