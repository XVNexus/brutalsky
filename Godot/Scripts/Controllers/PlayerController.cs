using Brutalsky.Scripts.Data;
using Godot;

namespace Brutalsky.Scripts.Controllers;

public partial class PlayerController : RigidBody2D
{
    // Controller metadata
    public BsPlayer Player { get; set; }

    // External references
    [Export] public NodePath Collider { get; set; }
    private CollisionShape2D _collider;
    [Export] public NodePath Sprite { get; set; }
    private Sprite2D _sprite;
    [Export] public NodePath Light { get; set; }
    private PointLight2D _light;

    // Config settings
    [Export] public float MovementForce { get; set; }
    [Export] public float JumpForce { get; set; }

    // Local variables
    private Vector2 _movementInput;
    private bool _boostInput;

    // Init functions
    public override void _Ready()
    {
        _collider = GetNode<CollisionShape2D>(Collider);
        _sprite = GetNode<Sprite2D>(Sprite);
        _light = GetNode<PointLight2D>(Light);
    }

    // Event functions
    public override void _PhysicsProcess(double delta)
    {
        _movementInput = Input.GetVector("player_left", "player_right", "player_up", "player_down");
        _boostInput = Input.IsActionPressed("player_boost");
    }

    public override void _IntegrateForces(PhysicsDirectBodyState2D state)
    {
        state.ApplyForce(_movementInput * MovementForce);
    }
}
