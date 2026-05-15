using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Guzhenren.Scripts;

public sealed class XinXuePower : AbstractGuZhenRenPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override bool AllowNegative => false;

    public override async Task AfterDamageReceived(PlayerChoiceContext choiceContext, Creature target, DamageResult result, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (target != Owner || result.UnblockedDamage <= 0m || Owner.CombatState?.CurrentSide != CombatSide.Player)
        {
            return;
        }

        ArgumentNullException.ThrowIfNull(CombatState, nameof(CombatState));
        Flash();
        foreach (var enemy in CombatState.HittableEnemies)
        {
            await CreatureCmd.Damage(choiceContext, enemy, result.UnblockedDamage * Amount, ValueProp.Unblockable | ValueProp.Unpowered, Owner);
        }
    }
}
