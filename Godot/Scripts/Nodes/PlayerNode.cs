using Godot;

namespace Brutalsky.Scripts.Nodes;

public partial class PlayerNode : RigidBody2D
{
	// User input
	private Vector2 _iMovement;
	private bool _iBoost;

	// Event functions
	public override void _PhysicsProcess(double delta)
	{
		_iMovement = Input.GetVector("player_left", "player_right", "player_down", "player_up");
		_iBoost = Input.IsActionPressed("player_boost");
	}

	public override void _IntegrateForces(PhysicsDirectBodyState2D state)
	{
	}
}
