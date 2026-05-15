using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace Guzhenren.Scripts;

[Pool(typeof(GuZhenRenCardPool))]
[GuZhenRenDaoPool(GuZhenRenDao.JianDao)]
public sealed class JianYingGu : AbstractGuZhenRenCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(4m, ValueProp.Move),
        new PowerVar<JianHenPower>(2m)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<JianHenPower>()];

    public JianYingGu()
        : base(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
    {
        SetDao(GuZhenRenDao.JianDao);
        SetRank(2);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(CombatState, nameof(CombatState));
        ArgumentNullException.ThrowIfNull(cardPlay.Target, nameof(cardPlay.Target));
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
        await PowerCmd.Apply<JianHenPower>(cardPlay.Target, DynamicVars["JianHenPower"].BaseValue, Owner.Creature, this);
        await CardPileCmd.AddGeneratedCardToCombat(CombatState.CreateCard<JianYing>(Owner), PileType.Hand, addedByPlayer: true);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["JianHenPower"].UpgradeValueBy(3m);
        UpgradeRank(1);
    }
}
