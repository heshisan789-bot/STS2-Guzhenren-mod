using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace Guzhenren.Scripts;

[Pool(typeof(GuZhenRenCardPool))]
[GuZhenRenDaoPool(GuZhenRenDao.JinDao)]
public sealed class JuChiJinWu : AbstractGuZhenRenCard
{
    private const string HitsKey = "Hits";

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(2m, ValueProp.Move),
        new DynamicVar(HitsKey, 5m)
    ];

    public JuChiJinWu() : base(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
    {
        SetDao(GuZhenRenDao.JinDao);
        SetRank(3);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, nameof(cardPlay.Target));

        var hits = (int)DynamicVars[HitsKey].BaseValue;
        for (var i = 0; i < hits; i++)
        {
            await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(cardPlay.Target)
                .WithHitFx("vfx/vfx_attack_slash")
                .Execute(choiceContext);
        }

        if (hits > 0)
        {
            DynamicVars[HitsKey].BaseValue = Math.Max(0m, hits - 1);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars[HitsKey].UpgradeValueBy(1m);
        UpgradeRank(1);
    }
}
