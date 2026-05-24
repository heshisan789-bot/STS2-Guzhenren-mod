using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;

namespace Guzhenren.Scripts;

[Pool(typeof(GuZhenRenRelicPool))]
public sealed class RecipeXuePiaoLiu : AbstractGuFangRecipeRelic
{
    public override int IngredientCount => 2;

    protected override string RelicImageName => "Recipe_XueDao";

    protected override IReadOnlyList<CardModel> RequiredCardModels => [ModelDb.Card<XueZouGu>()];

    protected override CardModel RewardPreviewModel => ModelDb.Card<XuePiaoLiu>();

    public override bool RequiresUpgrade(ModelId cardId)
    {
        return cardId == ModelDb.Card<XueZouGu>().Id;
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

        return guCard.PrimaryDao == GuZhenRenDao.XueDao && guCard.Rank >= 6;
    }

    public override LocString GetIngredientDescription(int index)
    {
        return new LocString("guzhenren_ui", $"GUZHENREN-RECIPE_XUE_PIAO_LIU.ingredient{index}");
    }

    public override CardModel CreateRewardCard(Player owner)
    {
        return owner.RunState.CreateCard<XuePiaoLiu>(owner);
    }
}
