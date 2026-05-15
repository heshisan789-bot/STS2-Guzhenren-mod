using BaseLib.Abstracts;
using Godot;
using MegaCrit.Sts2.Core.Entities.Characters;
using MegaCrit.Sts2.Core.Models;

namespace Guzhenren.Scripts;

/// <summary>
/// 方源角色骨架。
/// 先跑通角色注册、卡池与起始内容，视觉资源后续替换成正式素材。
/// </summary>
public sealed class FangYuanCharacter : PlaceholderCharacterModel
{
    public override Color NameColor => new(0.72f, 0.72f, 0.76f);
    public override Color EnergyLabelOutlineColor => new(0.15f, 0.15f, 0.15f);
    public override CharacterGender Gender => CharacterGender.Masculine;
    protected override CharacterModel? UnlocksAfterRunAs => null;

    public override int StartingHp => 80;
    public override int StartingGold => 99;

    public override string CustomVisualPath => "res://guzhenren/scenes/fang_yuan_character.tscn";
    public override string CustomIconTexturePath => "res://guzhenren/images/character/FangYuan/TisIcon.png";
    public override string CustomIconPath => "res://guzhenren/scenes/fang_yuan_icon.tscn";
    public override string CustomEnergyCounterPath => "res://guzhenren/scenes/fang_yuan_energy_counter.tscn";
    public override string CustomRestSiteAnimPath => "res://guzhenren/scenes/fang_yuan_rest_site.tscn";
    public override string CustomMerchantAnimPath => "res://guzhenren/scenes/fang_yuan_merchant.tscn";
    public override string CustomCharacterSelectBg => "res://guzhenren/scenes/fang_yuan_bg.tscn";
    public override string CustomCharacterSelectIconPath => "res://guzhenren/images/character_select/char_select_fang_yuan.png";
    public override string CustomCharacterSelectLockedIconPath => "res://guzhenren/images/character_select/char_select_fang_yuan_locked.png";
    public override string CustomCharacterSelectTransitionPath => "res://materials/transitions/ironclad_transition_mat.tres";
    public override string CustomMapMarkerPath => "res://guzhenren/images/map_marker_fang_yuan.png";
    public override string CharacterSelectSfx => "event:/sfx/characters/ironclad/ironclad_select";
    public override string CharacterTransitionSfx => "event:/sfx/ui/wipe_ironclad";

    public override float AttackAnimDelay => 0.15f;
    public override float CastAnimDelay => 0.20f;

    public override CardPoolModel CardPool => ModelDb.CardPool<GuZhenRenCardPool>();
    public override RelicPoolModel RelicPool => ModelDb.RelicPool<GuZhenRenRelicPool>();
    public override PotionPoolModel PotionPool => ModelDb.PotionPool<GuZhenRenPotionPool>();

    public override IEnumerable<CardModel> StartingDeck =>
    [
        ModelDb.Card<YueGuangGu>(),
        ModelDb.Card<YueGuangGu>(),
        ModelDb.Card<YueGuangGu>(),
        ModelDb.Card<YueGuangGu>(),
        ModelDb.Card<YuPiGu>(),
        ModelDb.Card<YuPiGu>(),
        ModelDb.Card<YuPiGu>(),
        ModelDb.Card<YuPiGu>(),
        ModelDb.Card<XiaoGuangGu>(),
    ];

    public override IReadOnlyList<RelicModel> StartingRelics =>
    [
        ModelDb.Relic<KongQiao1>()
    ];

    public override List<string> GetArchitectAttackVfx() =>
    [
        "vfx/vfx_attack_blunt",
        "vfx/vfx_heavy_blunt",
        "vfx/vfx_attack_slash"
    ];
}
