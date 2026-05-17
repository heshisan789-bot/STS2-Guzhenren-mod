using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using HarmonyLib;
using MegaCrit.Sts2.addons.mega_text;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.UI;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Cards;

namespace Guzhenren.Scripts;

[HarmonyPatch(typeof(NCard), "UpdateVisuals")]
internal static class GuZhenRenCardRankPatch
{
    private sealed class DescriptionCache
    {
        public string? Text { get; set; }
    }

    private static readonly ConditionalWeakTable<NCard, DescriptionCache> DescriptionCacheTable = new();

    private static readonly FieldInfo? DescriptionLabelField =
        typeof(NCard).GetField("_descriptionLabel", BindingFlags.NonPublic | BindingFlags.Instance);

    private static readonly FieldInfo? PreviewTargetField =
        typeof(NCard).GetField("_previewTarget", BindingFlags.NonPublic | BindingFlags.Instance);

    [HarmonyPostfix]
    private static void UpdateVisualsPostfix(NCard __instance, PileType pileType, CardPreviewMode previewMode)
    {
        try
        {
            if (__instance.Model is not AbstractGuZhenRenCard guzCard || __instance.Visibility != ModelVisibility.Visible)
            {
                return;
            }

            if (DescriptionLabelField?.GetValue(__instance) is not MegaRichTextLabel descriptionLabel)
            {
                return;
            }

            var target = (PreviewTargetField?.GetValue(__instance) as Creature) ?? __instance.Model.CurrentTarget;
            var text = previewMode != CardPreviewMode.Upgrade
                ? __instance.Model.GetDescriptionForPile(pileType, target)
                : __instance.Model.GetDescriptionForUpgradePreview();

            text = HighlightKeywords(text);

            var segments = new List<string>(5);
            if (guzCard.Rank > 0)
            {
                segments.Add($"[gold]{ToChineseRank(guzCard.Rank)}转[/gold]");
            }
            if (guzCard.PrimaryDao != GuZhenRenDao.None)
            {
                segments.Add($"[gold]{guzCard.DaoName}[/gold]");
            }
            if (guzCard.Rank >= 6 || guzCard.IsXianGu)
            {
                segments.Add("[gold]仙蛊[/gold]");
            }
            if (guzCard.IsShaZhao)
            {
                segments.Add("[gold]杀招[/gold]");
            }
            if (guzCard.IsBenMingGu)
            {
                segments.Add("[gold]本命蛊[/gold]");
            }

            var prefix = segments.Count > 0 ? string.Join("。", segments) + "。\n" : string.Empty;
            var rendered = "[center]" + prefix + text + "[/center]";
            var cache = DescriptionCacheTable.GetOrCreateValue(__instance);
            if (cache.Text == rendered)
            {
                return;
            }

            cache.Text = rendered;
            descriptionLabel.SetTextAutoSize(rendered);
        }
        catch (Exception e)
        {
            MegaCrit.Sts2.Core.Logging.Log.Error($"GuZhenRenCardRankPatch.UpdateVisuals failed card={__instance.Model?.Id.Entry ?? "null"}: {e}");
        }
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
        foreach (var keyword in keywords)
        {
            segment = segment.Replace(keyword, $"[gold]{keyword}[/gold]", StringComparison.Ordinal);
        }

        return segment;
    }
}
