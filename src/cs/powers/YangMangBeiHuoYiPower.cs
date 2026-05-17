using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Guzhenren.Scripts;

public sealed class YangMangBeiHuoYiPower : AbstractGuZhenRenPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override bool AllowNegative => false;

    public override async Task AfterDamageReceived(PlayerChoiceContext choiceContext, Creature target, DamageResult _, ValueProp props, Creature? dealer, CardModel? __)
    {
        if (target != Owner || dealer == null || dealer == Owner || dealer.Side == Owner.Side || !props.IsPoweredAttack() || Amount <= 0)
        {
            return;
        }

        Flash();
        for (var i = 0; i < Amount; i++)
        {
            await PowerCmd.Apply<FenShaoPower>(dealer, 1m, Owner, null);
        }
    }

    public override async Task AfterSideTurnStart(CombatSide side, CombatState combatState)
    {
        if (side == Owner.Side)
        {
            await PowerCmd.Remove(this);
        }
    }
}

