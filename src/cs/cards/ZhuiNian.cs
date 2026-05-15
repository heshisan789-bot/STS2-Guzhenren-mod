using BaseLib.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;

namespace Guzhenren.Scripts;

[Pool(typeof(GuZhenRenCardPool))]
[GuZhenRenDaoPool(GuZhenRenDao.ZhiDao)]
public sealed class ZhuiNian : AbstractGuZhenRenCard
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<NianPower>()];

    public ZhuiNian() : base(1, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
        SetDao(GuZhenRenDao.ZhiDao);
        SetRank(6);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var discardPile = PileType.Discard.GetPile(Owner);
        if (discardPile.Cards.Count == 0)
        {
            return;
        }

        var selected = new List<CardModel>(await CardSelectCmd.FromSimpleGrid(
            choiceContext,
            discardPile.Cards,
            Owner,
            new CardSelectorPrefs(SelectionScreenPrompt, 1)));

        if (selected.Count == 0)
        {
            return;
        }

        var card = selected[0];
        var nianGain = CalculateNianGain(card);

        if (card.EnergyCost.CostsX)
        {
            card.EnergyCost.CapturedXValue = Owner.PlayerCombatState?.Energy ?? 0;
        }

        card.SetToFreeThisTurn();
        card.ExhaustOnNextPlay = true;
        await CardCmd.AutoPlay(choiceContext, card, null, skipXCapture: card.EnergyCost.CostsX);

        if (nianGain > 0)
        {
            await PowerCmd.Apply<NianPower>(Owner.Creature, nianGain, Owner.Creature, this);
        }
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
        UpgradeRank(1);
    }

    private decimal CalculateNianGain(CardModel card)
    {
        var cost = card.EnergyCost.CostsX
            ? Owner.PlayerCombatState?.Energy ?? 0
            : card.EnergyCost.GetWithModifiers(CostModifiers.All);

        return Math.Max(0, cost) * 2m;
    }
}
