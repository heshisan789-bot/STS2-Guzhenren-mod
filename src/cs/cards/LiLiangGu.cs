using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Guzhenren.Scripts;

[Pool(typeof(GuZhenRenCardPool))]
[GuZhenRenDaoPool(GuZhenRenDao.LiDao)]
public sealed class LiLiangGu : AbstractBenMingGuCard
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<StrengthPower>(2m),
        new DynamicVar("SecondMagic", 1m)
    ];

    public LiLiangGu() : base(0, CardType.Skill, CardRarity.Token, TargetType.Self)
    {
        SetDao(GuZhenRenDao.LiDao);
        SetRank(1);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<StrengthPower>(Owner.Creature, DynamicVars.Strength.BaseValue, Owner.Creature, this);

        if (DynamicVars["SecondMagic"].BaseValue > 0)
        {
            await PowerCmd.Apply<LiLiangGuStrengthDownPower>(Owner.Creature, DynamicVars["SecondMagic"].BaseValue, Owner.Creature, this);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Strength.UpgradeValueBy(1);
        DynamicVars["SecondMagic"].UpgradeValueBy(-1);
        UpgradeRank(1);
    }
}
