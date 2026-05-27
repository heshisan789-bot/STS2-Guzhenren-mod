using BaseLib.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Guzhenren.Scripts;

[Pool(typeof(GuZhenRenCardPool))]
[GuZhenRenDaoPool(GuZhenRenDao.XueDao)]
public sealed class ZhuMoBang : AbstractXianGuWuCard
{
    private const decimal FangHuBlock = 8m;
    private const decimal GongFaDamage = 18m;
    private const int HuiFuSiphonPerEnemy = 5;

    private static bool _usedHuiFuThisCombat;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<XueYuanPower>(),
        HoverTipFactory.FromPower<XueYuanMarkPower>(),
        HoverTipFactory.FromPower<XueDianXingBanPower>(),
        HoverTipFactory.FromCard<OptionZhenChaZhuMoBang>(),
        HoverTipFactory.FromCard<OptionFangHuZhuMoBang>(),
        HoverTipFactory.FromCard<OptionGongFaZhuMoBang>(),
        HoverTipFactory.FromCard<OptionHuiFuZhuMoBang>()
    ];

    static ZhuMoBang()
    {
        BattleStateManager.OnBattleStart(() => _usedHuiFuThisCombat = false);
        BattleStateManager.OnPostBattle(() => _usedHuiFuThisCombat = false);
    }

    public ZhuMoBang() : base(1, CardType.Skill, TargetType.None)
    {
        SetDao(GuZhenRenDao.XueDao);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(CombatState, nameof(CombatState));

        var choices = new List<CardModel>
        {
            CombatState.CreateCard<OptionZhenChaZhuMoBang>(Owner),
            CombatState.CreateCard<OptionFangHuZhuMoBang>(Owner),
            CombatState.CreateCard<OptionGongFaZhuMoBang>(Owner)
        };

        if (!_usedHuiFuThisCombat)
        {
            choices.Add(CombatState.CreateCard<OptionHuiFuZhuMoBang>(Owner));
        }

        var selected = (await CardSelectCmd.FromSimpleGrid(
            choiceContext,
            choices,
            Owner,
            new CardSelectorPrefs(SelectionScreenPrompt, 1))).FirstOrDefault();
        switch (selected)
        {
            case OptionZhenChaZhuMoBang:
                await ApplyZhenCha(CombatState);
                break;
            case OptionFangHuZhuMoBang:
                await ApplyFangHu(cardPlay);
                break;
            case OptionGongFaZhuMoBang:
                await ApplyGongFa(choiceContext, CombatState);
                break;
            case OptionHuiFuZhuMoBang:
                _usedHuiFuThisCombat = true;
                await ApplyHuiFu(CombatState);
                break;
        }
    }

    private async Task ApplyZhenCha(MegaCrit.Sts2.Core.Combat.CombatState combatState)
    {
        if (Owner.Creature.GetPower<XueYuanPower>() == null)
        {
            await PowerCmd.Apply<XueYuanPower>(Owner.Creature, -1m, Owner.Creature, this);
        }

        foreach (var enemy in combatState.HittableEnemies)
        {
            await PowerCmd.Apply<XueYuanMarkPower>(enemy, 1m, Owner.Creature, this);
        }

        if (!combatState.HittableEnemies.Any(enemy => enemy.GetPower<XueYuanMarkPower>() != null))
        {
            var bond = Owner.Creature.GetPower<XueYuanPower>();
            if (bond != null)
            {
                await PowerCmd.Remove(bond);
            }
        }
    }

    private async Task ApplyFangHu(CardPlay cardPlay)
    {
        await CreatureCmd.GainBlock(Owner.Creature, FangHuBlock, ValueProp.Move, cardPlay);
        if (Owner.Creature.GetPower<XueDianXingBanPower>() == null)
        {
            await PowerCmd.Apply<XueDianXingBanPower>(Owner.Creature, -1m, Owner.Creature, this);
        }
    }

    private async Task ApplyGongFa(PlayerChoiceContext choiceContext, MegaCrit.Sts2.Core.Combat.CombatState combatState)
    {
        foreach (var enemy in combatState.HittableEnemies)
        {
            var hits = enemy.GetPowerAmount<XueYuanMarkPower>();
            for (var i = 0; i < hits; i++)
            {
                await CreatureCmd.Damage(choiceContext, enemy, GongFaDamage, ValueProp.Move, Owner.Creature, this);
            }
        }
    }

    private async Task ApplyHuiFu(MegaCrit.Sts2.Core.Combat.CombatState combatState)
    {
        var totalHeal = 0m;
        foreach (var enemy in combatState.HittableEnemies.Where(enemy => enemy.GetPower<XueYuanMarkPower>() != null))
        {
            var siphonAmount = Math.Min(HuiFuSiphonPerEnemy, enemy.CurrentHp);
            if (siphonAmount <= 0)
            {
                continue;
            }

            await CreatureCmd.SetCurrentHp(enemy, enemy.CurrentHp - siphonAmount);
            totalHeal += siphonAmount;
        }

        if (totalHeal > 0m)
        {
            await CreatureCmd.Heal(Owner.Creature, totalHeal);
        }
    }
}

