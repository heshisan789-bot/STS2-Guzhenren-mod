using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models;

namespace Guzhenren.Scripts;

[HarmonyPatch]
internal static class GuZhenRenCardDescriptionPrefixPatch
{
    [HarmonyPatch(typeof(CardModel), nameof(CardModel.GetDescriptionForPile), [typeof(PileType), typeof(Creature)])]
    [HarmonyPostfix]
    private static void GetDescriptionForPilePostfix(CardModel __instance, PileType pileType, Creature? target, ref string __result)
    {
        if (__instance is not AbstractGuZhenRenCard guzCard)
        {
            return;
        }

        __result = ApplyPrefixAndHighlight(guzCard, __result);
    }

    [HarmonyPatch(typeof(CardModel), nameof(CardModel.GetDescriptionForUpgradePreview))]
    [HarmonyPostfix]
    private static void GetDescriptionForUpgradePreviewPostfix(CardModel __instance, ref string __result)
    {
        if (__instance is not AbstractGuZhenRenCard guzCard)
        {
            return;
        }

        __result = ApplyPrefixAndHighlight(guzCard, __result);
    }

    private static string ApplyPrefixAndHighlight(AbstractGuZhenRenCard card, string text)
    {
        text = HighlightKeywords(text);

        var segments = new List<string>(5);
        if (card.Rank > 0)
        {
            segments.Add($"[gold]{ToChineseRank(card.Rank)}转[/gold]");
        }
        if (card.PrimaryDao != GuZhenRenDao.None)
        {
            segments.Add($"[gold]{card.DaoName}[/gold]");
        }
        if (card.Rank >= 6 || card.IsXianGu)
        {
            segments.Add("[gold]仙蛊[/gold]");
        }
        if (card.IsShaZhao)
        {
            segments.Add("[gold]杀招[/gold]");
        }
        if (card.IsBenMingGu)
        {
            segments.Add("[gold]本命蛊[/gold]");
        }

        if (segments.Count == 0)
        {
            return text;
        }

        return string.Join("。", segments) + "。\n" + text;
    }

    private static string ToChineseRank(int rank)
    {
        return rank switch
        {
            1 => "一",
            2 => "二",
            3 => "三",
            4 => "四",
            5 => "五",
            6 => "六",
            7 => "七",
            8 => "八",
            9 => "九",
            _ => rank.ToString()
        };
    }

    private static string HighlightKeywords(string text)
    {
        if (string.IsNullOrEmpty(text))
        {
            return text;
        }

        const string open = "[gold]";
        const string close = "[/gold]";
        var sb = new StringBuilder(text.Length + 32);
        var idx = 0;
        while (idx < text.Length)
        {
            var openIdx = text.IndexOf(open, idx, StringComparison.OrdinalIgnoreCase);
            if (openIdx < 0)
            {
                sb.Append(HighlightKeywordsInSegment(text[idx..]));
                break;
            }

            sb.Append(HighlightKeywordsInSegment(text[idx..openIdx]));
            var closeIdx = text.IndexOf(close, openIdx, StringComparison.OrdinalIgnoreCase);
            if (closeIdx < 0)
            {
                sb.Append(text[openIdx..]);
                break;
            }

            sb.Append(text[openIdx..(closeIdx + close.Length)]);
            idx = closeIdx + close.Length;
        }

        return sb.ToString();
    }

    private static string HighlightKeywordsInSegment(string segment)
    {
        if (string.IsNullOrEmpty(segment))
        {
            return segment;
        }

        segment = Regex.Replace(segment, @"(\d+\s*点)(念)", "$1[gold]念[/gold]");
        segment = Regex.Replace(segment, @"(\d+\s*点)(情)", "$1[gold]情[/gold]");
        segment = Regex.Replace(segment, @"(\d+\s*点)(意)", "$1[gold]意[/gold]");

        var keywords = new[]
        {
            "概率",
            "消耗",
            "力量",
            "虚弱",
            "闪耀",
            "虚无"
        };

        return keywords.Aggregate(segment, (current, keyword) =>
            current.Replace(keyword, $"[gold]{keyword}[/gold]", StringComparison.Ordinal));
    }
}
