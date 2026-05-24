using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.RestSite;
using MegaCrit.Sts2.Core.Models;

namespace Guzhenren.Scripts;

public sealed class GuZhenRenShaZhaoSynthesisSystem : AbstractModel
{
    public override bool ShouldReceiveCombatHooks => false;

    public override bool TryModifyRestSiteOptions(Player player, ICollection<RestSiteOption> options)
    {
        if (player.Character is not FangYuanCharacter)
        {
            return false;
        }

        options.Add(new ShaZhaoCraftRestSiteOption(player));
        return true;
    }
}
