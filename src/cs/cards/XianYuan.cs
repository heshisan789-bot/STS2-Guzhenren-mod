using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace Guzhenren.Scripts;

[Pool(typeof(GuZhenRenCardPool))]
public abstract class AbstractXianYuanCard : AbstractGuZhenRenCard
{
    public override string PortraitPath => GuZhenRenArtPaths.GetCardPortrait("Xian");

    public override string BetaPortraitPath => GuZhenRenArtPaths.GetCardBetaPortrait("Xian");

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Retain, CardKeyword.Exhaust];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [EnergyHoverTip];

    protected override IEnumerable<DynamicVar> CanonicalVars => [new EnergyVar(EnergyGain)];

    protected abstract int EnergyGain { get; }

    protected AbstractXianYuanCard() : base(0, CardType.Skill, CardRarity.Token, TargetType.None)
    {
        SetRank(0);
    }

    public override GuZhenRenBannedCardSources BannedSources => GuZhenRenBannedCardSources.All;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PlayerCmd.GainEnergy(DynamicVars.Energy.BaseValue, Owner);
    }

    protected override void OnUpgrade()
    {
    }
}

[Pool(typeof(GuZhenRenCardPool))]
public sealed class QingTiXianYuan : AbstractXianYuanCard
{
    protected override int EnergyGain => 2;
    public override string PortraitPath => GuZhenRenArtPaths.GetRelicIcon("XianQiao_6");
    public override string BetaPortraitPath => GuZhenRenArtPaths.GetRelicIcon("XianQiao_6");
}

[Pool(typeof(GuZhenRenCardPool))]
public sealed class HongZaoXianYuan : AbstractXianYuanCard
{
    protected override int EnergyGain => 3;
    public override string PortraitPath => GuZhenRenArtPaths.GetRelicIcon("XianQiao_7");
    public override string BetaPortraitPath => GuZhenRenArtPaths.GetRelicIcon("XianQiao_7");
}

[Pool(typeof(GuZhenRenCardPool))]
public sealed class BaiLiXianYuan : AbstractXianYuanCard
{
    protected override int EnergyGain => 4;
    public override string PortraitPath => GuZhenRenArtPaths.GetRelicIcon("XianQiao_8");
    public override string BetaPortraitPath => GuZhenRenArtPaths.GetRelicIcon("XianQiao_8");
}

[Pool(typeof(GuZhenRenCardPool))]
public sealed class HuangXingXianYuan : AbstractXianYuanCard
{
    protected override int EnergyGain => 5;
    public override string PortraitPath => GuZhenRenArtPaths.GetRelicIcon("XianQiao_9");
    public override string BetaPortraitPath => GuZhenRenArtPaths.GetRelicIcon("XianQiao_9");
}
