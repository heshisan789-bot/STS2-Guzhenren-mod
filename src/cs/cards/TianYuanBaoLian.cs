using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Saves.Runs;

namespace Guzhenren.Scripts;

[Pool(typeof(GuZhenRenCardPool))]
[GuZhenRenDaoPool(GuZhenRenDao.MuDao)]
public sealed class TianYuanBaoLian : AbstractGuZhenRenCard
{
    private const int BaseEnergyGain = 2;
    private const int UpgradeExtraEnergy = 1;

    private int _decayCount;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new EnergyVar(BaseEnergyGain)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [EnergyHoverTip];

    [SavedProperty]
    public int DecayCount
    {
        get => _decayCount;
        set
        {
            AssertMutable();
            _decayCount = value;
        }
    }

    public TianYuanBaoLian() : base(0, CardType.Skill, CardRarity.Common, TargetType.None)
    {
        SetDao(GuZhenRenDao.MuDao);
        SetRank(3);
        RefreshEnergyGain();
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (DynamicVars.Energy.IntValue > 0)
        {
            await PlayerCmd.GainEnergy(DynamicVars.Energy.IntValue, Owner);
        }

        DecayCount += 1;
        RefreshEnergyGain();
    }

    protected override void OnUpgrade()
    {
        UpgradeRank(1);
        RefreshEnergyGain();
    }

    protected override void AfterDowngraded()
    {
        base.AfterDowngraded();
        RefreshEnergyGain();
    }

    private void RefreshEnergyGain()
    {
        DynamicVars.Energy.BaseValue = Math.Max(0, GetStartingEnergyGain() - DecayCount);
    }

    private int GetStartingEnergyGain()
    {
        return BaseEnergyGain + (IsUpgraded ? UpgradeExtraEnergy : 0);
    }
}
