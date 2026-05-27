using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;

namespace Guzhenren.Scripts;

[Pool(typeof(GuZhenRenRelicPool))]
public sealed class RecipeAnQiSha : AbstractGuFangRecipeRelic
{
    private static readonly ModelId JianYingGuId = ModelDb.Card<JianYingGu>().Id;
    private static readonly ModelId DuoChongJianYingGuId = ModelDb.Card<DuoChongJianYingGu>().Id;
    private static readonly ModelId DieYingGuId = ModelDb.Card<DieYingGu>().Id;

    public override int IngredientCount => 2;

    protected override string RelicImageName => "Recipe_JianDao";

    protected override IReadOnlyList<CardModel> RequiredCardModels => [ModelDb.Card<FeiJian>()];

    protected override CardModel RewardPreviewModel => ModelDb.Card<AnQiSha>();

    public override bool RequiresUpgrade(ModelId cardId)
    {
        return cardId == ModelDb.Card<FeiJian>().Id;
    }

    public override bool IsGenericIngredient(int index, CardModel card)
    {
        if (index != 1)
        {
            return false;
        }

        return card.Id == JianYingGuId || card.Id == DuoChongJianYingGuId || card.Id == DieYingGuId;
    }

    public override LocString GetIngredientDescription(int index)
    {
        return new LocString("rest_site_ui", $"GUZHENREN-RECIPE_AN_QI_SHA.ingredient{index}");
    }

    public override CardModel CreateRewardCard(Player owner)
    {
        return owner.RunState.CreateCard<AnQiSha>(owner);
    }
}
