using BaseLib.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace Guzhenren.Scripts;

[Pool(typeof(GuZhenRenCardPool))]
[GuZhenRenDaoPool(GuZhenRenDao.XueDao)]
public sealed class XueShouYinGu : AbstractGuZhenRenCard
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(7m, ValueProp.Move)];

    public XueShouYinGu() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
        SetDao(GuZhenRenDao.XueDao);
        SetRank(4);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, nameof(cardPlay.Target));

        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_blunt")
            .Execute(choiceContext);

        var selected = (await CardSelectCmd.FromHand(
            prefs: new CardSelectorPrefs(SelectionScreenPrompt, 1),
            context: choiceContext,
            player: Owner,
            filter: static _ => true,
            source: this)).FirstOrDefault();

        if (selected == null)
        {
            return;
        }

        if (selected.Type == CardType.Attack)
        {
            selected.SetToFreeThisTurn();
            selected.ExhaustOnNextPlay = true;
            await CardCmd.AutoPlay(choiceContext, selected, cardPlay.Target);
            return;
        }

        await CardPileCmd.Add(selected, PileType.Exhaust);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3m);
        UpgradeRank(1);
    }
}
