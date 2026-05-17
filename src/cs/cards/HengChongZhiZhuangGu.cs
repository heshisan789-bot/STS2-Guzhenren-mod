using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace Guzhenren.Scripts;

[Pool(typeof(GuZhenRenCardPool))]
[GuZhenRenDaoPool(GuZhenRenDao.LiDao)]
public sealed class HengChongZhiZhuangGu : AbstractGuZhenRenCard
{
    private const string SelfDamageKey = "SelfDamage";

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(7m, ValueProp.Move),
        new DynamicVar(SelfDamageKey, 2m)
    ];

    public HengChongZhiZhuangGu() : base(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
    {
        SetDao(GuZhenRenDao.LiDao);
        SetRank(4);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, nameof(cardPlay.Target));

        for (var i = 0; i < 2; i++)
        {
            var damage = DynamicVars.Damage.BaseValue;
            var wasFullyBlocked = damage > 0m && cardPlay.Target.Block >= damage;

            await DamageCmd.Attack(damage).FromCard(this).Targeting(cardPlay.Target)
                .WithHitFx("vfx/vfx_attack_blunt")
                .Execute(choiceContext);

            if (wasFullyBlocked)
            {
                await CreatureCmd.Damage(choiceContext, Owner.Creature, DynamicVars[SelfDamageKey].BaseValue, ValueProp.Unpowered | ValueProp.Move, Owner.Creature, this);
            }
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(2m);
        UpgradeRank(1);
    }
}
