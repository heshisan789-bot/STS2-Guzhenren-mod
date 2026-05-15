using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace Guzhenren.Scripts;

[Pool(typeof(GuZhenRenCardPool))]
[GuZhenRenDaoPool(GuZhenRenDao.GuangDao)]
public sealed class GuangGu : AbstractGuZhenRenCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<ShanYaoPower>(2m),
        new PowerVar<RiGuangPower>(1m)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<ShanYaoPower>(),
        HoverTipFactory.FromPower<RiGuangPower>()
    ];

    public GuangGu() : base(1, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
        SetDao(GuZhenRenDao.GuangDao);
        SetRank(8);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ShanYaoPower.RecordGain(DynamicVars["ShanYaoPower"].BaseValue);
        await PowerCmd.Apply<ShanYaoPower>(Owner.Creature, DynamicVars["ShanYaoPower"].BaseValue, Owner.Creature, this);
        await PowerCmd.Apply<RiGuangPower>(Owner.Creature, DynamicVars["RiGuangPower"].BaseValue, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["ShanYaoPower"].UpgradeValueBy(1m);
        UpgradeRank(1);
    }
}
