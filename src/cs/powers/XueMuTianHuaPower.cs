using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace Guzhenren.Scripts;

public sealed class XueMuTianHuaPower : AbstractGuZhenRenPower
{
    private sealed class Data
    {
        public bool HpLostThisTurn;
    }

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override bool AllowNegative => false;

    protected override object InitInternalData()
    {
        return new Data();
    }

    public override Task AfterSideTurnStart(CombatSide side, CombatState combatState)
    {
        if (side == Owner.Side)
        {
            GetInternalData<Data>().HpLostThisTurn = false;
        }

        return Task.CompletedTask;
    }

    public override Task AfterDamageReceived(PlayerChoiceContext choiceContext, Creature target, DamageResult result, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (target == Owner && result.UnblockedDamage > 0)
        {
            GetInternalData<Data>().HpLostThisTurn = true;
        }

        return Task.CompletedTask;
    }

    public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        var data = GetInternalData<Data>();
        if (side != Owner.Side || !data.HpLostThisTurn)
        {
            return;
        }

        data.HpLostThisTurn = false;
        Flash();
        await PowerCmd.Apply<BufferPower>(Owner, Amount, Owner, null);
    }
}
