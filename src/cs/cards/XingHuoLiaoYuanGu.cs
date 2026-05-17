using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace Guzhenren.Scripts;

[Pool(typeof(GuZhenRenCardPool))]
[GuZhenRenDaoPool(GuZhenRenDao.YanDao)]
public sealed class XingHuoLiaoYuanGu : AbstractGuZhenRenCard
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("Times", 1m),
        new PowerVar<FenShaoPower>(1m)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<FenShaoPower>(),
        HoverTipFactory.FromPower<XingHuoLiaoYuanPower>()
    ];

    public XingHuoLiaoYuanGu() : base(2, CardType.Skill, CardRarity.Rare, TargetType.AnyEnemy)
    {
        SetDao(GuZhenRenDao.YanDao);
        SetRank(5);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, nameof(cardPlay.Target));

        for (var i = 0; i < DynamicVars["Times"].IntValue; i++)
        {
            await PowerCmd.Apply<FenShaoPower>(cardPlay.Target, DynamicVars["FenShaoPower"].BaseValue, Owner.Creature, this);
        }

        await PowerCmd.Apply<XingHuoLiaoYuanPower>(cardPlay.Target, 1m, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["Times"].UpgradeValueBy(2m);
        UpgradeRank(1);
    }
}

