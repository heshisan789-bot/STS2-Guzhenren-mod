using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;

namespace Guzhenren.Scripts;

public sealed class TunHuoPower : AbstractGuZhenRenPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override bool AllowNegative => false;

    public override async Task AfterCardChangedPiles(CardModel card, PileType oldPileType, AbstractModel? source)
    {
        if (Amount <= 0 || CombatState == null || Owner.Player == null)
        {
            return;
        }

        if (card is not Burn || card.Owner?.Creature != Owner)
        {
            return;
        }

        if (card.Pile?.Type != PileType.Exhaust || oldPileType == PileType.Exhaust)
        {
            return;
        }

        Flash();
        for (var i = 0; i < Amount; i++)
        {
            await CardPileCmd.AddGeneratedCardToCombat(CombatState.CreateCard<HuoShi>(Owner.Player), PileType.Hand, addedByPlayer: true);
        }
    }
}

