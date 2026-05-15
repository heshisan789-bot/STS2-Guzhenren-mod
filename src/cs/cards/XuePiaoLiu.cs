using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace Guzhenren.Scripts;

[Pool(typeof(GuZhenRenCardPool))]
[GuZhenRenDaoPool(GuZhenRenDao.XueDao)]
public sealed class XuePiaoLiu : AbstractShaZhaoCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<XuePiaoLiuPower>(1m)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<XuePiaoLiuPower>()];

    public XuePiaoLiu() : base(1, CardType.Power, TargetType.Self)
    {
        SetDao(GuZhenRenDao.XueDao);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<XuePiaoLiuPower>(Owner.Creature, DynamicVars["XuePiaoLiuPower"].BaseValue, Owner.Creature, this);
    }
}
