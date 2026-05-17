using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace Guzhenren.Scripts;

[Pool(typeof(GuZhenRenCardPool))]
[GuZhenRenDaoPool(GuZhenRenDao.TouDao)]
public sealed class TouSheng : AbstractGuZhenRenCard
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("MaxHp", 1m)];

    public TouSheng() : base(3, CardType.Skill, CardRarity.Rare, TargetType.AllEnemies)
    {
        SetDao(GuZhenRenDao.TouDao);
        SetRank(8);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(CombatState, nameof(CombatState));

        var stealAmount = (int)DynamicVars["MaxHp"].BaseValue;
        if (stealAmount <= 0)
        {
            return;
        }

        var totalStolen = 0m;
        foreach (var enemy in CombatState.HittableEnemies)
        {
            if (!enemy.IsAlive)
            {
                continue;
            }

            var actualSteal = Math.Min(stealAmount, enemy.MaxHp);
            if (actualSteal <= 0)
            {
                continue;
            }

            var newMax = enemy.MaxHp - actualSteal;
            if (newMax < enemy.CurrentHp)
            {
                await CreatureCmd.Damage(choiceContext, enemy, enemy.CurrentHp - newMax, ValueProp.Unblockable | ValueProp.Unpowered | ValueProp.Move, Owner.Creature, this);
            }

            await CreatureCmd.SetMaxHp(enemy, Math.Max(0m, newMax));
            totalStolen += actualSteal;
        }

        if (totalStolen > 0m)
        {
            await CreatureCmd.GainMaxHp(Owner.Creature, totalStolen);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars["MaxHp"].UpgradeValueBy(1m);
        UpgradeRank(1);
    }
}
