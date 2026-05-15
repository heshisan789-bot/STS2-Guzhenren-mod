using BaseLib.Abstracts;
using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Guzhenren.Scripts;

public sealed class XueYuanMarkPower : AbstractGuZhenRenPower
{
    public override PowerType Type => PowerType.Debuff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override bool AllowNegative => false;

    public override decimal ModifyDamageMultiplicative(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (dealer != Owner || !props.IsPoweredAttack_())
        {
            return 1m;
        }

        var player = Owner.CombatState?.Players.FirstOrDefault();
        if (player?.Creature.GetPower<XueDianXingBanPower>() != null)
        {
            return 0.5m;
        }

        return 1m;
    }

    public override async Task AfterDeath(PlayerChoiceContext choiceContext, Creature creature, bool wasRemovalPrevented, float deathAnimLength)
    {
        if (creature == Owner)
        {
            await RemovePlayerBondIfLastMark();
        }
    }

    public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if (Owner.IsDead)
        {
            await RemovePlayerBondIfLastMark();
        }
    }

    private async Task RemovePlayerBondIfLastMark()
    {
        var combatState = Owner.CombatState;
        if (combatState == null)
        {
            return;
        }

        var hasOtherMarks = combatState.HittableEnemies.Any(enemy =>
            enemy != Owner &&
            enemy.GetPower<XueYuanMarkPower>() != null);

        if (!hasOtherMarks)
        {
            var player = combatState.Players.FirstOrDefault();
            var playerBond = player?.Creature.GetPower<XueYuanPower>();
            if (playerBond != null)
            {
                await PowerCmd.Remove(playerBond);
            }
        }
    }
}
