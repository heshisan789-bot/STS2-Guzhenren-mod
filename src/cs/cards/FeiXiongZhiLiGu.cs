using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace Guzhenren.Scripts;

[Pool(typeof(GuZhenRenCardPool))]
[GuZhenRenDaoPool(GuZhenRenDao.LiDao)]
public sealed class FeiXiongZhiLiGu : AbstractGuZhenRenCard
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(12m, ValueProp.Move),
        new PowerVar<VulnerablePower>(2m)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<VulnerablePower>(),
        HoverTipFactory.FromCard<FeiXiongXuYing>()
    ];

    public FeiXiongZhiLiGu() : base(2, CardType.Attack, CardRarity.Rare, TargetType.AllEnemies)
    {
        SetDao(GuZhenRenDao.LiDao);
        SetRank(6);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(CombatState, nameof(CombatState));

        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).TargetingAllOpponents(CombatState)
            .WithHitFx("vfx/vfx_attack_blunt", null, "heavy_attack.mp3")
            .Execute(choiceContext);

        foreach (var enemy in CombatState.HittableEnemies)
        {
            await PowerCmd.Apply<VulnerablePower>(enemy, DynamicVars.Vulnerable.BaseValue, Owner.Creature, this);
        }

        var generated = CombatState.CreateCard<FeiXiongXuYing>(Owner);
        if (IsUpgraded)
        {
            generated.UpgradeInternal();
            generated.FinalizeUpgradeInternal();
        }

        await CardPileCmd.AddGeneratedCardToCombat(generated, PileType.Hand, addedByPlayer: true);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Vulnerable.UpgradeValueBy(2m);
        UpgradeRank(1);
    }
}
