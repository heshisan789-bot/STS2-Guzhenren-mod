using BaseLib.Abstracts;
using Godot;

namespace Guzhenren.Scripts;

/// <summary>
/// 塔二对外暴露的方源总卡池。
/// 内部真实分层由 <see cref="GuZhenRenCardCatalog"/> 维护：同一道的卡归到同一个逻辑子池。
/// </summary>
public sealed class GuZhenRenCardPool : CustomCardPoolModel
{
    public override string Title => "guzhenren";
    public override string EnergyColorName => "guzhenren";
    public override string? TextEnergyIconPath => GuZhenRenArtPaths.GetCardEnergyOrb(useBigOrb: false);
    public override string? BigEnergyIconPath => GuZhenRenArtPaths.GetCardEnergyOrb(useBigOrb: false);
    public override Texture2D? CustomFrame(CustomCardModel card) => null;

    public override Color DeckEntryCardColor => new(0.88f, 0.88f, 0.90f);
    public override bool IsColorless => false;

    public static IReadOnlyDictionary<GuZhenRenDao, GuZhenRenLogicalCardPool> DaoPools => GuZhenRenCardCatalog.DaoPools;

    public static IReadOnlyList<Type> GetCardTypesForDao(GuZhenRenDao dao)
    {
        return GuZhenRenCardCatalog.GetCardTypes(dao);
    }
}
