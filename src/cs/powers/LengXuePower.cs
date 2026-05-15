using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;

namespace Guzhenren.Scripts;

public sealed class LengXuePower : AbstractGuZhenRenPower
{
    public override PowerType Type => PowerType.Debuff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override bool AllowNegative => false;

    public override async Task AfterSideTurnStart(CombatSide side, CombatState combatState)
    {
        if (side != Owner.Side || Owner.IsDead || Amount <= 0 || !LocalContext.NetId.HasValue)
        {
            return;
        }

        var hpLoss = Math.Max(1m, Math.Floor(Owner.MaxHp * 0.1m));
        Flash();
        var choiceContext = new HookPlayerChoiceContext(this, LocalContext.NetId.Value, combatState, GameActionType.Combat);
        await choiceContext.AssignTaskAndWaitForPauseOrCompletion(
            CreatureCmd.Damage(choiceContext, Owner, hpLoss, ValueProp.Unblockable | ValueProp.Unpowered, Owner));
        await PowerCmd.TickDownDuration(this);
    }
}
