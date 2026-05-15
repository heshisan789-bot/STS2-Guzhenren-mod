using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Guzhenren.Scripts;

[Pool(typeof(GuZhenRenCardPool))]
[GuZhenRenDaoPool(GuZhenRenDao.LiDao)]
public sealed class KuLiGu : AbstractGuZhenRenCard
{
    private const string ThresholdKey = "Threshold";
    private const string CalculatedStrengthKey = "CalculatedStrength";

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar(ThresholdKey, 6m),
        new CalculationBaseVar(0m),
        new CalculationExtraVar(1m),
        new CalculatedVar(CalculatedStrengthKey).WithMultiplier((CardModel card, MegaCrit.Sts2.Core.Entities.Creatures.Creature? _) =>
        {
            var kuLiGu = (KuLiGu)card;
            return kuLiGu.CalculateTemporaryStrengthAmount();
        })
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<StrengthPower>()];

    public KuLiGu() : base(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        SetDao(GuZhenRenDao.LiDao);
        SetRank(4);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var amount = CalculateTemporaryStrengthAmount();
        if (amount <= 0)
        {
            return;
        }

        await PowerCmd.Apply<StrengthPower>(Owner.Creature, amount, Owner.Creature, this);
        await PowerCmd.Apply<LiLiangGuStrengthDownPower>(Owner.Creature, amount, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars[ThresholdKey].UpgradeValueBy(-2m);
        UpgradeRank(1);
    }

    private decimal CalculateTemporaryStrengthAmount()
    {
        if (Owner?.Creature == null)
        {
            return 0m;
        }

        var threshold = Math.Max(1m, DynamicVars[ThresholdKey].BaseValue);
        var missingHp = Math.Max(0, Owner.Creature.MaxHp - Owner.Creature.CurrentHp);
        return Math.Floor(missingHp / threshold);
    }
}
