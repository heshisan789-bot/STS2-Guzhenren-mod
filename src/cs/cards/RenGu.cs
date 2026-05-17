using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace Guzhenren.Scripts;

[Pool(typeof(GuZhenRenCardPool))]
[GuZhenRenDaoPool(GuZhenRenDao.JianDao)]
public sealed class RenGu : AbstractBenMingGuCard
{
    private static readonly MethodInfo _modelDbCardMethod =
        typeof(MegaCrit.Sts2.Core.Models.ModelDb).GetMethods(BindingFlags.Public | BindingFlags.Static)
            .Single(m => m is { Name: "Card", IsGenericMethodDefinition: true } && m.GetParameters().Length == 0);

    private const string ChoicesKey = "Choices";
    private const string JianFengKey = "JianFeng";

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar(ChoicesKey, 1m),
        new PowerVar<JianFengPower>(0m),
        new DynamicVar(JianFengKey, 0m)
    ];

    public RenGu() : base(1, CardType.Skill, CardRarity.Token, TargetType.Self)
    {
        SetDao(GuZhenRenDao.JianDao);
        SetRank(1);
    }

    protected override void OnRankChanged()
    {
        base.OnRankChanged();

        var rank = Math.Clamp(Rank, 1, 9);
        var choices = rank == 1 ? 1 : rank <= 7 ? 2 : 3;

        var jianFeng = rank switch
        {
            5 or 6 => 1m,
            7 or 8 => 2m,
            9 => 3m,
            _ => 0m
        };

        DynamicVars[ChoicesKey].BaseValue = choices;
        DynamicVars["JianFengPower"].BaseValue = jianFeng;
        DynamicVars[JianFengKey].BaseValue = jianFeng;
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(CombatState, nameof(CombatState));

        var jianFeng = DynamicVars["JianFengPower"].BaseValue;
        if (jianFeng > 0m)
        {
            await PowerCmd.Apply<JianFengPower>(Owner.Creature, jianFeng, Owner.Creature, this);
        }

        var rank = Math.Clamp(Rank, 1, 9);
        var poolType = rank == 3 ? 2 : rank >= 4 ? 3 : 1;
        var doUpgrade = rank >= 6;
        var optionsCount = DynamicVars[ChoicesKey].IntValue;

        var candidates = GuZhenRenCardCatalog.GetCardTypes(GuZhenRenDao.JianDao)
            .Where(t => t != GetType() && !typeof(AbstractBenMingGuCard).IsAssignableFrom(t))
            .ToList();

        var rng = Owner.RunState.Rng.CombatCardGeneration;
        for (var i = candidates.Count - 1; i > 0; i--)
        {
            var j = rng.NextInt(i + 1);
            (candidates[i], candidates[j]) = (candidates[j], candidates[i]);
        }

        var options = new List<CardModel>();
        foreach (var type in candidates)
        {
            if (options.Count >= optionsCount)
            {
                break;
            }

            var canonical = (CardModel)_modelDbCardMethod.MakeGenericMethod(type).Invoke(null, null)!;
            if (canonical is not AbstractGuZhenRenCard guCard || guCard.Rank is < 1 or > 9)
            {
                continue;
            }

            var isXianGuCandidate = guCard.IsXianGu || guCard.Rank >= 6;
            if (isXianGuCandidate && Owner.Deck.Cards.Any(c => c.Id == guCard.Id && c is AbstractGuZhenRenCard deckCard && (deckCard.IsXianGu || deckCard.Rank >= 6)))
            {
                continue;
            }

            if (!IsCandidateRarityAllowed(canonical.Rarity, poolType))
            {
                continue;
            }

            var created = CombatState.CreateCard(canonical, Owner);
            if (doUpgrade && created.IsUpgradable)
            {
                created.UpgradeInternal();
                created.FinalizeUpgradeInternal();
            }
            created.SetToFreeThisTurn();
            options.Add(created);
        }

        if (options.Count == 0)
        {
            return;
        }

        CardModel? selected;
        if (options.Count == 1)
        {
            selected = options[0];
        }
        else
        {
            selected = await CardSelectCmd.FromChooseACardScreen(choiceContext, options, Owner, canSkip: true);
        }

        if (selected == null)
        {
            return;
        }

        await CardPileCmd.AddGeneratedCardToCombat(selected, PileType.Hand, addedByPlayer: true);
    }

    protected override void OnUpgrade()
    {
        UpgradeRank(1);
    }

    private static bool IsCandidateRarityAllowed(CardRarity rarity, int poolType)
    {
        return poolType switch
        {
            1 => rarity == CardRarity.Common,
            2 => rarity is CardRarity.Common or CardRarity.Uncommon,
            3 => rarity is CardRarity.Common or CardRarity.Uncommon or CardRarity.Rare,
            _ => false
        };
    }
}
