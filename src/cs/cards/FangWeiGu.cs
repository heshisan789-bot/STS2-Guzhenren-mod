using BaseLib.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Guzhenren.Scripts;

[Pool(typeof(GuZhenRenCardPool))]
[GuZhenRenDaoPool(GuZhenRenDao.BianHuaDao)]
public sealed class FangWeiGu : AbstractGuZhenRenCard
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    public FangWeiGu() : base(2, CardType.Skill, CardRarity.Rare, TargetType.None)
    {
        SetDao(GuZhenRenDao.BianHuaDao);
        SetRank(7);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(CombatState, nameof(CombatState));

        List<CardModel> candidates;
        if (IsUpgraded)
        {
            // 升级后：从全卡牌库中发现所有合法蛊虫牌
            candidates = GuZhenRenCardCatalog.AllCardTypes
                .Where(t => !IsExcludedType(t))
                .Select(t => ModelDb.GetById<CardModel>(ModelDb.GetId(t)))
                .ToList();
        }
        else
        {
            // 未升级：仅从卡组中发现
            candidates = Owner.Deck.Cards
                .Where(c => c is AbstractGuZhenRenCard guCard
                    && guCard.Rank >= 1 && guCard.Rank <= 9
                    && !guCard.IsBenMingGu
                    && guCard.Rarity != CardRarity.Token
                    && guCard.GetType() != typeof(FangWeiGu))
                .DistinctBy(c => c.GetType())
                .ToList();
        }

        if (candidates.Count == 0)
            return;

        var selected = (await CardSelectCmd.FromSimpleGrid(
            choiceContext,
            candidates,
            Owner,
            new CardSelectorPrefs(SelectionScreenPrompt, 1, 1) { Cancelable = false })).ToList();

        if (selected.Count == 0)
            return;

        var copy = CombatState.CreateCard(selected[0], Owner);

        await CardPileCmd.AddGeneratedCardToCombat(copy, PileType.Hand, addedByPlayer: true);
    }

    protected override void OnUpgrade()
    {
        UpgradeRank(1);
    }

    private static bool IsExcludedType(Type t) =>
        t == typeof(FangWeiGu)
        || typeof(AbstractBenMingGuCard).IsAssignableFrom(t)
        || typeof(AbstractShaZhaoCard).IsAssignableFrom(t)
        || typeof(AbstractXuYingCard).IsAssignableFrom(t);
}
