using System;

namespace Guzhenren.Scripts;

[Flags]
public enum GuZhenRenBannedCardSources
{
    None = 0,
    Reward = 1 << 0,
    Shop = 1 << 1,
    PotionChoice = 1 << 2,
    Event = 1 << 3,
    All = Reward | Shop | PotionChoice | Event
}

