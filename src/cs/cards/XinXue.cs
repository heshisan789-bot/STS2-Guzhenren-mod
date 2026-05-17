using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace Guzhenren.Scripts;

[Pool(typeof(GuZhenRenCardPool))]
[GuZhenRenDaoPool(GuZhenRenDao.XueDao)]
public sealed class XinXue : AbstractBenMingGuCard
{
    private const string HealAmountKey = "HealAmount";

    protected override int MaxRank => 8;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar(HealAmountKey, 2m),
        new PowerVar<XinXuePower>(1m)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<XinXuePower>()];

    public XinXue() : base(1, CardType.Power, CardRarity.Token, TargetType.Self)
    {
        SetDao(GuZhenRenDao.XueDao);
        SetRank(1);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var healAmount = DynamicVars[HealAmountKey].BaseValue;
        if (healAmount > 0m)
        {
            await CreatureCmd.Heal(Owner.Creature, healAmount);
        }

        await PowerCmd.Apply<XinXuePower>(Owner.Creature, DynamicVars["XinXuePower"].BaseValue, Owner.Creature, this);
    }

    protected override void OnRankChanged()
    {
        base.OnRankChanged();

        var clampedRank = Math.Clamp(Rank, 1, 8);
        var healValues = new[] { 2m, 2m, 3m, 3m, 4m, 4m, 5m, 5m };
        var multiplierValues = new[] { 1m, 2m, 2m, 3m, 3m, 4m, 4m, 5m };

        DynamicVars[HealAmountKey].BaseValue = healValues[clampedRank - 1];
        DynamicVars["XinXuePower"].BaseValue = multiplierValues[clampedRank - 1];

        if (IsCanonical)
        {
            return;
        }

        if (clampedRank >= 6)
        {
            AddKeyword(CardKeyword.Innate);
        }
        else
        {
            RemoveKeyword(CardKeyword.Innate);
        }
    }

    protected override void OnUpgrade()
    {
        UpgradeRank(1);
    }
}
