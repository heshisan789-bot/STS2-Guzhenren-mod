using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;

namespace Guzhenren.Scripts;

[HarmonyPatch]
internal static class XianGuUniqueAddToDeckResultPatch
{
    private static IEnumerable<MethodBase> TargetMethods()
    {
        return typeof(CardPileCmd).GetMethods(BindingFlags.Public | BindingFlags.Static)
            .Where(m => m.Name == nameof(CardPileCmd.Add))
            .Where(m => m.ReturnType.IsGenericType)
            .Where(m => m.ReturnType.GetGenericTypeDefinition() == typeof(Task<>));
    }

    [HarmonyPostfix]
    private static void Postfix(ref object __result, object[] __args)
    {
        if (__result is not Task task)
        {
            return;
        }

        CardModel? card = null;
        PileType? pileType = null;
        foreach (var arg in __args)
        {
            if (card == null && arg is CardModel c)
            {
                card = c;
                continue;
            }

            if (pileType == null && arg is PileType p)
            {
                pileType = p;
            }
        }

        if (card == null || pileType != PileType.Deck)
        {
            return;
        }

        if (CombatManager.Instance.IsInProgress)
        {
            return;
        }

        if (!IsDuplicateXianGu(card))
        {
            return;
        }

        __result = WrapTask(task, card);
    }

    private static bool IsDuplicateXianGu(CardModel card)
    {
        if (card.Owner == null || card is not AbstractGuZhenRenCard guCard)
        {
            return false;
        }

        if (guCard.IsXuYing)
        {
            return false;
        }

        if (!(guCard.IsXianGu || guCard.Rank >= 6))
        {
            return false;
        }

        return card.Owner.Deck.Cards.Any(c => c.Id == card.Id && !ReferenceEquals(c, card));
    }

    private static object WrapTask(Task task, CardModel card)
    {
        var t = task.GetType();
        var resultType = t.GetGenericArguments()[0];
        var method = typeof(XianGuUniqueAddToDeckResultPatch)
            .GetMethod(nameof(WrapTaskGeneric), BindingFlags.NonPublic | BindingFlags.Static)!
            .MakeGenericMethod(resultType);
        return method.Invoke(null, [task, card])!;
    }

    private static async Task<T> WrapTaskGeneric<T>(Task<T> task, CardModel card)
    {
        var result = await task;
        var boxed = (object)result!;

        if (!TryGetBoolMember(boxed, "success", out var success) || success)
        {
            return result;
        }

        if (!TrySetBoolMember(ref boxed, "success", true))
        {
            return result;
        }

        return (T)boxed;
    }

    private static bool TryGetBoolMember(object instance, string name, out bool value)
    {
        var type = instance.GetType();
        var field = type.GetField(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        if (field != null && field.FieldType == typeof(bool))
        {
            value = (bool)field.GetValue(instance)!;
            return true;
        }

        var prop = type.GetProperty(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        if (prop != null && prop.PropertyType == typeof(bool) && prop.GetMethod != null)
        {
            value = (bool)prop.GetValue(instance)!;
            return true;
        }

        value = default;
        return false;
    }

    private static bool TrySetBoolMember(ref object instance, string name, bool value)
    {
        var type = instance.GetType();
        var field = type.GetField(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        if (field != null && field.FieldType == typeof(bool))
        {
            field.SetValue(instance, value);
            return true;
        }

        var prop = type.GetProperty(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        if (prop != null && prop.PropertyType == typeof(bool) && prop.SetMethod != null)
        {
            prop.SetValue(instance, value);
            return true;
        }

        return false;
    }
}
