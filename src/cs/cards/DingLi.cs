using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace Guzhenren.Scripts;

[Pool(typeof(GuZhenRenCardPool))]
[GuZhenRenDaoPool(GuZhenRenDao.LiDao)]
public sealed class DingLi : AbstractGuZhenRenCard
{
    private const string StrengthMultiplierKey = "StrengthMultiplier";

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Retain];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<StrengthPower>()];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar(StrengthMultiplierKey, 3m),
        new CalculationBaseVar(10m),
        new CalculationExtraVar(1m),
        new CalculatedBlockVar(ValueProp.Move).WithMultiplier((CardModel card, Creature? _) =>
        {
            var multiplier = card.DynamicVars[StrengthMultiplierKey].BaseValue;
            var strength = card.Owner.Creature.GetPowerAmount<StrengthPower>();
            return strength * multiplier;
        })
    ];

    public override bool GainsBlock => true;

    public DingLi() : base(2, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        SetDao(GuZhenRenDao.LiDao);
        SetRank(6);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.CalculatedBlock.Calculate(cardPlay.Target), DynamicVars.CalculatedBlock.Props, cardPlay);
    }

    protected override void OnUpgrade()
    {
        DynamicVars[StrengthMultiplierKey].UpgradeValueBy(2m);
        UpgradeRank(1);
    }
}
