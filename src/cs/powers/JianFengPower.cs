using BaseLib.Abstracts;
using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Guzhenren.Scripts;

public sealed class JianFengPower : AbstractGuZhenRenPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override bool AllowNegative => false;

    public override async Task AfterDamageGiven(PlayerChoiceContext choiceContext, Creature? dealer, DamageResult result, ValueProp props, Creature target, CardModel? cardSource)
    {
        if (dealer != Owner || target == Owner || cardSource is not AbstractGuZhenRenCard guCard || guCard.PrimaryDao != GuZhenRenDao.JianDao || !props.IsPoweredAttack_() || result.TotalDamage <= 0)
        {
            return;
        }

        await PowerCmd.Apply<JianHenPower>(target, Amount, Owner, null);
    }
}
