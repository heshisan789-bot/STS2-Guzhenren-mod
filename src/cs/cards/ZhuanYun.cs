using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace Guzhenren.Scripts;

[Pool(typeof(GuZhenRenCardPool))]
[GuZhenRenDaoPool(GuZhenRenDao.YunDao)]
public sealed class ZhuanYun : AbstractGuZhenRenCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<ZhuanYunPower>(4m)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<ZhuanYunPower>()];

    public ZhuanYun() : base(1, CardType.Power, CardRarity.Uncommon, TargetType.Self)
    {
        SetDao(GuZhenRenDao.YunDao);
        SetRank(6);
    }

    protected override Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        return PowerCmd.Apply<ZhuanYunPower>(Owner.Creature, DynamicVars["ZhuanYunPower"].BaseValue, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["ZhuanYunPower"].UpgradeValueBy(2m);
        UpgradeRank(1);
    }
}
