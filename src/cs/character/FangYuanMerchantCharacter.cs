using Godot;
using MegaCrit.Sts2.Core.Nodes.Screens.Shops;

namespace Guzhenren.Scripts;

/// <summary>
/// 商店里先用静态立绘做一个轻微呼吸动画，避免依赖 Spine 资源。
/// </summary>
public partial class FangYuanMerchantCharacter : NMerchantCharacter
{
    private Node2D? _visuals;
    private Vector2 _basePosition;
    private Tween? _idleTween;

    public override void _Ready()
    {
        _visuals = GetNodeOrNull<Node2D>("Visuals");
        if (_visuals == null)
        {
            return;
        }

        _basePosition = _visuals.Position;
        PlayAnimation("relaxed_loop", loop: true);
    }

    public new void PlayAnimation(string anim, bool loop = false)
    {
        if (_visuals == null)
        {
            return;
        }

        _idleTween?.Kill();
        _visuals.Position = _basePosition;
        _visuals.RotationDegrees = 0f;

        if (!loop)
        {
            return;
        }

        _idleTween = CreateTween();
        _idleTween.SetLoops();
        _idleTween.TweenProperty(_visuals, "position:y", _basePosition.Y - 8f, 1.6)
            .SetEase(Tween.EaseType.InOut)
            .SetTrans(Tween.TransitionType.Sine);
        _idleTween.TweenProperty(_visuals, "rotation_degrees", -1.2f, 1.6)
            .SetEase(Tween.EaseType.InOut)
            .SetTrans(Tween.TransitionType.Sine)
            .From(1.2f);
        _idleTween.TweenProperty(_visuals, "position:y", _basePosition.Y, 1.6)
            .SetEase(Tween.EaseType.InOut)
            .SetTrans(Tween.TransitionType.Sine);
        _idleTween.TweenProperty(_visuals, "rotation_degrees", 1.2f, 1.6)
            .SetEase(Tween.EaseType.InOut)
            .SetTrans(Tween.TransitionType.Sine)
            .From(-1.2f);
    }
}
