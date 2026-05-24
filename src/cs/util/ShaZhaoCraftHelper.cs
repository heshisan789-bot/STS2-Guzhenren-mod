using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Guzhenren.Scripts;

public static class ShaZhaoCraftHelper
{
    public static IReadOnlyList<AbstractGuFangRecipeRelic> GetCraftableRecipes(Player player)
    {
        return player.Relics
            .OfType<AbstractGuFangRecipeRelic>()
            .Where(r => !r.UsedUp && CanCraft(player, r))
            .ToList();
    }

    public static bool CanCraft(Player player, AbstractGuFangRecipeRelic recipe)
    {
        if (recipe.UsedUp)
        {
            return false;
        }

        foreach (var requiredRelicId in recipe.RequiredRelicIds)
        {
            if (!player.Relics.Any(r => r.Id == requiredRelicId && !r.IsUsedUp))
            {
                return false;
            }
        }

        var remaining = player.Deck.Cards.ToList();
        var fixedIngredients = recipe.GetFixedIngredientCards();
        for (var i = 0; i < recipe.IngredientCount; i++)
        {
            CardModel? found = null;

            if (i < fixedIngredients.Count)
            {
                var required = fixedIngredients[i];
                found = remaining.FirstOrDefault(c =>
                    c.Id == required.Id &&
                    (!recipe.RequiresUpgrade(required.Id) || c.IsUpgraded));
            }
            else
            {
                found = remaining.FirstOrDefault(c => recipe.IsGenericIngredient(i, c));
            }

            if (found == null)
            {
                found = remaining.FirstOrDefault(IsUniversalIngredient);
            }

            if (found == null)
            {
                return false;
            }

            remaining.Remove(found);
        }

        return true;
    }

    private static bool IsUniversalIngredient(CardModel card)
    {
        if (!card.IsUpgraded)
        {
            return false;
        }

        return card.Id.Entry.EndsWith("FANG_WEI_GU", StringComparison.OrdinalIgnoreCase);
    }
}
