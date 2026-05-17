using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace Guzhenren.Scripts;

[Pool(typeof(GuZhenRenCardPool))]
[GuZhenRenDaoPool(GuZhenRenDao.BianHuaDao)]
public sealed class LongXiGu : AbstractGuZhenRenCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<FenShaoPower>(5m)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<FenShaoPower>(),
        HoverTipFactory.FromPower<JianHenPower>()
    ];

    public LongXiGu() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
        SetDao(GuZhenRenDao.BianHuaDao);
        SetRank(6);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, nameof(cardPlay.Target));

        await PowerCmd.Apply<FenShaoPower>(cardPlay.Target, DynamicVars["FenShaoPower"].BaseValue, Owner.Creature, this);

        var fenShao = cardPlay.Target.GetPowerAmount<FenShaoPower>();
        if (fenShao > 0)
        {
            await PowerCmd.Apply<JianHenPower>(cardPlay.Target, fenShao, Owner.Creature, this);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars["FenShaoPower"].UpgradeValueBy(2m);
        UpgradeRank(1);
    }
}