[Pool(typeof(GuZhenRenCardPool))]
public sealed class OptionZhenChaZhuMoBang : AbstractGuZhenRenCard
{
    public override GuZhenRenBannedCardSources BannedSources => GuZhenRenBannedCardSources.All;
    public override string PortraitPath => GuZhenRenArtPaths.GetCardPortrait(nameof(ZhuMoBang));
    public override string BetaPortraitPath => GuZhenRenArtPaths.GetCardBetaPortrait(nameof(ZhuMoBang));

    public OptionZhenChaZhuMoBang() : base(-2, CardType.Skill, CardRarity.Token, TargetType.None)
    {
        SetRank(0);
    }

    protected override Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay) => Task.CompletedTask;
}

[Pool(typeof(GuZhenRenCardPool))]
public sealed class OptionFangHuZhuMoBang : AbstractGuZhenRenCard
{
    public override bool GainsBlock => true;
    public override GuZhenRenBannedCardSources BannedSources => GuZhenRenBannedCardSources.All;
    public override string PortraitPath => GuZhenRenArtPaths.GetCardPortrait(nameof(ZhuMoBang));
    public override string BetaPortraitPath => GuZhenRenArtPaths.GetCardBetaPortrait(nameof(ZhuMoBang));

    public OptionFangHuZhuMoBang() : base(-2, CardType.Skill, CardRarity.Token, TargetType.Self)
    {
        SetRank(0);
    }

    protected override Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay) => Task.CompletedTask;
}

[Pool(typeof(GuZhenRenCardPool))]
public sealed class OptionGongFaZhuMoBang : AbstractGuZhenRenCard
{
    public override GuZhenRenBannedCardSources BannedSources => GuZhenRenBannedCardSources.All;
    public override string PortraitPath => GuZhenRenArtPaths.GetCardPortrait(nameof(ZhuMoBang));
    public override string BetaPortraitPath => GuZhenRenArtPaths.GetCardBetaPortrait(nameof(ZhuMoBang));

    public OptionGongFaZhuMoBang() : base(-2, CardType.Skill, CardRarity.Token, TargetType.AllEnemies)
    {
        SetRank(0);
    }

    protected override Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay) => Task.CompletedTask;
}

[Pool(typeof(GuZhenRenCardPool))]
public sealed class OptionHuiFuZhuMoBang : AbstractGuZhenRenCard
{
    public override GuZhenRenBannedCardSources BannedSources => GuZhenRenBannedCardSources.All;
    public override string PortraitPath => GuZhenRenArtPaths.GetCardPortrait(nameof(ZhuMoBang));
    public override string BetaPortraitPath => GuZhenRenArtPaths.GetCardBetaPortrait(nameof(ZhuMoBang));

    public OptionHuiFuZhuMoBang() : base(-2, CardType.Skill, CardRarity.Token, TargetType.None)
    {
        SetRank(0);
    }

    protected override Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay) => Task.CompletedTask;
}
