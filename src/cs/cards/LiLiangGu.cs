using System;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
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

    protected override void OnRankChanged()
    {
        base.OnRankChanged();

        var clampedRank = Math.Clamp(Rank, 1, 9);
        var gains = new[] { 0m, 2m, 2m, 3m, 3m, 4m, 5m, 6m, 7m, 8m };
        var loses = new[] { 0m, 1m, 0m, 1m, 0m, 0m, 0m, 0m, 0m, 0m };

        DynamicVars.Strength.BaseValue = gains[clampedRank];
        DynamicVars["SecondMagic"].BaseValue = loses[clampedRank];

        if (IsCanonical)
        {
            return;
        }

        if (clampedRank >= 6)
        {
            AddKeyword(CardKeyword.Innate);
        }
        else
        {
            RemoveKeyword(CardKeyword.Innate);
        }
    }

    protected override void AddExtraArgsToDescription(LocString description)
    {
        var lose = DynamicVars["SecondMagic"].IntValue;
        if (lose > 0)
        {
            description.Add("LoseLine", $"\n回合结束时失去{lose}点[gold]力量[/gold]。");
        }
        else
        {
            description.Add("LoseLine", string.Empty);
        }
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
        UpgradeRank(1);
    }
}
