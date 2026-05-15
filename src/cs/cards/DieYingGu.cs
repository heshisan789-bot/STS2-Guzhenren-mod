using System.Linq;
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
public sealed class DieYingGu : AbstractGuZhenRenCard
{
    private const string GrowthKey = "Growth";

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(8m, ValueProp.Move),
        new PowerVar<JianHenPower>(1m),
        new DynamicVar(GrowthKey, 2m)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<JianHenPower>()];

    public DieYingGu()
        : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
        SetDao(GuZhenRenDao.JianDao);
        SetRank(4);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, nameof(cardPlay.Target));
        var count = CountJianYingInExhaust();
        var damage = DynamicVars.Damage.BaseValue + count * DynamicVars[GrowthKey].BaseValue;
        var jianHen = DynamicVars["JianHenPower"].BaseValue + count * DynamicVars[GrowthKey].BaseValue;

        await DamageCmd.Attack(damage).FromCard(this).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
        await PowerCmd.Apply<JianHenPower>(cardPlay.Target, jianHen, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars[GrowthKey].UpgradeValueBy(1m);
        UpgradeRank(1);
    }

    private int CountJianYingInExhaust()
    {
        return PileType.Exhaust.GetPile(Owner).Cards.Count(c => c is JianYing);
    }
}
