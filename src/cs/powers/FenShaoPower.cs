using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Guzhenren.Scripts;

public sealed class FenShaoPower : AbstractGuZhenRenPower
{
    public override PowerType Type => PowerType.Debuff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override bool AllowNegative => false;

    public override async Task AfterPowerAmountChanged(PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
    {
        if (power != this || Amount <= 0 || Owner.IsDead || Owner.CombatState == null || !LocalContext.NetId.HasValue)
        {
            return;
        }

        Flash();
        var combatState = Owner.CombatState;
        var dealer = Owner.IsPlayer ? Owner : combatState.Players.FirstOrDefault()?.Creature ?? Owner;
        var choiceContext = new HookPlayerChoiceContext(this, LocalContext.NetId.Value, combatState, GameActionType.Combat);
        await choiceContext.AssignTaskAndWaitForPauseOrCompletion(
            CreatureCmd.Damage(choiceContext, Owner, Amount, ValueProp.Unblockable | ValueProp.Unpowered, dealer));
    }

    public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if (side != Owner.Side || Amount <= 0)
        {
            return;
        }

        var nextAmount = Amount / 2;
        if (nextAmount <= 0)
        {
            await PowerCmd.Remove(this);
            return;
        }

        await PowerCmd.SetAmount<FenShaoPower>(Owner, nextAmount, Owner, null);
    }
}
