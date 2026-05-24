using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;

namespace Guzhenren.Scripts;

[Pool(typeof(GuZhenRenRelicPool))]
public sealed class RecipeSanShiSanTianGuang : AbstractGuFangRecipeRelic
{
    public override int IngredientCount => 2;

    protected override string RelicImageName => "Recipe_GuangDao";

    protected override IReadOnlyList<CardModel> RequiredCardModels => [ModelDb.Card<JiangHeRiXiaGu>()];

    protected override CardModel RewardPreviewModel => ModelDb.Card<SanShiSanTianGuang>();

    public override bool RequiresUpgrade(ModelId cardId)
    {
        return cardId == ModelDb.Card<JiangHeRiXiaGu>().Id;
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
        return new LocString("guzhenren_ui", $"GUZHENREN-RECIPE_SAN_SHI_SAN_TIAN_GUANG.ingredient{index}");
    }

    public override CardModel CreateRewardCard(Player owner)
    {
        return owner.RunState.CreateCard<SanShiSanTianGuang>(owner);
    }
}
