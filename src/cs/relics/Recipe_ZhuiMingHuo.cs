using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;

namespace Guzhenren.Scripts;

[Pool(typeof(GuZhenRenRelicPool))]
public sealed class Recipe_ZhuiMingHuo : AbstractGuFangRecipeRelic
{
    public override int IngredientCount => 2;

    protected override string RelicImageName => "Recipe_YanDao";

    protected override IReadOnlyList<CardModel> RequiredCardModels => [ModelDb.Card<LiaoYuanHuo>()];

    protected override CardModel RewardPreviewModel => ModelDb.Card<ZhuiMingHuo>();

    public override bool RequiresUpgrade(ModelId cardId)
    {
        return cardId == ModelDb.Card<LiaoYuanHuo>().Id;
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

        return guCard.PrimaryDao == GuZhenRenDao.YanDao && guCard.Rank is >= 6 and <= 9;
    }

    public override LocString GetIngredientDescription(int index)
    {
        return new LocString("rest_site_ui", $"GUZHENREN-RECIPE_ZHUI_MING_HUO.ingredient{index}");
    }

    public override CardModel CreateRewardCard(Player owner)
    {
        return owner.RunState.CreateCard<ZhuiMingHuo>(owner);
    }
}
