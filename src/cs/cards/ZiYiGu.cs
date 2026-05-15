using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Guzhenren.Scripts;

[Pool(typeof(GuZhenRenCardPool))]
[GuZhenRenDaoPool(GuZhenRenDao.ZhiDao)]
public sealed class ZiYiGu : AbstractGuZhenRenCard
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<VulnerablePower>(3m),
        new PowerVar<YiPower>(3m)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<StrengthPower>(),
        HoverTipFactory.FromPower<VulnerablePower>(),
        HoverTipFactory.FromPower<YiPower>(),
        HoverTipFactory.FromPower<QingPower>()
    ];

    public ZiYiGu() : base(1, CardType.Skill, CardRarity.Common, TargetType.AllEnemies)
    {
        SetDao(GuZhenRenDao.ZhiDao);
        SetRank(3);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(CombatState, nameof(CombatState));

        foreach (var enemy in CombatState.HittableEnemies)
        {
            await PowerCmd.Apply<StrengthPower>(enemy, 1m, Owner.Creature, this);
            await PowerCmd.Apply<VulnerablePower>(enemy, DynamicVars.Vulnerable.BaseValue, Owner.Creature, this);
        }

        await PowerCmd.Apply<YiPower>(Owner.Creature, DynamicVars["YiPower"].BaseValue, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Vulnerable.UpgradeValueBy(2m);
        DynamicVars["YiPower"].UpgradeValueBy(2m);
        UpgradeRank(1);
    }
}
