using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace Guzhenren.Scripts;

[Pool(typeof(GuZhenRenCardPool))]
[GuZhenRenDaoPool(GuZhenRenDao.LuDao)]
public sealed class ShiBaiGu : AbstractGuZhenRenCard, IGuZhenRenProbabilityCard
{
    private const string ChanceKey = "Chance";
    private decimal _baseChancePercent = 1m;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new HpLossVar(4m),
        new DynamicVar(ChanceKey, _baseChancePercent)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromCard<ChengGongGu>()];

    public ShiBaiGu() : base(1, CardType.Skill, CardRarity.Common, TargetType.Self)
    {
        SetDao(GuZhenRenDao.LuDao);
        SetRank(1);
    }

    public void IncreaseBaseChance(decimal deltaProbability)
    {
        _baseChancePercent = Math.Clamp(_baseChancePercent + deltaProbability * 100m, 0m, 100m);
        DynamicVars[ChanceKey].BaseValue = _baseChancePercent;
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(CombatState, nameof(CombatState));

        await CreatureCmd.Damage(choiceContext, Owner.Creature, DynamicVars.HpLoss.BaseValue,
            ValueProp.Unblockable | ValueProp.Unpowered | ValueProp.Move, this);

        var chance = (float)(_baseChancePercent / 100m);
        if (Owner.RunState.Rng.Shuffle.NextFloat() > chance)
            return;

        var generated = CombatState.CreateCard<ChengGongGu>(Owner);
        await CardPileCmd.AddGeneratedCardToCombat(generated, PileType.Hand, addedByPlayer: true);

        if (DeckVersion != null)
            await CardPileCmd.RemoveFromDeck(DeckVersion);

        await CardPileCmd.Add(this, PileType.Exhaust);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.HpLoss.UpgradeValueBy(-2m);
        UpgradeRank(1);
    }
}
