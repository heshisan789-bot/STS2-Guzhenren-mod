using System.Collections.Generic;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Guzhenren.Scripts;

[HarmonyPatch]
internal static class EnchantmentPreviewDynamicVarPatch
{
    [HarmonyPatch(typeof(BlockVar), nameof(BlockVar.UpdateCardPreview))]
    [HarmonyPrefix]
    private static bool BlockVarUpdateCardPreviewPrefix(BlockVar __instance, CardModel card, CardPreviewMode previewMode, Creature? target, bool runGlobalHooks)
    {
        var num = __instance.BaseValue;
        var enchantment = card.Enchantment;
        if (enchantment != null)
        {
            num += enchantment.EnchantBlockAdditive(num, __instance.Props);
            num *= enchantment.EnchantBlockMultiplicative(num, __instance.Props);
            if (!card.IsEnchantmentPreview)
            {
                __instance.EnchantedValue = num;
            }
        }

        if (runGlobalHooks)
        {
            num = Hook.ModifyBlock(card.CombatState!, card.Owner.Creature, num, __instance.Props, card, null,
                out IEnumerable<AbstractModel> _);
        }

        __instance.PreviewValue = num;
        return false;
    }

    [HarmonyPatch(typeof(DamageVar), nameof(DamageVar.UpdateCardPreview))]
    [HarmonyPrefix]
    private static bool DamageVarUpdateCardPreviewPrefix(DamageVar __instance, CardModel card, CardPreviewMode previewMode, Creature? target, bool runGlobalHooks)
    {
        var num = __instance.BaseValue;
        var enchantment = card.Enchantment;
        if (enchantment != null)
        {
            num += enchantment.EnchantDamageAdditive(num, __instance.Props);
            num *= enchantment.EnchantDamageMultiplicative(num, __instance.Props);
            if (!card.IsEnchantmentPreview)
            {
                __instance.EnchantedValue = num;
            }
        }

        if (runGlobalHooks)
        {
            num = Hook.ModifyDamage(card.Owner.RunState, card.CombatState!, target, card.Owner.Creature, num, __instance.Props, card,
                ModifyDamageHookType.All, previewMode, out IEnumerable<AbstractModel> _);
        }

        __instance.PreviewValue = num;
        return false;
    }

    [HarmonyPatch(typeof(CalculatedBlockVar), nameof(CalculatedBlockVar.UpdateCardPreview))]
    [HarmonyPrefix]
    private static bool CalculatedBlockVarUpdateCardPreviewPrefix(CalculatedBlockVar __instance, CardModel card, CardPreviewMode previewMode, Creature? target, bool runGlobalHooks)
    {
        var num = __instance.Calculate(target);
        var enchantment = card.Enchantment;
        if (enchantment != null)
        {
            num += enchantment.EnchantBlockAdditive(num, __instance.Props);
            num *= enchantment.EnchantBlockMultiplicative(num, __instance.Props);
            if (!card.IsEnchantmentPreview)
            {
                __instance.EnchantedValue = num;
            }
        }

        if (runGlobalHooks)
        {
            var combatState = card.CombatState ?? card.Owner.Creature.CombatState;
            if (combatState != null)
            {
                num = Hook.ModifyBlock(combatState, card.Owner.Creature, num, __instance.Props, card, null, out IEnumerable<AbstractModel> _);
            }
        }

        __instance.PreviewValue = num;
        return false;
    }

    [HarmonyPatch(typeof(CalculatedDamageVar), nameof(CalculatedDamageVar.UpdateCardPreview))]
    [HarmonyPrefix]
    private static bool CalculatedDamageVarUpdateCardPreviewPrefix(CalculatedDamageVar __instance, CardModel card, CardPreviewMode previewMode, Creature? target, bool runGlobalHooks)
    {
        var num = __instance.Calculate(target);
        var enchantment = card.Enchantment;
        if (enchantment != null)
        {
            num += enchantment.EnchantDamageAdditive(num, __instance.Props);
            num *= enchantment.EnchantDamageMultiplicative(num, __instance.Props);
            num = num < 0m ? 0m : num;
            if (!card.IsEnchantmentPreview)
            {
                __instance.EnchantedValue = num;
            }
        }

        if (runGlobalHooks)
        {
            var combatState = card.CombatState ?? card.Owner.Creature.CombatState;
            if (combatState != null)
            {
                num = Hook.ModifyDamage(card.Owner.RunState, combatState, target, __instance.IsFromOsty ? card.Owner.Osty : card.Owner.Creature, num,
                    __instance.Props, card, ModifyDamageHookType.All, previewMode, out IEnumerable<AbstractModel> _);
            }
        }

        __instance.PreviewValue = num < 0m ? 0m : num;
        return false;
    }
}
