using Godot;

namespace Brutalsky.Scripts.Node;

public partial class Player : RigidBody2D
{
	public override void _PhysicsProcess(double delta)
	{
		var iMovement = Input.GetVector("player_left", "player_right", "player_down", "player_up");
		var iBoost = Input.IsActionPressed("player_boost");
		GD.Print($"{Name}  //  movement: {iMovement}  //  boost: {iBoost}");
	}
}
