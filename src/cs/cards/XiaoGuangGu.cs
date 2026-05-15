using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Guzhenren.Scripts;

[Pool(typeof(GuZhenRenCardPool))]
[GuZhenRenDaoPool(GuZhenRenDao.GuangDao)]
public sealed class XiaoGuangGu : AbstractGuZhenRenCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<ShanYaoPower>(1m),
        new PowerVar<WeakPower>(1m)
    ];

    public XiaoGuangGu() : base(0, CardType.Skill, CardRarity.Basic, TargetType.AnyEnemy)
    {
        SetDao(GuZhenRenDao.GuangDao);
        SetRank(1);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, nameof(cardPlay.Target));

        ShanYaoPower.RecordGain(DynamicVars["ShanYaoPower"].BaseValue);
        await PowerCmd.Apply<ShanYaoPower>(Owner.Creature, DynamicVars["ShanYaoPower"].BaseValue, Owner.Creature, this);
        await PowerCmd.Apply<WeakPower>(cardPlay.Target, DynamicVars.Weak.BaseValue, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["ShanYaoPower"].UpgradeValueBy(1);
        UpgradeRank(1);
    }
}
