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
public sealed class XueMuTianHuaGu : AbstractGuZhenRenCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<XueMuTianHuaPower>(1m)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<XueMuTianHuaPower>(),
        HoverTipFactory.FromPower<BufferPower>()
    ];

    public XueMuTianHuaGu() : base(2, CardType.Power, CardRarity.Rare, TargetType.Self)
    {
        SetDao(GuZhenRenDao.XueDao);
        SetRank(5);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<XueMuTianHuaPower>(Owner.Creature, DynamicVars["XueMuTianHuaPower"].BaseValue, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
        UpgradeRank(1);
    }
}
