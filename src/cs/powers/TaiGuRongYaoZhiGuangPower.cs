using BaseLib.Abstracts;
using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Guzhenren.Scripts;

public sealed class TaiGuRongYaoZhiGuangPower : AbstractGuZhenRenPower
{
    public override PowerType Type => PowerType.Debuff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override bool AllowNegative => false;

    public override async Task AfterDamageReceived(PlayerChoiceContext choiceContext, Creature target, DamageResult result, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (target != Owner || dealer == null || cardSource is not AbstractGuZhenRenCard guCard || guCard.PrimaryDao != GuZhenRenDao.GuangDao || !props.IsPoweredAttack_())
        {
            return;
        }

        var reflectedHits = Amount;
        var reflectedDamage = result.TotalDamage;
        if (reflectedHits <= 0 || reflectedDamage <= 0)
        {
            return;
        }

        for (var i = 0; i < reflectedHits; i++)
        {
            await CreatureCmd.Damage(choiceContext, Owner, reflectedDamage, ValueProp.Unpowered, dealer, null);
        }
    }
}
