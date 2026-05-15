using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;

namespace Guzhenren.Scripts;

[Pool(typeof(GuZhenRenCardPool))]
[GuZhenRenDaoPool(GuZhenRenDao.XueDao)]
public sealed class XueChou : AbstractGuZhenRenCard
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => IsUpgraded
        ? [CardKeyword.Exhaust, CardKeyword.Retain]
        : [CardKeyword.Exhaust];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<XueChouPower>()];

    public XueChou() : base(0, CardType.Skill, CardRarity.Rare, TargetType.AnyEnemy)
    {
        SetDao(GuZhenRenDao.XueDao);
        SetRank(6);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, nameof(cardPlay.Target));
        await PowerCmd.Apply<XueChouPower>(cardPlay.Target, -1m, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        UpgradeRank(1);
    }
}
