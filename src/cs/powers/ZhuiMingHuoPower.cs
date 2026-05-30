using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Guzhenren.Scripts;

public sealed class ZhuiMingHuoPower : AbstractGuZhenRenPower
{
    private const int FenShaoBase = 5;

    internal static readonly HashSet<Creature> SpreadThisTurn = [];

    public override PowerType Type => PowerType.Debuff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override bool AllowNegative => false;

    public override async Task AfterSideTurnStart(CombatSide side, CombatState combatState)
    {
        if (side != Owner.Side)
        {
            return;
        }

        // 清理上回合的扩散跟踪
        SpreadThisTurn.Clear();

        if (Owner.IsDead || Amount <= 0)
        {
            return;
        }

        Flash();
        var totalFenShao = FenShaoBase * Amount;
        var applier = combatState.Players.FirstOrDefault()?.Creature ?? Owner;
        await PowerCmd.Apply<FenShaoPower>(Owner, totalFenShao, applier, null);
    }
}
