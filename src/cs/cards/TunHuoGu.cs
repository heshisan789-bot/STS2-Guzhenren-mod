using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace Guzhenren.Scripts;

[Pool(typeof(GuZhenRenCardPool))]
[GuZhenRenDaoPool(GuZhenRenDao.YanDao)]
public sealed class TunHuoGu : AbstractGuZhenRenCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<TunHuoPower>(1m)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromCard<HuoShi>()];

    public TunHuoGu() : base(2, CardType.Power, CardRarity.Uncommon, TargetType.Self)
    {
        SetDao(GuZhenRenDao.YanDao);
        SetRank(4);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(CombatState, nameof(CombatState));

        if (IsUpgraded)
        {
            await CardPileCmd.AddGeneratedCardToCombat(CombatState.CreateCard<HuoShi>(Owner), PileType.Hand, addedByPlayer: true);
            await CardPileCmd.AddGeneratedCardToCombat(CombatState.CreateCard<HuoShi>(Owner), PileType.Hand, addedByPlayer: true);
        }

        await PowerCmd.Apply<TunHuoPower>(Owner.Creature, DynamicVars["TunHuoPower"].BaseValue, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        UpgradeRank(1);
    }
}

