using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace Guzhenren.Scripts;

[Pool(typeof(GuZhenRenCardPool))]
[GuZhenRenDaoPool(GuZhenRenDao.ZhiDao)]
public sealed class ZhiHuiGu : AbstractBenMingGuCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<ZhiHuiPower>(2m)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<ZhiHuiPower>(),
        HoverTipFactory.FromPower<NianPower>()
    ];

    public ZhiHuiGu() : base(1, CardType.Power, CardRarity.Token, TargetType.Self)
    {
        SetDao(GuZhenRenDao.ZhiDao);
        SetRank(1);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<ZhiHuiPower>(Owner.Creature, DynamicVars["ZhiHuiPower"].BaseValue, Owner.Creature, this);
    }

    protected override void OnRankChanged()
    {
        base.OnRankChanged();

        var amount = Rank >= 9 ? 9m : Rank + 1m;
        DynamicVars["ZhiHuiPower"].BaseValue = amount;

        if (IsCanonical)
        {
            return;
        }

        if (Rank >= 9)
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
