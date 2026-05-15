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
[GuZhenRenDaoPool(GuZhenRenDao.TouDao)]
public sealed class TouXiGu : AbstractGuZhenRenCard
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<VulnerablePower>()];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(3m, ValueProp.Move | ValueProp.Unblockable),
        new PowerVar<VulnerablePower>(1m)
    ];

    public TouXiGu() : base(0, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
    {
        SetDao(GuZhenRenDao.TouDao);
        SetRank(2);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, nameof(cardPlay.Target));

        var attackTimes = IsUpgraded ? 2 : 1;
        for (var i = 0; i < attackTimes; i++)
        {
            await CreatureCmd.Damage(choiceContext, cardPlay.Target, DynamicVars.Damage.BaseValue, ValueProp.Move | ValueProp.Unblockable, Owner.Creature, this);
        }

        await PowerCmd.Apply<VulnerablePower>(cardPlay.Target, DynamicVars.Vulnerable.BaseValue, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        UpgradeRank(1);
    }
}
