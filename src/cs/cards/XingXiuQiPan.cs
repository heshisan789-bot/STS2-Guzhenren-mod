using BaseLib.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Guzhenren.Scripts;

[Pool(typeof(GuZhenRenCardPool))]
[GuZhenRenDaoPool(GuZhenRenDao.ZhiDao)]
public sealed class XingXiuQiPan : AbstractXianGuWuCard
{
    private static bool _usedTengNuoThisCombat;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<XingLuoQiBuPower>(),
        HoverTipFactory.FromPower<NianPower>(),
        HoverTipFactory.FromPower<BuMieXingBiaoPower>(),
        HoverTipFactory.FromCard<OptionFangHuXingXiuQiPan>(),
        HoverTipFactory.FromCard<OptionZhenChaXingXiuQiPan>(),
        HoverTipFactory.FromCard<OptionTuiSuanXingXiuQiPan>(),
        HoverTipFactory.FromCard<OptionTengNuoXingXiuQiPan>()
    ];

    static XingXiuQiPan()
    {
        BattleStateManager.OnBattleStart(() => _usedTengNuoThisCombat = false);
        BattleStateManager.OnPostBattle(() => _usedTengNuoThisCombat = false);
    }

    public XingXiuQiPan() : base(1, CardType.Skill, TargetType.None)
    {
        SetDao(GuZhenRenDao.ZhiDao);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(CombatState, nameof(CombatState));

        var choices = new List<CardModel>
        {
            CombatState.CreateCard<OptionFangHuXingXiuQiPan>(Owner),
            CombatState.CreateCard<OptionZhenChaXingXiuQiPan>(Owner),
            CombatState.CreateCard<OptionTuiSuanXingXiuQiPan>(Owner)
        };

        if (!_usedTengNuoThisCombat)
        {
            choices.Add(CombatState.CreateCard<OptionTengNuoXingXiuQiPan>(Owner));
        }

        var selected = (await CardSelectCmd.FromSimpleGrid(
            choiceContext,
            choices,
            Owner,
            new CardSelectorPrefs(SelectionScreenPrompt, 1))).FirstOrDefault();

        switch (selected)
        {
            case OptionFangHuXingXiuQiPan:
                await PowerCmd.Apply<XingLuoQiBuPower>(Owner.Creature, 3m, Owner.Creature, this);
                break;
            case OptionZhenChaXingXiuQiPan:
                foreach (var enemy in CombatState.HittableEnemies)
                {
                    await PowerCmd.Apply<BuMieXingBiaoPower>(enemy, 1m, Owner.Creature, this);
                }
                break;
            case OptionTuiSuanXingXiuQiPan:
                await PowerCmd.Apply<NianPower>(Owner.Creature, 8m, Owner.Creature, this);
                break;
            case OptionTengNuoXingXiuQiPan:
                _usedTengNuoThisCombat = true;
                foreach (var enemy in CombatState.HittableEnemies)
                {
                    await CreatureCmd.Stun(enemy);
                }
                PlayerCmd.EndTurn(Owner, canBackOut: false);
                break;
        }
    }

    protected override void OnUpgrade()
    {
    }
}

[Pool(typeof(GuZhenRenCardPool))]
public sealed class OptionFangHuXingXiuQiPan : AbstractGuZhenRenCard
{
    public override bool GainsBlock => true;
    public override GuZhenRenBannedCardSources BannedSources => GuZhenRenBannedCardSources.All;
    public override string PortraitPath => GuZhenRenArtPaths.GetCardPortrait(nameof(XingXiuQiPan));
    public override string BetaPortraitPath => GuZhenRenArtPaths.GetCardBetaPortrait(nameof(XingXiuQiPan));

    public OptionFangHuXingXiuQiPan() : base(-2, CardType.Skill, CardRarity.Token, TargetType.Self)
    {
        SetRank(0);
    }

    protected override Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay) => Task.CompletedTask;
}

[Pool(typeof(GuZhenRenCardPool))]
public sealed class OptionZhenChaXingXiuQiPan : AbstractGuZhenRenCard
{
    public override GuZhenRenBannedCardSources BannedSources => GuZhenRenBannedCardSources.All;
    public override string PortraitPath => GuZhenRenArtPaths.GetCardPortrait(nameof(XingXiuQiPan));
    public override string BetaPortraitPath => GuZhenRenArtPaths.GetCardBetaPortrait(nameof(XingXiuQiPan));

    public OptionZhenChaXingXiuQiPan() : base(-2, CardType.Skill, CardRarity.Token, TargetType.None)
    {
        SetRank(0);
    }

    protected override Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay) => Task.CompletedTask;
}

[Pool(typeof(GuZhenRenCardPool))]
public sealed class OptionTuiSuanXingXiuQiPan : AbstractGuZhenRenCard
{
    public override GuZhenRenBannedCardSources BannedSources => GuZhenRenBannedCardSources.All;
    public override string PortraitPath => GuZhenRenArtPaths.GetCardPortrait(nameof(XingXiuQiPan));
    public override string BetaPortraitPath => GuZhenRenArtPaths.GetCardBetaPortrait(nameof(XingXiuQiPan));

    public OptionTuiSuanXingXiuQiPan() : base(-2, CardType.Skill, CardRarity.Token, TargetType.None)
    {
        SetRank(0);
    }

    protected override Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay) => Task.CompletedTask;
}

[Pool(typeof(GuZhenRenCardPool))]
public sealed class OptionTengNuoXingXiuQiPan : AbstractGuZhenRenCard
{
    public override GuZhenRenBannedCardSources BannedSources => GuZhenRenBannedCardSources.All;
    public override string PortraitPath => GuZhenRenArtPaths.GetCardPortrait(nameof(XingXiuQiPan));
    public override string BetaPortraitPath => GuZhenRenArtPaths.GetCardBetaPortrait(nameof(XingXiuQiPan));

    public OptionTengNuoXingXiuQiPan() : base(-2, CardType.Skill, CardRarity.Token, TargetType.None)
    {
        SetRank(0);
    }

    protected override Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay) => Task.CompletedTask;
}
