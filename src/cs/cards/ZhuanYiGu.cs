using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace Guzhenren.Scripts;

[Pool(typeof(GuZhenRenCardPool))]
[GuZhenRenDaoPool(GuZhenRenDao.ZhiDao)]
public sealed class ZhuanYiGu : AbstractGuZhenRenCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<ZhuanYiPower>(3m)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<ZhuanYiPower>()];

    public ZhuanYiGu() : base(1, CardType.Power, CardRarity.Uncommon, TargetType.Self)
    {
        SetDao(GuZhenRenDao.ZhiDao);
        SetRank(4);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<ZhuanYiPower>(Owner.Creature, DynamicVars["ZhuanYiPower"].BaseValue, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["ZhuanYiPower"].UpgradeValueBy(1m);
        UpgradeRank(1);
    }
}
