using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Saves.Runs;

namespace Guzhenren.Scripts;

public abstract class AbstractGuFangRecipeRelic : CustomRelicModel
{
    private bool _usedUp;

    private static string SafeFormat(LocString locString)
    {
        try
        {
            return locString.GetFormattedText();
        }
        catch
        {
            return locString.LocTable + "." + locString.LocEntryKey;
        }
    }

    private static string ToRequirementText(string ingredientDescription)
    {
        const string prefix = "选择材料：";
        if (ingredientDescription.StartsWith(prefix, StringComparison.Ordinal))
        {
            return ingredientDescription[prefix.Length..].Trim();
        }

        return ingredientDescription.Trim();
    }

    public override RelicRarity Rarity => RelicRarity.Common;

    public override bool IsAllowedInShops => true;

    public override bool IsUsedUp => _usedUp;

    public abstract int IngredientCount { get; }

    protected abstract string RelicImageName { get; }

    protected abstract IReadOnlyList<CardModel> RequiredCardModels { get; }

    public virtual IReadOnlyList<ModelId> RequiredRelicIds => Array.Empty<ModelId>();

    protected abstract CardModel RewardPreviewModel { get; }

    public override string PackedIconPath => GuZhenRenArtPaths.GetRelicIcon(RelicImageName);
    protected override string PackedIconOutlinePath => GuZhenRenArtPaths.GetRelicOutline(RelicImageName);
    protected override string BigIconPath => GuZhenRenArtPaths.GetRelicIcon(RelicImageName);

    [SavedProperty]
    public bool UsedUp
    {
        get => _usedUp;
        set
        {
            AssertMutable();
            _usedUp = value;
            Status = _usedUp ? RelicStatus.Disabled : RelicStatus.Normal;
        }
    }

    public abstract bool RequiresUpgrade(ModelId cardId);

    public abstract bool IsGenericIngredient(int index, CardModel card);

    public abstract LocString GetIngredientDescription(int index);

    public abstract CardModel CreateRewardCard(Player owner);

    public virtual bool CanCraft(Player owner)
    {
        return ShaZhaoCraftHelper.CanCraft(owner, this);
    }

    public override Task AfterRoomEntered(AbstractRoom room)
    {
        if (UsedUp)
        {
            Status = RelicStatus.Disabled;
            return Task.CompletedTask;
        }

        if (room is RestSiteRoom && CanCraft(Owner))
        {
            Status = RelicStatus.Active;
        }
        else
        {
            Status = RelicStatus.Normal;
        }

        return Task.CompletedTask;
    }

    public IReadOnlyList<CardModel> GetFixedIngredientCards() => RequiredCardModels;

    protected override IEnumerable<IHoverTip> ExtraHoverTips
    {
        get
        {
            try
            {
                var ingredientNames = new List<string>(IngredientCount);
                for (var i = 0; i < IngredientCount; i++)
                {
                    ingredientNames.Add(ToRequirementText(SafeFormat(GetIngredientDescription(i))));
                }

                var shazhaoName = RewardPreviewModel.Title;
                var titleLoc = new LocString("rest_site_ui", "GUZHENREN-RECIPE_SHAZHAO_TITLE");
                titleLoc.Add("name", shazhaoName);

                var recipeTip = new HoverTip(titleLoc, "需要蛊虫：" + string.Join("、", ingredientNames));
                recipeTip.SetCanonicalModel(CanonicalInstance);

                CardModel preview;
                try
                {
                    preview = Owner != null ? CreateRewardCard(Owner) : RewardPreviewModel;
                }
                catch
                {
                    preview = RewardPreviewModel;
                }

                return new IHoverTip[]
                {
                    recipeTip,
                    new CardHoverTip(preview)
                };
            }
            catch
            {
                return Array.Empty<IHoverTip>();
            }
        }
    }
}
