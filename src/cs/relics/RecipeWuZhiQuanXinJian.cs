using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;

namespace Guzhenren.Scripts;

[Pool(typeof(GuZhenRenRelicPool))]
public sealed class RecipeWuZhiQuanXinJian : AbstractGuFangRecipeRelic
{
    public override int IngredientCount => 2;

    protected override string RelicImageName => "Recipe_JianDao";

    protected override IReadOnlyList<CardModel> RequiredCardModels => [ModelDb.Card<HuiJian>()];

    protected override CardModel RewardPreviewModel => ModelDb.Card<WuZhiQuanXinJian>();

    public override bool RequiresUpgrade(ModelId cardId)
    {
        return cardId == ModelDb.Card<HuiJian>().Id;
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

        return guCard.PrimaryDao == GuZhenRenDao.JianDao && guCard.Rank >= 1 && guCard.Rank <= 9;
    }

    public override LocString GetIngredientDescription(int index)
    {
        return new LocString("rest_site_ui", $"GUZHENREN-RECIPE_WU_ZHI_QUAN_XIN_JIAN.ingredient{index}");
    }

    public override CardModel CreateRewardCard(Player owner)
    {
        return owner.RunState.CreateCard<WuZhiQuanXinJian>(owner);
    }
}
