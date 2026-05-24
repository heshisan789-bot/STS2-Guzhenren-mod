using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.RestSite;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;

namespace Guzhenren.Scripts;

public sealed class ShaZhaoCraftRestSiteOption : RestSiteOption
{
    public override string OptionId => "GUZHENREN_SHAZHAO";

    public ShaZhaoCraftRestSiteOption(Player owner) : base(owner)
    {
        IsEnabled = ShaZhaoCraftHelper.GetCraftableRecipes(owner).Count > 0;
    }

    public override async Task<bool> OnSelect()
    {
        if (!IsEnabled)
        {
            return false;
        }

        var recipes = ShaZhaoCraftHelper.GetCraftableRecipes(Owner);
        if (recipes.Count == 0)
        {
            return false;
        }

        var tempCards = new List<CardModel>();
        var map = new Dictionary<CardModel, AbstractGuFangRecipeRelic>();
        foreach (var recipe in recipes)
        {
            var reward = recipe.CreateRewardCard(Owner);
            tempCards.Add(reward);
            map.Add(reward, recipe);
        }

        var context = new HookPlayerChoiceContext(Owner, LocalContext.NetId ?? Owner.NetId, GameActionType.NonCombat);
        var pickRecipePrefs = new CardSelectorPrefs(new LocString("guzhenren_ui", "GUZHENREN-SHAZHAO_CRAFT.selectRecipePrompt"), 1)
        {
            Cancelable = true,
            RequireManualConfirmation = true
        };

        var chosen = (await CardSelectCmd.FromSimpleGrid(context, tempCards, Owner, pickRecipePrefs)).FirstOrDefault();
        if (chosen == null)
        {
            CleanupTempCards(tempCards);
            return false;
        }

        var selectedRecipe = map[chosen];
        var fixedIngredients = selectedRecipe.GetFixedIngredientCards();
        var selectedIngredients = new List<CardModel>();

        for (var i = 0; i < selectedRecipe.IngredientCount; i++)
        {
            var prefs = new CardSelectorPrefs(selectedRecipe.GetIngredientDescription(i), 1)
            {
                Cancelable = true,
                RequireManualConfirmation = true
            };

            var selection = await CardSelectCmd.FromDeckGeneric(Owner, prefs, c => MatchesIngredient(selectedRecipe, fixedIngredients, i, c, selectedIngredients));
            var picked = selection.FirstOrDefault();
            if (picked == null)
            {
                CleanupTempCards(tempCards);
                return false;
            }

            selectedIngredients.Add(picked);
        }

        await CardPileCmd.RemoveFromDeck(selectedIngredients);

        var finalReward = selectedRecipe.CreateRewardCard(Owner);
        await CardPileCmd.Add(finalReward, PileType.Deck, source: selectedRecipe);

        selectedRecipe.UsedUp = true;
        selectedRecipe.Flash();

        CleanupTempCards(tempCards);
        return true;
    }

    private static bool MatchesIngredient(
        AbstractGuFangRecipeRelic recipe,
        IReadOnlyList<CardModel> fixedIngredients,
        int index,
        CardModel card,
        List<CardModel> alreadyChosen)
    {
        if (alreadyChosen.Contains(card))
        {
            return false;
        }

        if (index < fixedIngredients.Count)
        {
            var required = fixedIngredients[index];
            if (card.Id != required.Id)
            {
                return false;
            }

            if (recipe.RequiresUpgrade(required.Id) && !card.IsUpgraded)
            {
                return false;
            }

            return true;
        }

        if (recipe.IsGenericIngredient(index, card))
        {
            return true;
        }

        return card.IsUpgraded && card.Id.Entry.EndsWith("FANG_WEI_GU", StringComparison.OrdinalIgnoreCase);
    }

    private void CleanupTempCards(IEnumerable<CardModel> tempCards)
    {
        foreach (var card in tempCards)
        {
            Owner.RunState.RemoveCard(card);
        }
    }
}
