using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace Guzhenren.Scripts;

[Pool(typeof(GuZhenRenCardPool))]
[GuZhenRenDaoPool(GuZhenRenDao.YanDao)]
public sealed class YuHuo : AbstractGuZhenRenCard
{
    private const string CalculatedTimesKey = "CalculatedTimes";

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<FenShaoPower>(1m),
        new CalculationBaseVar(0m),
        new CalculationExtraVar(1m),
        new CalculatedVar(CalculatedTimesKey).WithMultiplier(static (CardModel card, MegaCrit.Sts2.Core.Entities.Creatures.Creature? _) =>
        {
            var yuHuo = (YuHuo)card;
            return yuHuo.GetExhaustedThisTurnCount();
        })
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<FenShaoPower>()];

    public YuHuo() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.AllEnemies)
    {
        SetDao(GuZhenRenDao.YanDao);
        SetRank(6);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(CombatState, nameof(CombatState));

        var times = GetExhaustedThisTurnCount();
        if (times <= 0)
        {
            return;
        }

        for (var i = 0; i < times; i++)
        {
            foreach (var enemy in CombatState.HittableEnemies)
            {
                await PowerCmd.Apply<FenShaoPower>(enemy, DynamicVars["FenShaoPower"].BaseValue, Owner.Creature, this);
            }
        }
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
        UpgradeRank(1);
    }

    private int GetExhaustedThisTurnCount()
    {
        if (CombatState == null || Owner?.Creature == null || !CombatManager.Instance.IsInProgress)
        {
            return 0;
        }

        return CombatManager.Instance.History.Entries
            .OfType<CardExhaustedEntry>()
            .Count(e => e.Card.Owner?.Creature == Owner.Creature && e.HappenedThisTurn(CombatState));
    }
}
