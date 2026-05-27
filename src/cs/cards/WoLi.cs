using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Guzhenren.Scripts;

[Pool(typeof(GuZhenRenCardPool))]
[GuZhenRenDaoPool(GuZhenRenDao.LiDao)]
public sealed class WoLi : AbstractGuZhenRenCard
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<OneTwoPunchPower>(1m)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<OneTwoPunchPower>(),
        HoverTipFactory.FromCard<WoLiXuYing>(IsUpgradedOrUpgradePreview)
    ];

    public WoLi() : base(2, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
        SetDao(GuZhenRenDao.LiDao);
        SetRank(6);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(CombatState, nameof(CombatState));

        await PowerCmd.Apply<OneTwoPunchPower>(Owner.Creature, DynamicVars["OneTwoPunchPower"].BaseValue, Owner.Creature, this);

        var generated = CombatState.CreateCard<WoLiXuYing>(Owner);
        if (IsUpgraded)
        {
            generated.UpgradeInternal();
            generated.FinalizeUpgradeInternal();
        }

        await CardPileCmd.AddGeneratedCardToCombat(generated, PileType.Hand, addedByPlayer: true);
    }

    protected override void OnUpgrade()
    {
        UpgradeRank(1);
    }
}
