using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace Guzhenren.Scripts;

[Pool(typeof(GuZhenRenCardPool))]
[GuZhenRenDaoPool(GuZhenRenDao.YanDao)]
public sealed class AngryBird : AbstractShaZhaoCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(19m, ValueProp.Move),
        new PowerVar<FenShaoPower>(19m)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<FenShaoPower>()];

    public AngryBird() : base(2, CardType.Attack, TargetType.AnyEnemy)
    {
        SetDao(GuZhenRenDao.YanDao);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, nameof(cardPlay.Target));

        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_bloody_impact")
            .Execute(choiceContext);
        await PowerCmd.Apply<FenShaoPower>(cardPlay.Target, DynamicVars["FenShaoPower"].BaseValue, Owner.Creature, this);
    }
}
