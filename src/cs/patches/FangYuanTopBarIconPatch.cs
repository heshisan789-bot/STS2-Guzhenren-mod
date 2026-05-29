using System;
using Godot;
using HarmonyLib;
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
               || string.Equals(entry, FangYuanCharacterIdLower, StringComparison.OrdinalIgnoreCase)
               || (entry.Contains("guzhenren", StringComparison.OrdinalIgnoreCase)
                   && entry.Contains("fang_yuan", StringComparison.OrdinalIgnoreCase));
    }

    [HarmonyPatch(typeof(CharacterModel), "get_Icon")]
    [HarmonyPrefix]
    private static bool GetIconPrefix(CharacterModel __instance, ref Control __result)
    {
        if (!IsFangYuan(__instance))
        {
            return true;
        }

        var scene = ResourceLoader.Load<PackedScene>(FangYuanIconScenePath, null, ResourceLoader.CacheMode.Reuse);
        if (scene == null)
        {
            return true;
        }

        __result = scene.Instantiate<Control>(PackedScene.GenEditState.Disabled);
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

        var texture = ResourceLoader.Load<Texture2D>(FangYuanIconTexturePath, null, ResourceLoader.CacheMode.Reuse);
        if (texture == null)
        {
            return true;
        }

        __result = texture;
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

        var texture = ResourceLoader.Load<Texture2D>(FangYuanIconTexturePath, null, ResourceLoader.CacheMode.Reuse);
        if (texture == null)
        {
            return true;
        }

        __result = texture;
        return false;
    }
}
