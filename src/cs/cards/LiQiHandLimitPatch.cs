using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Guzhenren.Scripts;

[HarmonyPatch]
internal static class LiQiHandLimitPatch
{
    private static int GetEffectiveHandCountForLimit(IReadOnlyList<CardModel> handCards)
    {
        if (handCards.Count == 0)
        {
            return 0;
        }

        var owner = handCards[0].Owner;
        if (owner?.Creature.GetPower<LiQiPower>() == null)
        {
            return handCards.Count;
        }

        return handCards.Count(card => card is not AbstractXuYingCard);
    }

    private static int GetEffectiveHandCountForAdd(IReadOnlyList<CardModel> handCards, CardModel addingCard)
    {
        if (addingCard.Owner?.Creature.GetPower<LiQiPower>() == null)
        {
            return handCards.Count;
        }

        var nonXuYingCount = handCards.Count(card => card is not AbstractXuYingCard);
        if (addingCard is AbstractXuYingCard)
        {
            return nonXuYingCount - 1000;
        }

        return nonXuYingCount;
    }

    private static MethodInfo GetAsyncStateMachineTarget(MethodInfo asyncMethod)
    {
        var attr = asyncMethod.GetCustomAttribute<AsyncStateMachineAttribute>();
        if (attr?.StateMachineType == null)
        {
            throw new MissingMemberException($"Missing async state machine for {asyncMethod.DeclaringType?.FullName}.{asyncMethod.Name}");
        }

        return AccessTools.Method(attr.StateMachineType, "MoveNext")
            ?? throw new MissingMethodException(attr.StateMachineType.FullName, "MoveNext");
    }

    [HarmonyPatch]
    private static class PatchAdd
    {
        private static MethodBase TargetMethod()
        {
            var method = AccessTools.Method(
                typeof(CardPileCmd),
                nameof(CardPileCmd.Add),
                [typeof(IEnumerable<CardModel>), typeof(CardPile), typeof(CardPilePosition), typeof(AbstractModel), typeof(bool)]);

            return GetAsyncStateMachineTarget((MethodInfo)method);
        }

        [HarmonyTranspiler]
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var list = instructions.ToList();

            object? cardLocal = null;
            for (var i = 1; i < list.Count; i++)
            {
                var prev = list[i - 1];
                var cur = list[i];
                if (IsStloc(cur) && prev.opcode == OpCodes.Ldfld && prev.operand is FieldInfo field && field.Name == "cardAdded")
                {
                    cardLocal = cur.operand;
                    break;
                }
            }

            if (cardLocal == null)
            {
                return list;
            }

            for (var i = 0; i < list.Count - 1; i++)
            {
                var cur = list[i];
                if ((cur.opcode == OpCodes.Callvirt || cur.opcode == OpCodes.Call)
                    && cur.operand is MethodInfo method
                    && method.Name == "get_Count"
                    && TryGetLoadedInt32(list[i + 1], out var v)
                    && v == 10)
                {
                    list.Insert(i, new CodeInstruction(OpCodes.Ldloc_S, cardLocal));
                    list[i + 1] = new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(LiQiHandLimitPatch), nameof(GetEffectiveHandCountForAdd)));
                    break;
                }
            }

            return list;
        }
    }

    [HarmonyPatch]
    private static class PatchDraw
    {
        private static MethodBase TargetMethod()
        {
            var method = AccessTools.Method(
                typeof(CardPileCmd),
                nameof(CardPileCmd.Draw),
                [typeof(PlayerChoiceContext), typeof(decimal), typeof(MegaCrit.Sts2.Core.Entities.Players.Player), typeof(bool)]);

            return GetAsyncStateMachineTarget((MethodInfo)method);
        }

        [HarmonyTranspiler]
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var list = instructions.ToList();
            for (var i = 1; i < list.Count; i++)
            {
                var prev = list[i - 1];
                var cur = list[i];
                if (cur.opcode == OpCodes.Callvirt && cur.operand is MethodInfo method && method.Name == "get_Count"
                    && prev.opcode == OpCodes.Callvirt && prev.operand is MethodInfo prevMethod && prevMethod.Name == "get_Cards")
                {
                    list[i] = new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(LiQiHandLimitPatch), nameof(GetEffectiveHandCountForLimit)));
                }
            }

            return list;
        }
    }

    [HarmonyPatch(typeof(CardPileCmd), "CheckIfDrawIsPossibleAndShowThoughtBubbleIfNot")]
    private static class PatchCanDraw
    {
        [HarmonyTranspiler]
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var list = instructions.ToList();
            for (var i = 1; i < list.Count; i++)
            {
                var prev = list[i - 1];
                var cur = list[i];
                if (cur.opcode == OpCodes.Callvirt && cur.operand is MethodInfo method && method.Name == "get_Count"
                    && prev.opcode == OpCodes.Callvirt && prev.operand is MethodInfo prevMethod && prevMethod.Name == "get_Cards")
                {
                    list[i] = new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(LiQiHandLimitPatch), nameof(GetEffectiveHandCountForLimit)));
                }
            }

            return list;
        }
    }

    private static bool TryGetLoadedInt32(CodeInstruction instruction, out int value)
    {
        if (instruction.opcode == OpCodes.Ldc_I4_M1) { value = -1; return true; }
        if (instruction.opcode == OpCodes.Ldc_I4_0) { value = 0; return true; }
        if (instruction.opcode == OpCodes.Ldc_I4_1) { value = 1; return true; }
        if (instruction.opcode == OpCodes.Ldc_I4_2) { value = 2; return true; }
        if (instruction.opcode == OpCodes.Ldc_I4_3) { value = 3; return true; }
        if (instruction.opcode == OpCodes.Ldc_I4_4) { value = 4; return true; }
        if (instruction.opcode == OpCodes.Ldc_I4_5) { value = 5; return true; }
        if (instruction.opcode == OpCodes.Ldc_I4_6) { value = 6; return true; }
        if (instruction.opcode == OpCodes.Ldc_I4_7) { value = 7; return true; }
        if (instruction.opcode == OpCodes.Ldc_I4_8) { value = 8; return true; }
        if (instruction.opcode == OpCodes.Ldc_I4_S) { value = (sbyte)instruction.operand; return true; }
        if (instruction.opcode == OpCodes.Ldc_I4) { value = (int)instruction.operand; return true; }

        value = 0;
        return false;
    }

    private static bool IsStloc(CodeInstruction instruction)
    {
        return instruction.opcode == OpCodes.Stloc
               || instruction.opcode == OpCodes.Stloc_S
               || instruction.opcode == OpCodes.Stloc_0
               || instruction.opcode == OpCodes.Stloc_1
               || instruction.opcode == OpCodes.Stloc_2
               || instruction.opcode == OpCodes.Stloc_3;
    }
}
