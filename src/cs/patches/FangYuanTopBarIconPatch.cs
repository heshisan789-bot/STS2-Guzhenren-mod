using System;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Models;

namespace Guzhenren.Scripts;

[HarmonyPatch]
internal static class FangYuanTopBarIconPatch
{
    private const string FangYuanCharacterId = "GUZHENREN-FANG_YUAN_CHARACTER";
    private const string FangYuanCharacterIdLower = "guzhenren-fang_yuan_character";
    private const string FangYuanIconScenePath = "res://guzhenren/scenes/fang_yuan_icon.tscn";
    private const string FangYuanIconTexturePath = "res://guzhenren/images/character/FangYuan/Button.png";

    private static bool IsFangYuan(CharacterModel character)
    {
        var entry = character.Id.Entry;
        return string.Equals(entry, FangYuanCharacterId, StringComparison.OrdinalIgnoreCase)
               || string.Equals(entry, FangYuanCharacterIdLower, StringComparison.OrdinalIgnoreCase);
    }

    [HarmonyPatch(typeof(CharacterModel), "get_Icon")]
    [HarmonyPrefix]
    private static bool GetIconPrefix(CharacterModel __instance, ref Control __result)
    {
        if (!IsFangYuan(__instance))
        {
            return true;
        }

        if (!ResourceLoader.Exists(FangYuanIconScenePath))
        {
            return true;
        }

        __result = PreloadManager.Cache.GetScene(FangYuanIconScenePath).Instantiate<Control>(PackedScene.GenEditState.Disabled);
        return false;
    }

    [HarmonyPatch(typeof(CharacterModel), "get_IconTexture")]
    [HarmonyPrefix]
    private static bool GetIconTexturePrefix(CharacterModel __instance, ref Texture2D __result)
    {
        if (!IsFangYuan(__instance))
        {
            return true;
        }

        if (!ResourceLoader.Exists(FangYuanIconTexturePath))
        {
            return true;
        }

        __result = PreloadManager.Cache.GetTexture2D(FangYuanIconTexturePath);
        return false;
    }

    [HarmonyPatch(typeof(CharacterModel), "get_IconOutlineTexture")]
    [HarmonyPrefix]
    private static bool GetIconOutlineTexturePrefix(CharacterModel __instance, ref Texture2D __result)
    {
        if (!IsFangYuan(__instance))
        {
            return true;
        }

        if (!ResourceLoader.Exists(FangYuanIconTexturePath))
        {
            return true;
        }

        __result = PreloadManager.Cache.GetTexture2D(FangYuanIconTexturePath);
        return false;
    }
}
