using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Saves.Runs;

namespace Guzhenren.Scripts;

[Pool(typeof(GuZhenRenRelicPool))]
public sealed class LiDaoDaoHen : CustomRelicModel
{
    private int _counter;

    public override RelicRarity Rarity => RelicRarity.Event;

    public override bool ShowCounter => true;

    public override int DisplayAmount => Counter;

    [SavedProperty]
    public int Counter
    {
        get => _counter;
        set
        {
            AssertMutable();
            _counter = Math.Max(0, value);
        }
    }

    public override string PackedIconPath => GuZhenRenArtPaths.GetRelicIcon("LiDaoDaoHen");
    protected override string PackedIconOutlinePath => GuZhenRenArtPaths.GetRelicOutline("LiDaoDaoHen");
    protected override string BigIconPath => GuZhenRenArtPaths.GetRelicIcon("LiDaoDaoHen");

    public override Task AfterObtained()
    {
        if (IsMutable && Counter <= 0)
        {
            Counter = 0;
        }

        return Task.CompletedTask;
    }

    public override async Task BeforeCombatStart()
    {
        if (Counter <= 0)
        {
            return;
        }

        Flash();
        await PowerCmd.Apply<StrengthPower>(Owner.Creature, Counter, Owner.Creature, null);
    }
}
