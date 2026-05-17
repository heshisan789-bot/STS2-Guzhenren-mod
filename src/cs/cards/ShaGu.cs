using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Saves.Runs;
using MegaCrit.Sts2.Core.ValueProps;

namespace Guzhenren.Scripts;

[Pool(typeof(GuZhenRenCardPool))]
[GuZhenRenDaoPool(GuZhenRenDao.ShaDao)]
public sealed class ShaGu : AbstractBenMingGuCard
{
    private const int BaseDamageRank1 = 6;

    private int _currentDamage = BaseDamageRank1;
    private int _increasedDamage;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(CurrentDamage, ValueProp.Move),
        new HpLossVar(1m),
        new DynamicVar("SecondMagic", 1m)
    ];

    [SavedProperty]
    public int CurrentDamage
    {
        get => _currentDamage;
        set
        {
            AssertMutable();
            _currentDamage = value;
            DynamicVars.Damage.BaseValue = _currentDamage;
        }
    }

    [SavedProperty]
    public int IncreasedDamage
    {
        get => _increasedDamage;
        set
        {
            AssertMutable();
            _increasedDamage = value;
        }
    }

    public ShaGu() : base(0, CardType.Attack, CardRarity.Token, TargetType.AnyEnemy)
    {
        SetDao(GuZhenRenDao.ShaDao);
        SetRank(1);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, nameof(cardPlay.Target));

        await CreatureCmd.Damage(choiceContext, Owner.Creature, DynamicVars.HpLoss.BaseValue, ValueProp.Unblockable | ValueProp.Unpowered | ValueProp.Move, this);

        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
    }

    public override Task AfterDeath(PlayerChoiceContext choiceContext, Creature creature, bool wasRemovalPrevented, float deathAnimLength)
    {
        if (wasRemovalPrevented || CombatState == null)
        {
            return Task.CompletedTask;
        }

        if (!creature.IsEnemy || creature.IsSecondaryEnemy || creature.GetPower<MinionPower>() != null)
        {
            return Task.CompletedTask;
        }

        var growth = DynamicVars["SecondMagic"].IntValue;
        BuffFromKill(growth);
        (DeckVersion as ShaGu)?.BuffFromKill(growth);
        return Task.CompletedTask;
    }

    protected override void OnRankChanged()
    {
        base.OnRankChanged();

        var rank = Math.Clamp(Rank, 1, 9);
        var baseDamages = new[] { 0, 6, 7, 8, 9, 10, 10, 11, 12, 13 };
        var incPerKill = new[] { 0, 1, 1, 1, 1, 1, 2, 2, 2, 3 };

        DynamicVars["SecondMagic"].BaseValue = incPerKill[rank];
        UpdateDamage(baseDamages[rank]);
    }

    protected override void OnUpgrade()
    {
        UpgradeRank(1);
    }

    protected override void AfterDowngraded()
    {
        base.AfterDowngraded();
        OnRankChanged();
    }

    private void BuffFromKill(int extraDamage)
    {
        IncreasedDamage += extraDamage;
        UpdateDamage(GetBaseDamageForRank());
    }

    private int GetBaseDamageForRank()
    {
        var rank = Math.Clamp(Rank, 1, 9);
        var baseDamages = new[] { 0, 6, 7, 8, 9, 10, 10, 11, 12, 13 };
        return baseDamages[rank];
    }

    private void UpdateDamage(int baseDamage)
    {
        var value = baseDamage + IncreasedDamage;
        if (IsCanonical)
        {
            _currentDamage = value;
            return;
        }

        CurrentDamage = value;
    }
}
