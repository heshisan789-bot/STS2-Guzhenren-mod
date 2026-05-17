using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;

namespace Guzhenren.Scripts;

internal static class XuYingHandLimitPatch
{
    private const int BaseHandLimit = 10;

    public static int GetHandLimit(Player player)
    {
        if (!player.Character.Id.Entry.StartsWith("GUZHENREN-", StringComparison.OrdinalIgnoreCase))
        {
            return BaseHandLimit;
        }

        var hand = player.PlayerCombatState?.Hand;
        if (hand == null)
        {
            return BaseHandLimit;
        }

        var xuYingCount = hand.Cards.Count(c => c is AbstractXuYingCard);
        return BaseHandLimit + xuYingCount;
    }

    [HarmonyPatch(typeof(CardPileCmd), nameof(CardPileCmd.Draw), typeof(PlayerChoiceContext), typeof(decimal), typeof(Player), typeof(bool))]
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> DrawTranspiler(IEnumerable<CodeInstruction> instructions)
    {
        var getHandLimit = AccessTools.Method(typeof(XuYingHandLimitPatch), nameof(GetHandLimit));
        foreach (var ci in instructions)
        {
            if (ci.opcode == OpCodes.Ldc_I4_S && ci.operand is sbyte sb && sb == 10)
            {
                yield return new CodeInstruction(OpCodes.Ldarg_2);
                yield return new CodeInstruction(OpCodes.Call, getHandLimit);
                continue;
            }

            if (ci.opcode == OpCodes.Ldc_I4 && ci.operand is int i && i == 10)
            {
                yield return new CodeInstruction(OpCodes.Ldarg_2);
                yield return new CodeInstruction(OpCodes.Call, getHandLimit);
                continue;
            }

            yield return ci;
        }
    }

    [HarmonyPatch(typeof(CardPileCmd), "CheckIfDrawIsPossibleAndShowThoughtBubbleIfNot")]
    [HarmonyPrefix]
    private static bool CheckIfDrawIsPossibleAndShowThoughtBubbleIfNotPrefix(Player player, ref bool __result)
    {
        if (PileType.Draw.GetPile(player).Cards.Count + PileType.Discard.GetPile(player).Cards.Count == 0)
        {
            ThinkCmd.Play(new LocString("combat_messages", "NO_DRAW"), player.Creature, 2.0);
            __result = false;
            return false;
        }

        var handLimit = GetHandLimit(player);
        if (PileType.Hand.GetPile(player).Cards.Count >= handLimit)
        {
            ThinkCmd.Play(new LocString("combat_messages", "HAND_FULL"), player.Creature, 2.0);
            __result = false;
            return false;
        }

        __result = true;
        return false;
    }

    [HarmonyPatch(typeof(CardPileCmd), nameof(CardPileCmd.Add), typeof(IEnumerable<CardModel>), typeof(CardPile), typeof(CardPilePosition), typeof(AbstractModel), typeof(bool))]
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> AddTranspiler(IEnumerable<CodeInstruction> instructions)
    {
        var list = instructions.ToList();
        var owningPlayerLocal = FindOwningPlayerLocal(list);
        if (owningPlayerLocal == null)
        {
            return list;
        }

        var getHandLimit = AccessTools.Method(typeof(XuYingHandLimitPatch), nameof(GetHandLimit));
        for (var i = 0; i < list.Count; i++)
        {
            var ci = list[i];
            if (ci.opcode == OpCodes.Ldc_I4_S && ci.operand is sbyte sb && sb == 10)
            {
                list[i] = new CodeInstruction(OpCodes.Ldloc, owningPlayerLocal);
                list.Insert(i + 1, new CodeInstruction(OpCodes.Call, getHandLimit));
                break;
            }
        }

        return list;
    }

    private static object? FindOwningPlayerLocal(IReadOnlyList<CodeInstruction> instructions)
    {
        var isMe = AccessTools.Method(typeof(LocalContext), nameof(LocalContext.IsMe), [typeof(Player)]);
        for (var i = 0; i < instructions.Count; i++)
        {
            var ci = instructions[i];
            if (ci.opcode == OpCodes.Call && ci.operand is MethodInfo mi && mi == isMe)
            {
                var prev = i > 0 ? instructions[i - 1] : null;
                if (prev != null && prev.IsLdloc())
                {
                    return prev.operand;
                }
            }
        }

        return null;
    }
}
