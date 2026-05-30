using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace Guzhenren.Scripts;

[Pool(typeof(GuZhenRenCardPool))]
[GuZhenRenDaoPool(GuZhenRenDao.BianHuaDao)]
public sealed class BianXing : AbstractBenMingGuCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<BianHuaDaoDaoHenPower>(1m)];
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<BianHuaDaoDaoHenPower>()];

    protected override int MaxRank => 8;

    public BianXing() : base(1, CardType.Power, CardRarity.Token, TargetType.Self)
    {
        SetDao(GuZhenRenDao.BianHuaDao);
        SetRank(1);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<BianHuaDaoDaoHenPower>(Owner.Creature, DynamicVars["BianHuaDaoDaoHenPower"].BaseValue, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["BianHuaDaoDaoHenPower"].UpgradeValueBy(1m);
        UpgradeRank(1);
    }
}
