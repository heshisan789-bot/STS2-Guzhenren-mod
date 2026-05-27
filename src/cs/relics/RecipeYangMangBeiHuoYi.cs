using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;

namespace Guzhenren.Scripts;

[Pool(typeof(GuZhenRenRelicPool))]
public sealed class RecipeYangMangBeiHuoYi : AbstractGuFangRecipeRelic
{
    public override int IngredientCount => 2;

    protected override string RelicImageName => "Recipe_YanDao";

    protected override IReadOnlyList<CardModel> RequiredCardModels => [ModelDb.Card<YanZhouGu>()];

    protected override CardModel RewardPreviewModel => ModelDb.Card<YangMangBeiHuoYi>();

    public override bool RequiresUpgrade(ModelId cardId)
    {
        return cardId == ModelDb.Card<YanZhouGu>().Id;
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

        return guCard.PrimaryDao == GuZhenRenDao.YanDao && guCard.Rank >= 6;
    }

    public override LocString GetIngredientDescription(int index)
    {
        return new LocString("rest_site_ui", $"GUZHENREN-RECIPE_YANG_MANG_BEI_HUO_YI.ingredient{index}");
    }

    public override CardModel CreateRewardCard(Player owner)
    {
        return owner.RunState.CreateCard<YangMangBeiHuoYi>(owner);
    }
}
