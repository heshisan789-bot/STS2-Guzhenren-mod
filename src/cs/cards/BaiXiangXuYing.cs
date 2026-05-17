using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace Guzhenren.Scripts;

[Pool(typeof(GuZhenRenCardPool))]
[GuZhenRenDaoPool(GuZhenRenDao.LiDao)]
public sealed class BaiXiangXuYing : AbstractXuYingCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        ..base.CanonicalVars,
        new DamageVar(5m, ValueProp.Move),
        new PowerVar<StrengthPower>(1m)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<StrengthPower>()];

    public BaiXiangXuYing() : base(CardType.Attack, TargetType.AnyEnemy, 20m)
    {
    }

    protected override async Task TriggerPhantomEffect(PlayerChoiceContext choiceContext, Creature? target)
    {
        if (target == null)
        {
            return;
        }

        await PowerCmd.Apply<StrengthPower>(Owner.Creature, DynamicVars["StrengthPower"].BaseValue, Owner.Creature, this);
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(target)
            .WithHitFx("vfx/vfx_attack_blunt")
            .Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        DynamicVars[ChanceKey].UpgradeValueBy(10m);
        DynamicVars.Damage.UpgradeValueBy(1m);
    }
}
