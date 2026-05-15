using System.Linq;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace Guzhenren.Scripts;

[Pool(typeof(GuZhenRenCardPool))]
[GuZhenRenDaoPool(GuZhenRenDao.JianDao)]
public sealed class ShanJianGu : AbstractGuZhenRenCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(8m, ValueProp.Move),
        new CardsVar(3)
    ];

    public ShanJianGu()
        : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
        SetDao(GuZhenRenDao.JianDao);
        SetRank(3);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, nameof(cardPlay.Target));
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
        var drawn = (await CardPileCmd.Draw(choiceContext, DynamicVars.Cards.BaseValue, Owner)).ToList();
        var nonAttacks = drawn.Where(c => c.Type != CardType.Attack).ToList();
        if (nonAttacks.Count > 0)
        {
            await CardCmd.Discard(choiceContext, nonAttacks);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(1m);
        DynamicVars.Cards.UpgradeValueBy(2m);
        UpgradeRank(1);
    }
}
