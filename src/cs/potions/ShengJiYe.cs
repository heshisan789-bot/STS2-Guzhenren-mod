using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Potions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace Guzhenren.Scripts;

[Pool(typeof(GuZhenRenPotionPool))]
public sealed class ShengJiYe : CustomPotionModel
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("Heal", 6m)];

    public override PotionRarity Rarity => PotionRarity.Token;
    public override PotionUsage Usage => PotionUsage.AnyTime;
    public override TargetType TargetType => TargetType.AnyPlayer;
    public override string? CustomPackedImagePath => GuZhenRenArtPaths.GetPotionImage(nameof(ShengJiYe));
    public override string? CustomPackedOutlinePath => GuZhenRenArtPaths.GetPotionOutline(nameof(ShengJiYe));

    protected override async Task OnUse(PlayerChoiceContext choiceContext, Creature? target)
    {
        await CreatureCmd.Heal(Owner.Creature, DynamicVars["Heal"].BaseValue);
    }
}
