using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;

namespace Guzhenren.Scripts;

public sealed class YanTongPower : AbstractGuZhenRenPower
{
    private sealed class Data
    {
        public int TriggerCount;
    }

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override bool AllowNegative => false;

    protected override object InitInternalData()
    {
        return new Data();
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        if (Amount <= 0 || cardPlay.Card.Owner?.Creature != Owner || cardPlay.Target == null || cardPlay.Target.Side == Owner.Side)
        {
            return;
        }

        Flash();
        await PowerCmd.Apply<FenShaoPower>(cardPlay.Target, Amount, Owner, cardPlay.Card);

        if (CombatState == null || Owner.Player == null)
        {
            return;
        }

        var data = GetInternalData<Data>();
        data.TriggerCount += 1;
        if (data.TriggerCount > 4)
        {
            await CardPileCmd.AddGeneratedCardToCombat(CombatState.CreateCard<Burn>(Owner.Player), PileType.Hand, addedByPlayer: true);
        }
    }

    public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if (side == Owner.Side)
        {
            await PowerCmd.Remove(this);
        }
    }
}

