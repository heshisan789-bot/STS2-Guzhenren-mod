using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Models;

namespace Guzhenren.Scripts;

[HarmonyPatch]
internal static class FangYuanCharacterIconPatch
{
    private const string IconTexturePath = "res://guzhenren/images/character/FangYuan/Button.png";
    private const string IconScenePath = "res://guzhenren/scenes/fang_yuan_icon.tscn";

    [HarmonyPatch(typeof(CharacterModel), "get_IconTexture")]
    [HarmonyPostfix]
    private static void IconTexturePostfix(CharacterModel __instance, ref Texture2D __result)
    {
        if (__instance is not FangYuanCharacter)
        {
            return;
        }

        __result = ResourceLoader.Load<Texture2D>(IconTexturePath, null, ResourceLoader.CacheMode.Reuse);
    }

    [HarmonyPatch(typeof(CharacterModel), "get_IconOutlineTexture")]
    [HarmonyPostfix]
    private static void IconOutlineTexturePostfix(CharacterModel __instance, ref Texture2D __result)
    {
        if (__instance is not FangYuanCharacter)
        {
            return;
        }

        __result = ResourceLoader.Load<Texture2D>(IconTexturePath, null, ResourceLoader.CacheMode.Reuse);
    }

    [HarmonyPatch(typeof(CharacterModel), "get_Icon")]
    [HarmonyPostfix]
    private static void IconPostfix(CharacterModel __instance, ref Control __result)
    {
        if (__instance is not FangYuanCharacter)
        {
            return;
        }

        var scene = ResourceLoader.Load<PackedScene>(IconScenePath, null, ResourceLoader.CacheMode.Reuse);
        __result = scene.Instantiate<Control>(PackedScene.GenEditState.Disabled);
    }
}
