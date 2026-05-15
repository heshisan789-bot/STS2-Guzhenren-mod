using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Guzhenren.Scripts;

[Pool(typeof(GuZhenRenCardPool))]
[GuZhenRenDaoPool(GuZhenRenDao.XueDao)]
public sealed class XueZhanGu : AbstractGuZhenRenCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<XueZhanPower>(1m)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<XueZhanPower>(),
        HoverTipFactory.FromPower<StrengthPower>()
    ];

    public XueZhanGu() : base(1, CardType.Power, CardRarity.Uncommon, TargetType.Self)
    {
        SetDao(GuZhenRenDao.XueDao);
        SetRank(5);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<XueZhanPower>(Owner.Creature, DynamicVars["XueZhanPower"].BaseValue, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
        UpgradeRank(1);
    }
}
