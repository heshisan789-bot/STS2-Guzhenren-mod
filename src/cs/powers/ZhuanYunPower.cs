using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;

namespace Guzhenren.Scripts;

public sealed class ZhuanYunPower : AbstractGuZhenRenPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override bool AllowNegative => false;

    public void OnProbabilityRollFailed(CardModel card)
    {
        if (Amount <= 0 || card is not IGuZhenRenProbabilityCard probabilityCard)
        {
            return;
        }

        Flash();
        probabilityCard.IncreaseBaseChance(Amount / 100m);
    }
}
