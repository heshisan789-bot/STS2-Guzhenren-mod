using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Guzhenren.Scripts;

public sealed class LiQiPower : AbstractGuZhenRenPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override bool AllowNegative => false;

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player.Creature != Owner || Amount <= 0)
        {
            return;
        }

        var combatState = Owner.CombatState;
        if (combatState == null)
        {
            return;
        }

        var hand = PileType.Hand.GetPile(player);
        var phantoms = hand.Cards.OfType<AbstractXuYingCard>().ToList();
        if (phantoms.Count == 0 || combatState.HittableEnemies.Count == 0)
        {
            return;
        }

        Flash();
        for (var i = 0; i < Amount; i++)
        {
            foreach (var phantom in phantoms)
            {
                var target = player.RunState.Rng.CombatTargets.NextItem(combatState.HittableEnemies);
                if (target != null)
                {
                    await phantom.TriggerPhantom(choiceContext, target);
                }
            }
        }
    }
}
