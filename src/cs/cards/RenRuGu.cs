using System.Collections.Generic;
using System.Linq;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace Guzhenren.Scripts;

[Pool(typeof(GuZhenRenCardPool))]
[GuZhenRenDaoPool(GuZhenRenDao.ZhouDao)]
public sealed class RenRuGu : AbstractGuZhenRenCard
{
    private static readonly Dictionary<ulong, List<int>> HpHistoryByNetId = new();
    private static bool _hooksRegistered;

    public override IEnumerable<CardKeyword> CanonicalKeywords =>
        IsUpgraded ? [CardKeyword.Exhaust, CardKeyword.Retain] : [CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars => [];

    public RenRuGu() : base(1, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
        SetDao(GuZhenRenDao.ZhouDao);
        SetRank(6);
    }

    private static void EnsureHooksRegistered()
    {
        if (_hooksRegistered) return;
        _hooksRegistered = true;

        CombatManager.Instance.CombatSetUp += state =>
        {
            HpHistoryByNetId.Clear();
            foreach (var player in state.Players)
                HpHistoryByNetId[player.NetId] = [player.Creature.CurrentHp];
        };

        CombatManager.Instance.CombatEnded += _ => HpHistoryByNetId.Clear();

        CombatManager.Instance.TurnEnded += state =>
        {
            if (state.CurrentSide != CombatSide.Player) return;
            foreach (var player in state.Players)
            {
                if (!HpHistoryByNetId.TryGetValue(player.NetId, out var list))
                {
                    list = [];
                    HpHistoryByNetId[player.NetId] = list;
                }

                list.Add(player.Creature.CurrentHp);
            }
        };
    }

    private int CalculateTargetHp()
    {
        EnsureHooksRegistered();

        if (CombatState == null || Owner == null)
            return Owner?.Creature.CurrentHp ?? 0;

        if (!HpHistoryByNetId.TryGetValue(Owner.NetId, out var history) || history.Count == 0)
            return Owner.Creature.CurrentHp;

        var targetIndex = Math.Max(0, CombatState.RoundNumber - 2);
        if (targetIndex >= history.Count)
            targetIndex = history.Count - 1;

        return history[targetIndex];
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var creature = Owner.Creature;
        int targetHp = CalculateTargetHp();

        if (targetHp != creature.CurrentHp)
        {
            await CreatureCmd.SetCurrentHp(creature, targetHp);
        }
    }

    protected override void OnUpgrade()
    {
        if (!IsCanonical)
        {
            AddKeyword(CardKeyword.Retain);
        }

        UpgradeRank(1);
    }
}
