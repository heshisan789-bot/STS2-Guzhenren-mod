using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Guzhenren.Scripts;

public sealed class XueYuanPower : AbstractGuZhenRenPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;
    public override bool AllowNegative => false;
    public override int DisplayAmount => 0;

    public override async Task AfterDamageReceived(PlayerChoiceContext choiceContext, Creature target, DamageResult result, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (target != Owner || result.UnblockedDamage <= 0)
        {
            return;
        }

        var combatState = Owner.CombatState;
        if (combatState == null)
        {
            return;
        }

        var markedEnemies = combatState.HittableEnemies
            .Where(enemy => enemy.GetPower<XueYuanMarkPower>() is { Amount: > 0 })
            .ToList();

        if (markedEnemies.Count == 0)
        {
            await PowerCmd.Remove(this);
            return;
        }

        Flash();
        foreach (var enemy in markedEnemies)
        {
            var multiplier = enemy.GetPower<XueYuanMarkPower>()!.Amount;
            await CreatureCmd.Damage(choiceContext, enemy, result.UnblockedDamage * multiplier, ValueProp.Unblockable | ValueProp.Unpowered, Owner);
        }
    }

    public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if (side != Owner.Side)
        {
            return;
        }

        var combatState = Owner.CombatState;
        if (combatState == null || !combatState.HittableEnemies.Any(enemy => enemy.GetPower<XueYuanMarkPower>() != null))
        {
            await PowerCmd.Remove(this);
        }
    }
}
