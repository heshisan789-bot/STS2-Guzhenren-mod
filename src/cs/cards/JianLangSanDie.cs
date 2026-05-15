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
public sealed class JianLangSanDie : AbstractShaZhaoCard
{
    private const int WaveCount = 3;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(4m, ValueProp.Move),
        new PowerVar<JianHenPower>(4m)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<JianHenPower>()];

    public JianLangSanDie() : base(2, CardType.Attack, TargetType.AllEnemies)
    {
        SetDao(GuZhenRenDao.JianDao);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(CombatState, nameof(CombatState));

        for (var wave = 0; wave < WaveCount; wave++)
        {
            await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).TargetingAllOpponents(CombatState)
                .WithHitFx("vfx/vfx_attack_slash")
                .Execute(choiceContext);

            foreach (var enemy in CombatState.HittableEnemies.ToList())
            {
                await PowerCmd.Apply<JianHenPower>(enemy, DynamicVars["JianHenPower"].BaseValue, Owner.Creature, this);
            }
        }
    }

    protected override void OnUpgrade()
    {
    }
}
