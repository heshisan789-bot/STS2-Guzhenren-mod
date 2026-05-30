using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace Guzhenren.Scripts;

[Pool(typeof(GuZhenRenCardPool))]
[GuZhenRenDaoPool(GuZhenRenDao.BianHuaDao)]
public sealed class WanWuDaTongBian : AbstractShaZhaoCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<WanWuDaTongBianPower>(1m)];
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<WanWuDaTongBianPower>()];

    public WanWuDaTongBian() : base(2, CardType.Power, TargetType.Self)
    {
        SetDao(GuZhenRenDao.BianHuaDao);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<WanWuDaTongBianPower>(Owner.Creature, DynamicVars["WanWuDaTongBianPower"].BaseValue, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
    }
}
