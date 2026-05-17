using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace Guzhenren.Scripts;

public sealed class FenShenPower : AbstractGuZhenRenPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override bool AllowNegative => false;

    public override async Task AfterSideTurnStart(CombatSide side, CombatState combatState)
    {
        if (side != Owner.Side || Amount <= 0 || Owner.Player == null)
        {
            return;
        }

        Flash();
        await PlayerCmd.GainEnergy(Amount, Owner.Player);
        for (var i = 0; i < Amount; i++)
        {
            await CardPileCmd.AddGeneratedCardToCombat(combatState.CreateCard<Burn>(Owner.Player), PileType.Hand, addedByPlayer: true);
        }
    }
}
