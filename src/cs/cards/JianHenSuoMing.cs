using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace Guzhenren.Scripts;

[Pool(typeof(GuZhenRenCardPool))]
[GuZhenRenDaoPool(GuZhenRenDao.JianDao)]
public sealed class JianHenSuoMing : AbstractShaZhaoCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(0m, ValueProp.Move)];

    public JianHenSuoMing() : base(2, CardType.Attack, TargetType.AnyEnemy)
    {
        SetDao(GuZhenRenDao.JianDao);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, nameof(cardPlay.Target));

        var hits = cardPlay.Target.GetPowerAmount<JianHenPower>();
        for (var i = 0; i < hits; i++)
        {
            await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(cardPlay.Target)
                .WithHitFx("vfx/vfx_heavy_slash")
                .Execute(choiceContext);
        }

        PlayerCmd.EndTurn(Owner, canBackOut: false);
    }

    protected override void OnUpgrade()
    {
    }
}
