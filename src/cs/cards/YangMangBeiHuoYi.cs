using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace Guzhenren.Scripts;

[Pool(typeof(GuZhenRenCardPool))]
[GuZhenRenDaoPool(GuZhenRenDao.YanDao)]
public sealed class YangMangBeiHuoYi : AbstractShaZhaoCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(24m, ValueProp.Move),
        new DynamicVar("Times", 3m),
        new PowerVar<FenShaoPower>(1m)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<FenShaoPower>()];

    public YangMangBeiHuoYi() : base(2, CardType.Skill, TargetType.Self)
    {
        SetDao(GuZhenRenDao.YanDao);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, cardPlay);
        await PowerCmd.Apply<YangMangBeiHuoYiPower>(Owner.Creature, DynamicVars["Times"].BaseValue, Owner.Creature, this);
    }
}

