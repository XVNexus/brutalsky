using System;
using Brutalsky.Scripts.Data;
using Brutalsky.Scripts.Systems;
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
    [Export] public float GroundSensitivity { get; set; }
    [Export] public int MaxGroundedFrames { get; set; }

    // Local variables
    private string[]? _inputMap;
    private Vector2 _movementInput;
    private bool _boostInput;
    private bool _grounded;
    private int _groundedFrames;
    private float _groundFriction = 2f; // TODO: MAKE THIS WORK
    private int _jumpCooldown;
    private float _boostCharge;
    private float _boostCooldown;
    private bool _lastBoostInput;
    private float _lastSpeed;
    private Vector2 _lastPosition;
    public Vector2 _movementScale;
    public Vector2 _jumpVector;
    public Func<Vector2, Vector2, bool> _testGrounded = (_, _) => false;
    public Func<Vector2, bool> _testJumpInput = _ => false;

    // Init functions
    public override void _Ready()
    {
        EventSystem.Inst.OnMapBuild += OnMapBuild;

        _collider = GetNode<CollisionShape2D>(Collider);
        _sprite = GetNode<Sprite2D>(Sprite);
        _light = GetNode<PointLight2D>(Light);

        _inputMap = new[] { "player_left", "player_right", "player_up", "player_down", "player_boost" };

        // TODO: TEMPORARY
        OnMapBuild(new BsMap
        {
            InitialGravity = new Vector2(0f, 20f)
        });
    }

    public override void _ExitTree()
    {
        EventSystem.Inst.OnMapBuild -= OnMapBuild;
    }

    // Controller functions
    public void Reset()
    {
        LinearVelocity = Vector2.Zero;
        _boostCharge = 0f;
        _boostCooldown = 0f;
    }

    // Event functions
    private void OnMapBuild(BsMap map)
    {
        // Configure movement to fit with current map settings
        if (map.InitialGravity.Y < 0f)
        {
            _movementScale = new Vector2(MovementForce, map.InitialGravity.Length() * .5f);
            _jumpVector = Vector2.Down * JumpForce;
            _testGrounded = (point, position) => point.Y < position.Y - GroundSensitivity;
            _testJumpInput = movementInput => movementInput.Y > 0f;
        }
        else if (map.InitialGravity.Y > 0f)
        {
            _movementScale = new Vector2(MovementForce, map.InitialGravity.Length() * .5f);
            _jumpVector = Vector2.Up * JumpForce;
            _testGrounded = (point, position) => point.Y > position.Y + GroundSensitivity;
            _testJumpInput = movementInput => movementInput.Y < 0f;
        }
        else if (map.InitialGravity.X < 0f)
        {
            _movementScale = new Vector2(map.InitialGravity.Length() * .5f, MovementForce);
            _jumpVector = Vector2.Right * JumpForce;
            _testGrounded = (point, position) => point.X < position.X - GroundSensitivity;
            _testJumpInput = movementInput => movementInput.X > 0f;
        }
        else if (map.InitialGravity.X > 0f)
        {
            _movementScale = new Vector2(map.InitialGravity.Length() * .5f, MovementForce);
            _jumpVector = Vector2.Left * JumpForce;
            _testGrounded = (point, position) => point.X > position.X + GroundSensitivity;
            _testJumpInput = movementInput => movementInput.X < 0f;
        }
        else
        {
            _movementScale = new Vector2(MovementForce, MovementForce);
            _jumpVector = Vector2.Zero;
            _testGrounded = (point, position) => point.Y < position.Y - .25f;
            _testJumpInput = _ => false;
        }
        LinearDamp = map.InitialAtmosphere;
    }

    public override void _PhysicsProcess(double delta)
    {
        // Get user input
        if (_inputMap != null)
        {
            _movementInput = Input.GetVector(_inputMap[0], _inputMap[1], _inputMap[2], _inputMap[3]);
            _boostInput = Input.IsActionPressed(_inputMap[4]);
        }

        // Update ground status
        _groundedFrames = Mathf.Max(_groundedFrames - 1, 0);
        _grounded = _groundedFrames > 0;

        // Get movement data
        var speed = LinearVelocity.Length();

        // Apply jump movement
        var jumpInput = _grounded && _testJumpInput(_movementInput) && _jumpCooldown == 0;
        if (jumpInput)
        {
            ApplyImpulse(_jumpVector);
            _jumpCooldown = MaxGroundedFrames;
        }
        if (_jumpCooldown > 0)
        {
            _jumpCooldown--;
        }

        // Apply boost movement
        var boostInput = _boostInput && _boostCooldown == 0f;
        if (boostInput)
        {
            _boostCharge = Mathf.Min(_boostCharge + (float)delta, 3f);
        }
        else if (_lastBoostInput)
        {
            var boost = Mathf.Pow(_boostCharge, 1.5f) + 1.5f;
            LinearVelocity *= boost;
            _boostCharge = 0f;
            _boostCooldown = Mathf.Min(boost, speed);
        }
        if (_boostCooldown > 0f)
        {
            _boostCooldown = Mathf.Max(_boostCooldown - (float)delta, 0f);
        }
        _lastBoostInput = boostInput;

        // Save the current position and speed for future reference
        _lastPosition = Position;
        _lastSpeed = speed;
    }

    public override void _IntegrateForces(PhysicsDirectBodyState2D state)
    {
        // Apply directional movement
        state.ApplyForce(_movementInput * _movementScale * (_grounded ? Mathf.Clamp(_groundFriction, .5f, 2f) : .5f));

        // Update ground status
        var contactCount = GetContactCount();
        if (contactCount <= 0) return;
        for (var i = 0; i < contactCount; i++)
        {
            if (_testGrounded(state.GetContactColliderPosition(i), _lastPosition))
            {
                _groundedFrames = MaxGroundedFrames;
                _grounded = true;
            }
        }
    }
}
