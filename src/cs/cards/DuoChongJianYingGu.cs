using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace Guzhenren.Scripts;

[Pool(typeof(GuZhenRenCardPool))]
[GuZhenRenDaoPool(GuZhenRenDao.JianDao)]
public sealed class DuoChongJianYingGu : AbstractGuZhenRenCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new CardsVar(3)];

    public DuoChongJianYingGu() : base(1, CardType.Skill, CardRarity.Common, TargetType.None)
    {
        SetDao(GuZhenRenDao.JianDao);
        SetRank(4);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(CombatState, nameof(CombatState));

        for (var i = 0; i < DynamicVars.Cards.IntValue; i++)
        {
            var generated = CombatState.CreateCard<JianYing>(Owner);
            if (IsUpgraded)
            {
                generated.UpgradeInternal();
                generated.FinalizeUpgradeInternal();
            }

            await CardPileCmd.AddGeneratedCardToCombat(generated, PileType.Hand, addedByPlayer: true);
        }
    }

    protected override void OnUpgrade()
    {
        UpgradeRank(1);
    }
}
