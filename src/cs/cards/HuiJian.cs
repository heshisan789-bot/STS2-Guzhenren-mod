using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace Guzhenren.Scripts;

[Pool(typeof(GuZhenRenCardPool))]
[GuZhenRenDaoPool(GuZhenRenDao.JianDao)]
public sealed class HuiJian : AbstractGuZhenRenCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(9m, ValueProp.Move),
        new PowerVar<NianPower>(5m)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<NianPower>(),
        HoverTipFactory.FromPower<QingPower>(),
        HoverTipFactory.FromPower<JianFengPower>()
    ];

    public HuiJian() : base(1, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
    {
        SetDao(GuZhenRenDao.JianDao);
        SetRank(7);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, nameof(cardPlay.Target));

        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_heavy_slash")
            .Execute(choiceContext);

        await PowerCmd.Apply<NianPower>(Owner.Creature, DynamicVars["NianPower"].BaseValue, Owner.Creature, this);

        if (Owner.Creature.GetPowerAmount<QingPower>() > 0)
        {
            await PowerCmd.Apply<QingPower>(Owner.Creature, -1, Owner.Creature, this, true);
            await PowerCmd.Apply<JianFengPower>(Owner.Creature, 1, Owner.Creature, this);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3m);
        UpgradeRank(1);
    }
}
