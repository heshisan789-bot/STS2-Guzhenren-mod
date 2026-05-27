using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Saves.Runs;
using MegaCrit.Sts2.Core.ValueProps;

namespace Guzhenren.Scripts;

[Pool(typeof(GuZhenRenCardPool))]
[GuZhenRenDaoPool(GuZhenRenDao.XueDao)]
public sealed class XueShenZi : AbstractGuZhenRenCard
{
    private const int BaseDamageAmount = 6;

    private int _currentDamage = BaseDamageAmount;
    private int _increasedDamage;

    public override IEnumerable<CardKeyword> CanonicalKeywords => IsUpgraded ? [] : [CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(CurrentDamage, ValueProp.Move),
        new HpLossVar(6m),
        new DynamicVar("SecondMagic", 6m)
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

    public XueShenZi() : base(1, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
    {
        SetDao(GuZhenRenDao.XueDao);
        SetRank(5);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, nameof(cardPlay.Target));

        await CreatureCmd.Damage(choiceContext, Owner.Creature, DynamicVars.HpLoss.BaseValue, ValueProp.Unblockable | ValueProp.Unpowered | ValueProp.Move, this);
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_bloody_impact")
            .Execute(choiceContext);

        var growth = DynamicVars["SecondMagic"].IntValue;
        BuffFromPlay(growth);
        (DeckVersion as XueShenZi)?.BuffFromPlay(growth);
    }

    protected override void OnUpgrade()
    {
        if (!IsCanonical)
        {
            RemoveKeyword(CardKeyword.Exhaust);
        }

        UpgradeRank(1);
    }

    protected override void AfterDowngraded()
    {
        base.AfterDowngraded();
        if (!IsCanonical)
        {
            AddKeyword(CardKeyword.Exhaust);
        }
        UpdateDamage();
    }

    private void BuffFromPlay(int extraDamage)
    {
        IncreasedDamage += extraDamage;
        UpdateDamage();
    }

    private void UpdateDamage()
    {
        var value = BaseDamageAmount + IncreasedDamage;
        if (IsCanonical)
        {
            _currentDamage = value;
            return;
        }

        CurrentDamage = value;
    }
}
