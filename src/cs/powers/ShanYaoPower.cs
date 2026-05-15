using BaseLib.Abstracts;
using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Guzhenren.Scripts;

/// <summary>
/// 骨架版“闪耀”：为下一次攻击提供额外伤害并消耗 1 层。
/// 后续可按塔一正式实现继续细化。
/// </summary>
public sealed class ShanYaoPower : AbstractGuZhenRenPower
{
    private static decimal _totalGainedThisCombat;

    static ShanYaoPower()
    {
        BattleStateManager.OnBattleStart(ResetCombatTracking);
        BattleStateManager.OnPostBattle(ResetCombatTracking);
    }

    public static int TotalGainedThisCombat => (int)_totalGainedThisCombat;

    public static void RecordGain(decimal amount)
    {
        if (amount > 0)
        {
            _totalGainedThisCombat += amount;
        }
    }

    public static void ResetCombatTracking()
    {
        _totalGainedThisCombat = 0m;
    }

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override bool AllowNegative => false;

    public override decimal ModifyDamageMultiplicative(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (dealer != Owner || cardSource is not AbstractGuZhenRenCard guCard || guCard.PrimaryDao != GuZhenRenDao.GuangDao || !props.IsPoweredAttack_())
        {
            return 1m;
        }

        return 1m + Amount * 0.5m;
    }

    public override async Task AfterDamageGiven(PlayerChoiceContext choiceContext, Creature? dealer, DamageResult result, ValueProp props, Creature target, CardModel? cardSource)
    {
        if (dealer != Owner || cardSource is not AbstractGuZhenRenCard guCard || guCard.PrimaryDao != GuZhenRenDao.GuangDao || !props.IsPoweredAttack_() || Amount <= 0)
        {
            return;
        }

        if (Owner.GetPowerAmount<RiGuangPower>() > 0)
        {
            await PowerCmd.Apply<RiGuangPower>(Owner, -1, Owner, null, true);
        }
        else
        {
            await PowerCmd.Remove(this);
        }
    }
}
