using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace Guzhenren.Scripts;

[Pool(typeof(GuZhenRenCardPool))]
[GuZhenRenDaoPool(GuZhenRenDao.ZhiDao)]
public sealed class YiXinErYongGu : AbstractGuZhenRenCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<NianPower>(3m),
        new PowerVar<YiXinErYongPower>(1m)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<NianPower>(),
        HoverTipFactory.FromPower<YiXinErYongPower>()
    ];

    public YiXinErYongGu() : base(1, CardType.Skill, CardRarity.Common, TargetType.Self)
    {
        SetDao(GuZhenRenDao.ZhiDao);
        SetRank(2);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<NianPower>(Owner.Creature, DynamicVars["NianPower"].BaseValue, Owner.Creature, this);
        await PowerCmd.Apply<YiXinErYongPower>(Owner.Creature, DynamicVars["YiXinErYongPower"].BaseValue, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["YiXinErYongPower"].UpgradeValueBy(1m);
        UpgradeRank(1);
    }
}
