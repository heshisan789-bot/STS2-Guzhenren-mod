using BaseLib.Abstracts;
using Godot;

namespace Guzhenren.Scripts;

public sealed class GuZhenRenSpecialCardPool : CustomCardPoolModel
{
    public override string Title => "guzhenren_special";
    public override string EnergyColorName => "guzhenren";
    public override string? TextEnergyIconPath => GuZhenRenArtPaths.GetCardEnergyOrb(useBigOrb: false);
    public override string? BigEnergyIconPath => GuZhenRenArtPaths.GetCardEnergyOrb(useBigOrb: false);
    public override Texture2D? CustomFrame(CustomCardModel card) => null;
    public override Color DeckEntryCardColor => new(0.88f, 0.88f, 0.90f);
    public override bool IsColorless => false;
}
