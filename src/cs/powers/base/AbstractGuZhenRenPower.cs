using BaseLib.Abstracts;

namespace Guzhenren.Scripts;

public abstract class AbstractGuZhenRenPower : CustomPowerModel
{
    public override string? CustomPackedIconPath => GuZhenRenArtPaths.GetPowerPackedIcon(GetType().Name);
    public override string? CustomBigIconPath => GuZhenRenArtPaths.GetPowerBigIcon(GetType().Name);
}
