using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;

namespace Guzhenren.Scripts;

[Pool(typeof(GuZhenRenRelicPool))]
public sealed class RecipeTianPuGuangHe : AbstractGuFangRecipeRelic
{
    public override int IngredientCount => 2;

    protected override string RelicImageName => "Recipe_GuangDao";

    protected override IReadOnlyList<CardModel> RequiredCardModels => [ModelDb.Card<TaiGuangGu>()];

    protected override CardModel RewardPreviewModel => ModelDb.Card<TianPuGuangHe>();

    public override bool RequiresUpgrade(ModelId cardId)
    {
        return cardId == ModelDb.Card<TaiGuangGu>().Id;
    }

    public override bool IsGenericIngredient(int index, CardModel card)
    {
        if (index != 1)
        {
            return false;
        }

        if (card is not AbstractGuZhenRenCard guCard)
        {
            return false;
        }

        return guCard.PrimaryDao == GuZhenRenDao.GuangDao && guCard.Rank is >= 1 and <= 9;
    }

    public override LocString GetIngredientDescription(int index)
    {
        return new LocString("guzhenren_ui", $"GUZHENREN-RECIPE_TIAN_PU_GUANG_HE.ingredient{index}");
    }

    public override CardModel CreateRewardCard(Player owner)
    {
        return owner.RunState.CreateCard<TianPuGuangHe>(owner);
    }
}
