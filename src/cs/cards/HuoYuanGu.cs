using BaseLib.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace Guzhenren.Scripts;

[Pool(typeof(GuZhenRenCardPool))]
[GuZhenRenDaoPool(GuZhenRenDao.YanDao)]
public sealed class HuoYuanGu : AbstractGuZhenRenCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new CardsVar(2)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromCard<HuoShi>()];

    public HuoYuanGu() : base(1, CardType.Skill, CardRarity.Common, TargetType.None)
    {
        SetDao(GuZhenRenDao.YanDao);
        SetRank(2);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(CombatState, nameof(CombatState));

        var selected = (await CardSelectCmd.FromHand(
            prefs: new CardSelectorPrefs(SelectionScreenPrompt, 1),
            context: choiceContext,
            player: Owner,
            filter: static _ => true,
            source: this)).FirstOrDefault();

        if (selected != null)
        {
            await CardPileCmd.Add(selected, PileType.Exhaust);
        }

        for (var i = 0; i < DynamicVars.Cards.IntValue; i++)
        {
            var generated = CombatState.CreateCard<HuoShi>(Owner);
            if (IsUpgraded)
            {
                generated.UpgradeInternal();
                generated.FinalizeUpgradeInternal();
            }

            await CardPileCmd.AddGeneratedCardToCombat(generated, PileType.Hand, addedByPlayer: true);
        }
    }

    protected override void OnUpgrade()
    {
        UpgradeRank(1);
    }
}
