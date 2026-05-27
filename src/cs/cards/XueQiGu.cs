using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace Guzhenren.Scripts;

[Pool(typeof(GuZhenRenCardPool))]
[GuZhenRenDaoPool(GuZhenRenDao.XueDao)]
public sealed class XueQiGu : AbstractGuZhenRenCard
{
    private const string CalculatedHealKey = "CalculatedHeal";

    public override IEnumerable<CardKeyword> CanonicalKeywords => IsUpgraded
        ? [CardKeyword.Exhaust, CardKeyword.Retain]
        : [CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CalculationBaseVar(2m),
        new CalculationExtraVar(1m),
        new CalculatedVar(CalculatedHealKey).WithMultiplier(static (card, _) => ((XueQiGu)card).CountHpLossEventsThisCombat())
    ];

    public XueQiGu() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        SetDao(GuZhenRenDao.XueDao);
        SetRank(2);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var healAmount = CalculateHealAmount();
        if (healAmount > 0m)
        {
            await CreatureCmd.Heal(Owner.Creature, healAmount);
        }
    }

    protected override void OnUpgrade()
    {
        if (!IsCanonical)
        {
            AddKeyword(CardKeyword.Retain);
        }

        UpgradeRank(1);
    }

    private int CountHpLossEventsThisCombat()
    {
        if (!CombatManager.Instance.IsInProgress)
        {
            return 0;
        }

        return CombatManager.Instance.History.Entries.OfType<DamageReceivedEntry>()
            .Count(entry => entry.Receiver == Owner.Creature && entry.Result.UnblockedDamage > 0);
    }

    private decimal CalculateHealAmount()
    {
        return DynamicVars.CalculationBase.BaseValue + CountHpLossEventsThisCombat() * DynamicVars.CalculationExtra.BaseValue;
    }
}
