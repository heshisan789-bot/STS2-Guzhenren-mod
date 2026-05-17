using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Guzhenren.Scripts;

[Pool(typeof(GuZhenRenCardPool))]
[GuZhenRenDaoPool(GuZhenRenDao.LiDao)]
public sealed class ZiLiGengShengGu : AbstractGuZhenRenCard
{
    private const string CalculatedHealKey = "CalculatedHeal";

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CalculationBaseVar(4m),
        new CalculatedVar(CalculatedHealKey).WithMultiplier(static (card, _) => ((ZiLiGengShengGu)card).CalculateHealAmount())
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<StrengthPower>()];

    public ZiLiGengShengGu() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        SetDao(GuZhenRenDao.LiDao);
        SetRank(3);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var amount = CalculateHealAmount();
        if (amount <= 0m)
        {
            return;
        }

        await CreatureCmd.Heal(Owner.Creature, amount);
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
        UpgradeRank(1);
    }

    private decimal CalculateHealAmount()
    {
        var baseHeal = DynamicVars.CalculationBase.BaseValue;
        var strength = Owner.Creature.GetPowerAmount<StrengthPower>();
        return Math.Max(0m, baseHeal + strength);
    }
}
