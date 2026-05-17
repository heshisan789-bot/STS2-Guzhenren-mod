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
public sealed class GuiLiXuYing : AbstractXuYingCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        ..base.CanonicalVars,
        new CalculationBaseVar(3m),
        new CalculationExtraVar(1m),
        new CalculatedBlockVar(ValueProp.Move).WithMultiplier(static (card, _) =>
        {
            var strength = card.Owner.Creature.GetPowerAmount<StrengthPower>();
            return strength;
        })
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<StrengthPower>()];

    public override bool GainsBlock => true;

    public GuiLiXuYing() : base(CardType.Skill, TargetType.Self, 30m)
    {
    }

    protected override async Task TriggerPhantomEffect(PlayerChoiceContext choiceContext, Creature? target)
    {
        var amount = DynamicVars.CalculatedBlock.Calculate(null);
        if (amount > 0m)
        {
            await CreatureCmd.GainBlock(Owner.Creature, amount, ValueProp.Move, null);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars[ChanceKey].UpgradeValueBy(10m);
        DynamicVars.CalculationBase.UpgradeValueBy(1m);
    }
}
