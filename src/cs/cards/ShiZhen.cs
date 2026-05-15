using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace Guzhenren.Scripts;

[Pool(typeof(GuZhenRenCardPool))]
[GuZhenRenDaoPool(GuZhenRenDao.ZhouDao)]
public sealed class ShiZhen : AbstractGuZhenRenCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(4m, ValueProp.Move),
        new BlockVar(4m, ValueProp.Move),
        new DynamicVar("Growth", 1m)
    ];

    public override bool GainsBlock => true;

    public ShiZhen() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
        SetDao(GuZhenRenDao.ZhouDao);
        SetRank(6);
        Drawn += OnCardDrawn;
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, nameof(cardPlay.Target));
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, cardPlay);
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["Growth"].UpgradeValueBy(1m);
        UpgradeRank(1);
    }

    private void OnCardDrawn()
    {
        DynamicVars.Damage.BaseValue += DynamicVars["Growth"].BaseValue;
        DynamicVars.Block.BaseValue += DynamicVars["Growth"].BaseValue;
    }
}
