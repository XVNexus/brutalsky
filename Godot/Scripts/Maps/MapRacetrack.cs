using Brutalsky.Scripts.Data;
using Brutalsky.Scripts.Extensions;
using Brutalsky.Scripts.Utils;
using Godot;

namespace Brutalsky.Scripts.Maps;

public class MapRacetrack : MapGenerator
{
    public override BsMap Generate()
    {
        // Create map
        var result = new BsMap("Racetrack", AuthorBuiltin)
        {
            PlayArea = new Rect2(-1250f, 0f, 2500f, 250f),
            InitialGravity = new Vector2(0f, -20f),
            InitialAtmosphere = .5f
        };

        // Add spawns
        result.Spawns.Add(new BsSpawn(new Vector2(-5f, 11f), 1));
        result.Spawns.Add(new BsSpawn(new Vector2(-3f, 11f), 0));
        result.Spawns.Add(new BsSpawn(new Vector2(3f, 11f), 0));
        result.Spawns.Add(new BsSpawn(new Vector2(5f, 11f), 1));

        // Add objects
        result.Objects.Add(new BsShape("wall-bottom")
        {
            Position = new Vector2(0f, -5f),
            Path = Path.Rectangle(2500f, 12f),
            Material = MaterialExt.Stone,
            Color = ColorExt.Stone
        });
        result.Objects.Add(new BsShape("wall-left")
        {
            Position = new Vector2(-1255f, 10f),
            Path = Path.Rectangle(12f, 42f),
            Material = MaterialExt.Stone,
            Color = ColorExt.Stone
        });
        result.Objects.Add(new BsShape("wall-right")
        {
            Position = new Vector2(1255f, 10f),
            Path = Path.Rectangle(12f, 42f),
            Material = MaterialExt.Stone,
            Color = ColorExt.Stone
        });
        result.Objects.Add(new BsShape("corner-bl")
        {
            Position = new Vector2(-1249f, 1f),
            Path = Path.Vector(0f, 0f, 0f, 0f, 20f, 1f, 0f, 0f, 20f, 0f),
            Material = MaterialExt.Stone,
            Color = ColorExt.Stone
        });
        result.Objects.Add(new BsShape("corner-br")
        {
            Position = new Vector2(1249f, 1f),
            Path = Path.Vector(0f, 0f, 0f, 0f, 20f, 1f, 0f, 0f, -20f, 0f),
            Material = MaterialExt.Stone,
            Color = ColorExt.Stone
        });
        result.Objects.Add(new BsShape("car-body")
        {
            Position = new Vector2(0f, 10f),
            Path = Path.Vector(-6f, .5f, 0f, -3f, .5f, 0f, -2.5f, 1f, 0f, -1.6f, 1f, 1f, -1.6f, .5f, -1f, .4f,
                1f, -.4f, .4f, -.4f, 1f, 0f, .4f, 1f, 1f, .4f, .5f, 1f, .4f, 1f, 1.6f, .4f, 1.6f, 1f, 0f, 2.5f, 1f,
                0f, 3f, .5f, 0f, 6f, .5f, 0f, 3f, -1f, 0f, -3f, -1f),
            Material = MaterialExt.Stone,
            Dynamic = true,
            Color = new Color(1f, 1f, 1f)
        });
        result.Objects.Add(new BsShape("car-wheel-1-axle")
        {
            Position = new Vector2(-6f, 7f),
            Rotation = 45f,
            Layer = 1,
            Path = Path.Square(1f),
            Material = (0f, 0f, 0f, 1f, 0f),
            Dynamic = true,
            Color = new Color(1f, 1f, 1f)
        });
        result.Objects.Add(new BsSlider("car-wheel-1-anchor", "car-wheel-1-axle", "car-body")
        {
            Position = new Vector2(-6f, 0f),
            Rotation = 90f
        });
        result.Objects.Add(new BsSpring("car-wheel-1-spring", "car-wheel-1-axle", "car-body")
        {
            Position = new Vector2(-6f, 0f),
            RestLength = 3f,
            Damping = .3f
        });
        result.Objects.Add(new BsShape("car-wheel-1")
        {
            Position = new Vector2(-6f, 7f),
            Path = Path.Circle(3f),
            Material = (10f, 0f, 0f, 1f, 0f),
            Dynamic = true,
            Color = new Color(.15f, .15f, .15f)
        });
        result.Objects.Add(new BsHinge("car-wheel-1-motor", "car-wheel-1", "car-wheel-1-axle")
        {
            MotorEnabled = true
        });
        result.Objects.Add(new BsShape("car-wheel-2-axle")
        {
            Position = new Vector2(6f, 7f),
            Rotation = 45f,
            Layer = 1,
            Path = Path.Square(1f),
            Material = (0f, 0f, 0f, 1f, 0f),
            Dynamic = true,
            Color = new Color(1f, 1f, 1f)
        });
        result.Objects.Add(new BsSlider("car-wheel-2-anchor", "car-wheel-2-axle", "car-body")
        {
            Position = new Vector2(6f, 0f),
            Rotation = 90f
        });
        result.Objects.Add(new BsSpring("car-wheel-2-spring", "car-wheel-2-axle", "car-body")
        {
            Position = new Vector2(6f, 0f),
            RestLength = 3f,
            Damping = .3f
        });
        result.Objects.Add(new BsShape("car-wheel-2")
        {
            Position = new Vector2(6f, 7f),
            Path = Path.Circle(3f),
            Material = (10f, 0f, 0f, 1f, 0f),
            Dynamic = true,
            Color = new Color(.15f, .15f, .15f)
        });
        result.Objects.Add(new BsHinge("car-wheel-2-motor", "car-wheel-2", "car-wheel-2-axle")
        {
            MotorEnabled = true
        });
        result.Objects.Add(new BsMount("car-seat-1", "car-body")
        {
            Position = new Vector2(1f, 1f)
        });
        result.Objects.Add(new BsMount("car-seat-2", "car-body")
        {
            Position = new Vector2(-1f, 1f)
        });

        // Add logic
        result.Nodes.Add(BsNode.ConstantFloat("engine-power", 15000f));
        result.Nodes.Add(BsNode.Multiply("wheel-output", 2));
        result.Links.Add(new BsLink("car-seat-1", 1, "wheel-output", 0));
        result.Links.Add(new BsLink("engine-power", 0, "wheel-output", 1));
        result.Links.Add(new BsLink("wheel-output", 0, "car-wheel-1-motor", 5));
        result.Links.Add(new BsLink("wheel-output", 0, "car-wheel-2-motor", 5));

        return result;
    }
}
