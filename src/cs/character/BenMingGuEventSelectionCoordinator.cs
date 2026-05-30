using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Random;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.Saves;

namespace Guzhenren.Scripts;

internal static class BenMingGuEventSelectionCoordinator
{
    private static readonly MethodInfo _setEventStateMethod =
        AccessTools.Method(typeof(EventModel), "SetEventState")
        ?? throw new MissingMethodException(typeof(EventModel).FullName, "SetEventState");

    private static bool _injecting;

    public static async Task TryInjectCurrentRoom()
    {
        if (_injecting)
        {
            return;
        }

        var runState = RunManager.Instance.DebugOnlyGetState();
        if (runState?.CurrentRoom is not EventRoom eventRoom)
        {
            return;
        }

        var localEvent = eventRoom.LocalMutableEvent;
        if (localEvent.Owner == null)
        {
            return;
        }

        var player = localEvent.Owner;
        if (!ShouldInject(player))
        {
            return;
        }

        _injecting = true;
        try
        {
            var originalDescription = localEvent.Description ?? localEvent.InitialDescription;
            var originalOptions = localEvent.CurrentOptions.ToList();
            if (originalOptions.Count == 0)
            {
                Log.Warn("BenMingGu event injection skipped because the current event has no options.");
                return;
            }

            var options = CreateSelectionOptions(localEvent, player, originalDescription, originalOptions);
            Log.Info($"BenMingGu event injected into {eventRoom.CanonicalEvent.Id.Entry} for player={player.NetId} options={string.Join(",", options.Select(o => o.Title.GetRawText()))}");
            SetEventState(localEvent, new LocString("events", "GUZHENREN-BEN_MING_GU_SELECTION.description"), options);
        }
        finally
        {
            _injecting = false;
        }
    }

    private static bool ShouldInject(MegaCrit.Sts2.Core.Entities.Players.Player player)
    {
        return player.Character is FangYuanCharacter
               && player.Relics.OfType<BenMingGuStarterRelic>().Any()
               && player.Deck.Cards.OfType<AbstractBenMingGuCard>().Any() == false;
    }

    private static List<EventOption> CreateSelectionOptions(
        EventModel eventModel,
        MegaCrit.Sts2.Core.Entities.Players.Player player,
        LocString originalDescription,
        IReadOnlyList<EventOption> originalOptions)
    {
        var cards = new List<CardModel>
        {
            ModelDb.Card<XinXue>(),
            ModelDb.Card<ZhiHuiGu>(),
            ModelDb.Card<LiLiangGu>(),
            ModelDb.Card<HuoGu>(),
            ModelDb.Card<RenGu>(),
            ModelDb.Card<ShaGu>()
        };

        var rng = new Rng(player.RunState.Rng.Seed, $"guzhenren_ben_ming_gu_{player.NetId}");
        rng.Shuffle(cards);

        return cards.Take(3)
            .Select(card => CreateChoiceOption(eventModel, player, card, originalDescription, originalOptions))
            .ToList();
    }

    private static EventOption CreateChoiceOption(
        EventModel eventModel,
        MegaCrit.Sts2.Core.Entities.Players.Player player,
        CardModel selectedCard,
        LocString originalDescription,
        IReadOnlyList<EventOption> originalOptions)
    {
        async Task OnChosen()
        {
            var toAdd = player.RunState.CreateCard(selectedCard, player);
            toAdd.FloorAddedToDeck = 1;
            SaveManager.Instance.MarkCardAsSeen(toAdd);
            if (!player.DiscoveredCards.Contains(toAdd.Id))
            {
                player.DiscoveredCards.Add(toAdd.Id);
            }
            var addResult = await CardPileCmd.Add(toAdd, PileType.Deck);
            CardCmd.PreviewCardPileAdd(addResult, 1.2f, CardPreviewStyle.EventLayout);
            if (addResult.success)
            {
                addResult.cardAdded.Pile?.InvokeCardAddFinished();
            }
            Log.Info(
                $"BenMingGu chosen card={toAdd.Id.Entry} success={addResult.success} pile={toAdd.Pile?.Type.ToString() ?? "null"} deckCount={player.Deck.Cards.Count} discovered={player.DiscoveredCards.Contains(toAdd.Id)} pool={toAdd.Pool.Id.Entry}");

            var starterRelic = player.Relics.OfType<BenMingGuStarterRelic>().FirstOrDefault();
            if (starterRelic != null)
            {
                await RelicCmd.Remove(starterRelic);
            }

            SetEventState(
                eventModel,
                new LocString("events", "GUZHENREN-BEN_MING_GU_SELECTION.confirmationDescription"),
                [
                    new EventOption(
                        eventModel,
                        () =>
                        {
                            SetEventState(eventModel, originalDescription, originalOptions);
                            return Task.CompletedTask;
                        },
                        new LocString("events", "GUZHENREN-BEN_MING_GU_SELECTION.continueTitle"),
                        new LocString("events", "GUZHENREN-BEN_MING_GU_SELECTION.continueDescription"),
                        "GUZHENREN-BEN_MING_GU_SELECTION.continue",
                        Array.Empty<MegaCrit.Sts2.Core.HoverTips.IHoverTip>())
                ]);
        }

        var hoverTips = new[] { HoverTipFactory.FromCard(selectedCard) }
            .Concat(selectedCard.HoverTips)
            .ToArray();

        return new EventOption(
            eventModel,
            OnChosen,
            selectedCard.TitleLocString,
            new LocString("events", "GUZHENREN-BEN_MING_GU_SELECTION.optionDescription"),
            $"GUZHENREN-BEN_MING_GU_SELECTION.options.{selectedCard.Id.Entry}",
            hoverTips);
    }

    private static void SetEventState(EventModel eventModel, LocString description, IEnumerable<EventOption> options)
    {
        _setEventStateMethod.Invoke(eventModel, [description, options]);
    }
}
