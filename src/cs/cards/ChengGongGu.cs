using BaseLib.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Guzhenren.Scripts;

[Pool(typeof(GuZhenRenCardPool))]
[GuZhenRenDaoPool(GuZhenRenDao.LuDao)]
public sealed class ChengGongGu : AbstractGuZhenRenCard
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
    public override int MaxUpgradeLevel => 0;
    public override GuZhenRenBannedCardSources BannedSources => GuZhenRenBannedCardSources.All;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromCard<OptionCaiFu>(),
        HoverTipFactory.FromCard<OptionYongSheng>(),
        HoverTipFactory.FromCard<OptionZiYou>()
    ];

    public ChengGongGu() : base(0, CardType.Skill, CardRarity.Rare, TargetType.None)
    {
        SetDao(GuZhenRenDao.LuDao);
        SetRank(9);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(CombatState, nameof(CombatState));

        var choices = new List<CardModel>
        {
            CombatState.CreateCard<OptionCaiFu>(Owner),
            CombatState.CreateCard<OptionYongSheng>(Owner),
            CombatState.CreateCard<OptionZiYou>(Owner)
        };

        var selected = (await CardSelectCmd.FromSimpleGrid(choiceContext, choices, Owner,
            new CardSelectorPrefs(SelectionScreenPrompt, 1))).FirstOrDefault();

        switch (selected)
        {
            case OptionCaiFu:
                await PlayerCmd.GainGold(300m, Owner);
                break;
            case OptionYongSheng:
                await CreatureCmd.GainMaxHp(Owner.Creature, 16m);
                break;
            case OptionZiYou:
            {
                var toRemove = (await CardSelectCmd.FromDeckForRemoval(Owner,
                    new CardSelectorPrefs(CardSelectorPrefs.RemoveSelectionPrompt, 2))).ToList();
                if (toRemove.Count > 0)
                    await CardPileCmd.RemoveFromDeck(toRemove);
                break;
            }
        }
    }

}

[Pool(typeof(GuZhenRenCardPool))]
public sealed class OptionCaiFu : AbstractGuZhenRenCard
{
    public override GuZhenRenBannedCardSources BannedSources => GuZhenRenBannedCardSources.All;
    public override string PortraitPath => GuZhenRenArtPaths.GetCardPortrait(nameof(ChengGongGu));
    public override string BetaPortraitPath => GuZhenRenArtPaths.GetCardBetaPortrait(nameof(ChengGongGu));

    public OptionCaiFu() : base(-2, CardType.Skill, CardRarity.Token, TargetType.None)
    {
        SetRank(0);
    }

    protected override Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay) => Task.CompletedTask;
}

[Pool(typeof(GuZhenRenCardPool))]
public sealed class OptionYongSheng : AbstractGuZhenRenCard
{
    public override GuZhenRenBannedCardSources BannedSources => GuZhenRenBannedCardSources.All;
    public override string PortraitPath => GuZhenRenArtPaths.GetCardPortrait(nameof(ChengGongGu));
    public override string BetaPortraitPath => GuZhenRenArtPaths.GetCardBetaPortrait(nameof(ChengGongGu));

    public OptionYongSheng() : base(-2, CardType.Skill, CardRarity.Token, TargetType.None)
    {
        SetRank(0);
    }

    protected override Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay) => Task.CompletedTask;
}

[Pool(typeof(GuZhenRenCardPool))]
public sealed class OptionZiYou : AbstractGuZhenRenCard
{
    public override GuZhenRenBannedCardSources BannedSources => GuZhenRenBannedCardSources.All;
    public override string PortraitPath => GuZhenRenArtPaths.GetCardPortrait(nameof(ChengGongGu));
    public override string BetaPortraitPath => GuZhenRenArtPaths.GetCardBetaPortrait(nameof(ChengGongGu));

    public OptionZiYou() : base(-2, CardType.Skill, CardRarity.Token, TargetType.None)
    {
        SetRank(0);
    }

    protected override Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay) => Task.CompletedTask;
}
