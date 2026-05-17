using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace Guzhenren.Scripts;

[Pool(typeof(GuZhenRenCardPool))]
[GuZhenRenDaoPool(GuZhenRenDao.JianDao)]
public sealed class WuZhiQuanXinJian : AbstractShaZhaoCard
{
    private const string UsesLeftKey = "UsesLeft";

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(9m, ValueProp.Move),
        new DynamicVar(UsesLeftKey, 5m)
    ];

    public WuZhiQuanXinJian() : base(1, CardType.Attack, TargetType.AnyEnemy)
    {
        SetDao(GuZhenRenDao.JianDao);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, nameof(cardPlay.Target));

        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);

        DynamicVars.Damage.BaseValue *= 3m;

        var usesLeft = (int)DynamicVars[UsesLeftKey].BaseValue;
        usesLeft = Math.Max(0, usesLeft - 1);
        DynamicVars[UsesLeftKey].BaseValue = usesLeft;

        if (usesLeft <= 0)
        {
            await CardCmd.Exhaust(choiceContext, this);
        }
    }
}
