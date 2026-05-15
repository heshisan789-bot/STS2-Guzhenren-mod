using Godot;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;

namespace Guzhenren.Scripts;

internal static class GuZhenRenArtPaths
{
    private const string Root = "res://guzhenren/images";
    private const string FallbackIcon = "res://guzhenren/images/icon.svg";

    private static readonly IReadOnlyDictionary<string, string> PowerNameOverrides = new Dictionary<string, string>
    {
        ["RiGuangPower"] = "RiGuangpower"
    };

    public static string GetCardPortrait(string cardName)
    {
        var portraitPath = $"{Root}/cards/{cardName}.png";
        return ResourceLoader.Exists(portraitPath) ? portraitPath : CardModel.MissingPortraitPath;
    }

    public static string GetCardBetaPortrait(string cardName)
    {
        var betaPath = $"{Root}/cards/{cardName}_p.png";
        return ResourceLoader.Exists(betaPath) ? betaPath : GetCardPortrait(cardName);
    }

    public static string? GetPowerPackedIcon(string powerName)
    {
        var baseName = GetPowerBaseName(powerName);
        var packedPath = $"{Root}/powers/{baseName}.png";
        return ResourceLoader.Exists(packedPath) ? packedPath : null;
    }

    public static string? GetPowerBigIcon(string powerName)
    {
        var baseName = GetPowerBaseName(powerName);
        var bigPath = $"{Root}/powers/{baseName}_p.png";
        return ResourceLoader.Exists(bigPath) ? bigPath : GetPowerPackedIcon(powerName);
    }

    public static string GetPotionImage(string potionName)
    {
        return GetExistingOrFallback($"{Root}/potions/{potionName}.png", FallbackIcon);
    }

    public static string GetPotionOutline(string potionName)
    {
        var outlinePath = $"{Root}/potions/{potionName}_outline.png";
        return ResourceLoader.Exists(outlinePath) ? outlinePath : GetPotionImage(potionName);
    }

    public static string GetRelicIcon(string relicImageName)
    {
        return GetExistingOrFallback($"{Root}/relics/{relicImageName}.png", FallbackIcon);
    }

    public static string GetRelicOutline(string relicImageName)
    {
        var outlinePath = $"{Root}/relics/outline/{relicImageName}.png";
        return ResourceLoader.Exists(outlinePath) ? outlinePath : GetRelicIcon(relicImageName);
    }

    public static string? GetCardFrame(CardType cardType)
    {
        var fileName = cardType switch
        {
            CardType.Attack => "bg_attack_grey.png",
            CardType.Power => "bg_power_grey.png",
            _ => "bg_skill_grey.png"
        };

        var framePath = $"{Root}/cardui/{fileName}";
        return ResourceLoader.Exists(framePath) ? framePath : null;
    }

    public static string? GetCardEnergyOrb(bool useBigOrb)
    {
        var orbPath = $"{Root}/cardui/{(useBigOrb ? "card_grey_orb.png" : "card_grey_small_orb.png")}";
        return ResourceLoader.Exists(orbPath) ? orbPath : null;
    }

    public static string? GetCharacterOrbLayer(string fileName)
    {
        var layerPath = $"{Root}/ui/orb/{fileName}";
        return ResourceLoader.Exists(layerPath) ? layerPath : null;
    }

    public static string? GetSceneImage(string imageName)
    {
        var scenePath = $"{Root}/scenes/{imageName}";
        return ResourceLoader.Exists(scenePath) ? scenePath : null;
    }

    private static string GetPowerBaseName(string powerName)
    {
        return PowerNameOverrides.TryGetValue(powerName, out var overrideName) ? overrideName : powerName;
    }

    private static string GetExistingOrFallback(string path, string fallbackPath)
    {
        return ResourceLoader.Exists(path) ? path : fallbackPath;
    }
}
