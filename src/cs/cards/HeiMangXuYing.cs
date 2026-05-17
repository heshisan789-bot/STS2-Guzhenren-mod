using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Guzhenren.Scripts;

[Pool(typeof(GuZhenRenCardPool))]
[GuZhenRenDaoPool(GuZhenRenDao.LiDao)]
public sealed class HeiMangXuYing : AbstractXuYingCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        ..base.CanonicalVars,
        new PowerVar<ConstrictPower>(4m)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<ConstrictPower>()];

    public HeiMangXuYing() : base(CardType.Skill, TargetType.AnyEnemy, 25m)
    {
    }

    protected override Task TriggerPhantomEffect(PlayerChoiceContext choiceContext, Creature? target)
    {
        if (target == null)
        {
            return Task.CompletedTask;
        }

        return PowerCmd.Apply<ConstrictPower>(target, DynamicVars["ConstrictPower"].BaseValue, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["ConstrictPower"].UpgradeValueBy(2m);
    }
}
