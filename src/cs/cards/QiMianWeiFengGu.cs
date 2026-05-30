using System.Collections.Generic;
using System.Linq;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Guzhenren.Scripts;

[Pool(typeof(GuZhenRenCardPool))]
[GuZhenRenDaoPool(GuZhenRenDao.FengDao)]
public sealed class QiMianWeiFengGu : AbstractGuZhenRenCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(7m, ValueProp.Move),
        new CardsVar(7)
    ];

    public QiMianWeiFengGu() : base(2, CardType.Attack, CardRarity.Rare, TargetType.AllEnemies)
    {
        SetDao(GuZhenRenDao.FengDao);
        SetRank(7);
    }

    public override string PortraitPath => GuZhenRenArtPaths.GetCardPortrait("BaMianWeiFengGu");
    public override string BetaPortraitPath => GuZhenRenArtPaths.GetCardBetaPortrait("BaMianWeiFengGu");

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(CombatState, nameof(CombatState));

        var targetHandSize = DynamicVars.Cards.IntValue;
        var currentHandSize = PileType.Hand.GetPile(Owner).Cards.Count;
        var drawAmount = Math.Max(0, targetHandSize - currentHandSize);

        if (drawAmount <= 0)
            return;

        var drawn = (await CardPileCmd.Draw(choiceContext, drawAmount, Owner)).ToList();
        var uniqueDaos = new HashSet<GuZhenRenDao>();

        foreach (var card in drawn)
        {
            if (card is AbstractGuZhenRenCard guCard && guCard.PrimaryDao != GuZhenRenDao.None)
                uniqueDaos.Add(guCard.PrimaryDao);
        }

        for (var i = 0; i < uniqueDaos.Count; i++)
        {
            await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).TargetingAllOpponents(CombatState)
                .WithHitFx("vfx/vfx_attack_slash")
                .Execute(choiceContext);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3m);
        DynamicVars.Cards.UpgradeValueBy(1m);
        UpgradeRank(1);

        // 升级后标题从"七面威风蛊"变成"八面威风蛊"
        var titleField = typeof(CardModel).GetField("_titleLocString",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        titleField?.SetValue(this, new LocString("cards", Id.Entry + ".upgradeTitle"));
    }
}
