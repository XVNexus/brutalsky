using System;
using Brutalsky.Scripts.Data;
using Brutalsky.Scripts.Extensions;
using Brutalsky.Scripts.Systems;
using Brutalsky.Scripts.Utils;
using Godot;
using Godot.Collections;

namespace Brutalsky.Scripts.Controllers;

public partial class PlayerController : RigidBody2D
{
    // Controller metadata
    public BsPlayer Player { get; set; }

    // External references
    [Export] public NodePath BodyCollider { get; set; }
    private CollisionShape2D _bodyCollider;
    [Export] public NodePath BodySprite { get; set; }
    private Sprite2D _bodySprite;
    [Export] public NodePath PointLight { get; set; }
    private PointLight2D _pointLight;
    [Export] public NodePath RingSprite { get; set; }
    private Sprite2D _ringSprite;
    [Export] public NodePath RingMask { get; set; }
    private Sprite2D _ringMask;

    // Config settings
    [Export] public float MovementForce { get; set; }
    [Export] public float JumpForce { get; set; }
    [Export] public float GroundSensitivity { get; set; }
    [Export] public int MaxGroundedFrames { get; set; }

    // Exposed properties
    public float MaxHealth { get; private set; }
    public float Health { get; private set; } = -1f;
    public bool Alive { get; private set; } = true;
    public float BoostCharge { get; private set; }
    public float BoostCooldown { get; private set; }

    // Local variables
    private string[]? _inputMap;
    private Vector2 _movementInput;
    private bool _boostInput;
    private bool _grounded;
    private int _groundedFrames;
    private float _groundFriction = 2f; // TODO: MAKE THIS WORK
    private int _jumpCooldown;
    private Vector2 _movementScale;
    private Vector2 _jumpVector;
    private Func<Vector2, Vector2, bool> _testGrounded = (_, _) => false;
    private Func<Vector2, bool> _testJumpInput = _ => false;
    private float _ringAlpha;
    private float _ringThickness;
    private float _ringSpin;

    private bool _lastBoostInput;
    private Vector2 _lastPosition;
    private float _lastSpeed;

    // Init functions
    public override void _Ready()
    {
        EventSystem.Inst.OnMapBuild += OnMapBuild;
        Connect("body_entered", new Callable(this, MethodName.OnBodyEntered));

        _bodyCollider = GetNode<CollisionShape2D>(BodyCollider);
        _bodySprite = GetNode<Sprite2D>(BodySprite);
        _pointLight = GetNode<PointLight2D>(PointLight);
        _ringSprite = GetNode<Sprite2D>(RingSprite);
        _ringMask = GetNode<Sprite2D>(RingMask);

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
        MaxHealth = Player.Health;
        Health = MaxHealth;
        Revive();
        LinearVelocity = Vector2.Zero;
        BoostCharge = 0f;
        BoostCooldown = 0f;
        _ringAlpha = 0f;
        _ringThickness = 0f;
        _ringSpin = 0f;
        _ringSprite.Rotation = 0f;
        EventSystem.Inst.EmitPlayerSpawn(Player);
    }

    public void Heal(float amount)
    {
        Health = Mathf.Min(Health + amount, MaxHealth);
    }

    public void Hurt(float amount)
    {
        if (amount >= Health)
        {
            CallDeferred(MethodName.Kill);
            return;
        }
        Health = Mathf.Max(Health - amount, 0f);
    }

    public void Revive()
    {
        if (Alive) return;
        Health = MaxHealth;
        Alive = true;
        Freeze = false;
        _bodyCollider.Disabled = false;
        _bodySprite.Visible = true;
        _ringSprite.Visible = true;
    }

    public void Kill()
    {
        if (!Alive) return;
        Health = 0f;
        Alive = false;
        Freeze = true;
        _bodyCollider.Disabled = true;
        _bodySprite.Visible = false;
        _ringSprite.Visible = false;
        EventSystem.Inst.EmitPlayerDie(Player);
    }

    // Event functions
    private void OnMapBuild(BsMap map)
    {
        // Configure player to fit current map settings
        MaxHealth = map.PlayerHealth;
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

    private void OnBodyEntered(Node body)
    {
        if (!Alive) return;

        // Get collision info
        var impactForce = (LinearVelocity.Length() * GameManager.Mpp - _lastSpeed) * (body.IsInGroup(Tags.Player) ? 2f : 1f);
        var impactSpeed = _lastSpeed;

        GD.Print($"force: {impactForce}  /  speed: {impactSpeed}");

        // Apply damage to player
        var damageApplied = CalculateDamage(impactForce);
        var damageMultiplier = Mathf.Min(1f / (impactSpeed * 0.2f), 1f);
        Hurt(damageApplied * damageMultiplier);
    }

    public override void _Process(double delta)
    {
        var deltaF = (float)delta;

        // Calculate target ring properties
        var targetRingThickness = Health / MaxHealth;
        var targetRingAlpha = .25f;
        var targetRingSpin = 40f;
        if (BoostCharge > 0f)
        {
            targetRingAlpha = .25f + BoostCharge * .25f;
            targetRingSpin = (Mathf.Pow(BoostCharge, 1.5f) + 1.5f) * 360f;
        }
        else if (BoostCooldown > 0f)
        {
            targetRingAlpha = .05f;
            targetRingSpin = 10f;
        }

        // Transition current ring properties to calculated target properties
        _ringThickness = MathfExt.MoveToExponential(_ringThickness, targetRingThickness, 5f * deltaF);
        _ringAlpha = MathfExt.MoveToLinear(_ringAlpha, targetRingAlpha, deltaF);
        _ringSpin = MathfExt.MoveToLinear(_ringSpin, targetRingSpin, 1440f * deltaF);

        // Apply current ring properties
        _ringMask.Scale = Vector2.One * (.6f + _ringThickness * .4f);
        _ringMask.Modulate = _ringMask.Modulate.SetAlpha(_ringAlpha);
        _ringSprite.RotationDegrees -= _ringSpin * deltaF;
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
        var speed = LinearVelocity.Length() * GameManager.Mpp;

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
        var boostInput = _boostInput && BoostCooldown == 0f;
        if (boostInput)
        {
            BoostCharge = Mathf.Min(BoostCharge + (float)delta, 3f);
        }
        else if (_lastBoostInput)
        {
            var boost = Mathf.Pow(BoostCharge, 1.5f) + 1.5f;
            LinearVelocity *= boost;
            BoostCharge = 0f;
            BoostCooldown = Mathf.Min(boost, speed);
        }
        if (BoostCooldown > 0f)
        {
            BoostCooldown = Mathf.Max(BoostCooldown - (float)delta, 0f);
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

    // Utility functions
    public static float CalculateDamage(float impactForce)
    {
        return Mathf.Max(impactForce - 20f, 0) * .5f;
    }
}
