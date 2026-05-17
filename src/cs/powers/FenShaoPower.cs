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
    private sealed class Data
    {
        public bool IsSpreading;
    }

    public override PowerType Type => PowerType.Debuff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override bool AllowNegative => false;

    protected override object InitInternalData()
    {
        return new Data();
    }

    public override async Task AfterPowerAmountChanged(PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
    {
        if (power != this || Amount <= 0 || Owner.IsDead || Owner.CombatState == null || !LocalContext.NetId.HasValue)
        {
            return;
        }

        var data = GetInternalData<Data>();
        if (!data.IsSpreading && amount > 0 && Owner.GetPower<XingHuoLiaoYuanPower>() != null)
        {
            data.IsSpreading = true;
            try
            {
                foreach (var enemy in Owner.CombatState.HittableEnemies.Where(enemy => enemy != Owner))
                {
                    await PowerCmd.Apply<FenShaoPower>(enemy, amount, applier ?? Owner, cardSource);
                }
            }
            finally
            {
                data.IsSpreading = false;
            }
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
