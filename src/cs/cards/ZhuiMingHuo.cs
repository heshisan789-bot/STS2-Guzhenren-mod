using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace Guzhenren.Scripts;

[Pool(typeof(GuZhenRenCardPool))]
[GuZhenRenDaoPool(GuZhenRenDao.YanDao)]
public sealed class ZhuiMingHuo : AbstractShaZhaoCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<ZhuiMingHuoPower>(1m)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<ZhuiMingHuoPower>(),
        HoverTipFactory.FromPower<FenShaoPower>()
    ];

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    public ZhuiMingHuo() : base(2, CardType.Skill, TargetType.AnyEnemy)
    {
        SetDao(GuZhenRenDao.YanDao);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, nameof(cardPlay.Target));
        await PowerCmd.Apply<ZhuiMingHuoPower>(cardPlay.Target, DynamicVars["ZhuiMingHuoPower"].BaseValue, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
    }
}
