using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;

namespace Guzhenren.Scripts;

[Pool(typeof(GuZhenRenRelicPool))]
public sealed class RecipeWanWuDaTongBian : AbstractGuFangRecipeRelic
{
    public override int IngredientCount => 2;
    protected override string RelicImageName => "Recipe_BianHuaDao";
    protected override IReadOnlyList<CardModel> RequiredCardModels => [ModelDb.Card<BianTong>()];
    protected override CardModel RewardPreviewModel => ModelDb.Card<WanWuDaTongBian>();

    public override bool RequiresUpgrade(ModelId cardId)
        => cardId == ModelDb.Card<BianTong>().Id;

    public override bool IsGenericIngredient(int index, CardModel card)
    {
        if (index != 1)
            return false;

        if (card is not AbstractGuZhenRenCard guCard)
            return false;

        return (guCard.PrimaryDao == GuZhenRenDao.BianHuaDao || guCard.PrimaryDao == GuZhenRenDao.LiDao)
            && guCard.Rank is >= 1 and <= 9;
    }

    public override LocString GetIngredientDescription(int index)
        => new("rest_site_ui", $"GUZHENREN-RECIPE_WAN_WU_DA_TONG_BIAN.ingredient{index}");

    public override CardModel CreateRewardCard(Player owner)
        => owner.RunState.CreateCard<WanWuDaTongBian>(owner);
}
