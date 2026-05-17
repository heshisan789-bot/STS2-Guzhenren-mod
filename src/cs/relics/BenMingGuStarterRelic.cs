using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Runs;

namespace Guzhenren.Scripts;

[Pool(typeof(GuZhenRenRelicPool))]
public sealed class BenMingGuStarterRelic : CustomRelicModel
{
    public override RelicRarity Rarity => RelicRarity.Starter;
    public override bool IsAllowedInShops => false;

    public override string PackedIconPath => GuZhenRenArtPaths.GetRelicIcon("KongQiao_1");
    protected override string PackedIconOutlinePath => GuZhenRenArtPaths.GetRelicOutline("KongQiao_1");
    protected override string BigIconPath => GuZhenRenArtPaths.GetRelicIcon("KongQiao_1");

    public override bool IsAllowed(IRunState runState)
    {
        return false;
    }
}
