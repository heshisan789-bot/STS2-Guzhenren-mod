using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;

namespace Guzhenren.Scripts;

[Pool(typeof(GuZhenRenRelicPool))]
public sealed class RecipeXingXiuQiPan : AbstractGuFangRecipeRelic
{
    public override int IngredientCount => 2;

    protected override string RelicImageName => "Recipe_ZhiDao";

    protected override IReadOnlyList<CardModel> RequiredCardModels => [ModelDb.Card<ZhiHuiGu>()];

    protected override CardModel RewardPreviewModel => ModelDb.Card<XingXiuQiPan>();

    public override bool RequiresUpgrade(ModelId cardId)
    {
        return false;
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

        return guCard.PrimaryDao == GuZhenRenDao.ZhiDao && guCard.Rank is >= 6 and <= 9;
    }

    public override LocString GetIngredientDescription(int index)
    {
        return new LocString("rest_site_ui", $"GUZHENREN-RECIPE_XING_XIU_QI_PAN.ingredient{index}");
    }

    public override CardModel CreateRewardCard(Player owner)
    {
        return owner.RunState.CreateCard<XingXiuQiPan>(owner);
    }
}

