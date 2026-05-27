using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Guzhenren.Scripts;

public sealed class BuMieXingBiaoPower : AbstractGuZhenRenPower
{
    public override PowerType Type => PowerType.Debuff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override bool AllowNegative => false;

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (Amount <= 0)
        {
            return;
        }

        Flash();
        await PowerCmd.Apply<NianPower>(player.Creature, Amount, Owner, null);
        await PowerCmd.SetAmount<BuMieXingBiaoPower>(Owner, Amount + 1m, Owner, null);
    }
}
