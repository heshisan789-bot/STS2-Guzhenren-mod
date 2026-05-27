using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace Guzhenren.Scripts;

[Pool(typeof(GuZhenRenCardPool))]
[GuZhenRenDaoPool(GuZhenRenDao.ZhiDao)]
public sealed class WanXingFeiYing : AbstractShaZhaoCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<WanXingFeiYingPower>(1m)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<WanXingFeiYingPower>(),
        HoverTipFactory.FromPower<NianPower>()
    ];

    public WanXingFeiYing() : base(1, CardType.Power, TargetType.Self)
    {
        SetDao(GuZhenRenDao.ZhiDao);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<WanXingFeiYingPower>(Owner.Creature, DynamicVars["WanXingFeiYingPower"].BaseValue, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
    }
}

