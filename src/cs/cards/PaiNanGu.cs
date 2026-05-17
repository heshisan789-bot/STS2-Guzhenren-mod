using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace Guzhenren.Scripts;

[Pool(typeof(GuZhenRenCardPool))]
[GuZhenRenDaoPool(GuZhenRenDao.YunDao)]
public sealed class PaiNanGu : AbstractGuZhenRenCard
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => IsUpgraded ? [CardKeyword.Innate] : Array.Empty<CardKeyword>();

    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<PaiNanPower>(1m)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<PaiNanPower>()];

    public PaiNanGu() : base(1, CardType.Power, CardRarity.Uncommon, TargetType.Self)
    {
        SetDao(GuZhenRenDao.YunDao);
        SetRank(7);
    }

    protected override Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        return PowerCmd.Apply<PaiNanPower>(Owner.Creature, DynamicVars["PaiNanPower"].BaseValue, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        UpgradeRank(1);
    }
}
