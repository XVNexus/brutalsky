using Brutalsky.Scripts.Data;
using Brutalsky.Scripts.Extensions;
using Brutalsky.Scripts.Utils;
using Godot;

namespace Brutalsky.Scripts.Maps;

public class MapTossup : MapGenerator
{
    public override BsMap Generate()
    {
        // Create map
        var result = new BsMap("Tossup", AuthorBuiltin)
        {
            PlayArea = new Rect2(-20f, -10f, 40f, 20f),
            InitialGravity = new Vector2(0f, -20f),
            InitialAtmosphere = .5f
        };

        // Add spawns
        result.Spawns.Add(new BsSpawn(new Vector2(-7f, -8.5f), 1));
        result.Spawns.Add(new BsSpawn(new Vector2(-5f, -8.5f), 0));
        result.Spawns.Add(new BsSpawn(new Vector2(5f, -8.5f), 0));
        result.Spawns.Add(new BsSpawn(new Vector2(7f, -8.5f), 1));

        // Add objects
        result.Objects.Add(new BsShape("wall-bottom")
        {
            Position = new Vector2(0f, -10f),
            Path = Path.Polygon(-15f, 0f, -10f, 1f, 10f, 1f, 15f, 0f),
            Material = MaterialExt.Stone,
            Color = ColorExt.Stone
        });
        result.Objects.Add(new BsShape("wall-left")
        {
            Position = new Vector2(-20f, 10f),
            Path = Path.Polygon(0f, 0f, 10f, 0f, 5f, -1f, 1f, -1f, 1f, -10f, 0f, -15f),
            Material = MaterialExt.Stone,
            Color = ColorExt.Stone
        });
        result.Objects.Add(new BsShape("wall-right")
        {
            Position = new Vector2(20f, 10f),
            Path = Path.Polygon(0f, 0f, -10f, 0f, -5f, -1f, -1f, -1f, -1f, -10f, 0f, -15f),
            Material = MaterialExt.Stone,
            Color = ColorExt.Stone
        });
        result.Objects.Add(new BsDecal("spinner-left-bg")
        {
            Position = new Vector2(-17f, -7f),
            Layer = -1,
            Path = Path.Circle(4f),
            Color = new Color(1f, .2f, .2f, .05f)
        });
        result.Objects.Add(new BsShape("spinner-left")
        {
            Position = new Vector2(-17f, -7f),
            Path = Path.Star(3, 4f, 1f),
            Material = (5f, 0f, 20f, 1f, 0f),
            Dynamic = true,
            Color = new Color(1f, .2f, .2f)
        });
        result.Objects.Add(new BsHinge("spinner-left-motor", "spinner-left")
        {
            Position = new Vector2(-17f, -7f),
            MotorEnabled = true
        });
        result.Objects.Add(new BsDecal("spinner-middle-bg")
        {
            Position = new Vector2(0f, 7f),
            Layer = -1,
            Path = Path.Circle(4f),
            Color = new Color(.2f, 1f, .2f, .05f)
        });
        result.Objects.Add(new BsShape("spinner-middle")
        {
            Position = new Vector2(0f, 7f),
            Path = Path.Star(3, 4f, 1f),
            Material = (5f, 0f, 20f, 1f, 0f),
            Dynamic = true,
            Color = new Color(.2f, 1f, .2f)
        });
        result.Objects.Add(new BsHinge("spinner-middle-motor", "spinner-middle")
        {
            Position = new Vector2(0f, 7f),
            MotorEnabled = true
        });
        result.Objects.Add(new BsDecal("spinner-right-bg")
        {
            Position = new Vector2(17f, -7f),
            Layer = -1,
            Path = Path.Circle(4f),
            Color = new Color(.2f, .2f, 1f, .05f)
        });
        result.Objects.Add(new BsShape("spinner-right")
        {
            Position = new Vector2(17f, -7f),
            Path = Path.Star(3, 4f, 1f),
            Material = (5f, 0f, 20f, 1f, 0f),
            Dynamic = true,
            Color = new Color(.2f, .2f, 1f)
        });
        result.Objects.Add(new BsHinge("spinner-right-motor", "spinner-right")
        {
            Position = new Vector2(17f, -7f),
            MotorEnabled = true
        });

        // Add logic
        result.Nodes.Add(BsNode.Timer("timer"));
        result.Nodes.Add(BsNode.ConstantFloat("pi", Mathf.Pi));
        result.Nodes.Add(BsNode.Multiply("timer-scale", 2));
        result.Nodes.Add(BsNode.Sin("timer-sin"));
        result.Nodes.Add(BsNode.ConstantFloat("max-speed", 1000f));
        result.Nodes.Add(BsNode.Multiply("motor-speed", 2));
        result.Nodes.Add(BsNode.Clock("random-driver", 60));
        result.Nodes.Add(BsNode.Monostable("random-trigger"));
        result.Nodes.Add(BsNode.RandomInt("random", 0, 2));
        result.Nodes.Add(BsNode.Demultiplexer("motor-output", 3));
        result.Links.Add(new BsLink("timer", 0, "timer-scale", 0));
        result.Links.Add(new BsLink("pi", 0, "timer-scale", 1));
        result.Links.Add(new BsLink("timer-scale", 0, "timer-sin", 0));
        result.Links.Add(new BsLink("timer-sin", 0, "motor-speed", 0));
        result.Links.Add(new BsLink("max-speed", 0, "motor-speed", 1));
        result.Links.Add(new BsLink("motor-speed", 0, "motor-output", 1));
        result.Links.Add(new BsLink("random-driver", 0, "random-trigger", 0));
        result.Links.Add(new BsLink("random-trigger", 0, "random", 0));
        result.Links.Add(new BsLink("random", 0, "motor-output", 0));
        result.Links.Add(new BsLink("motor-output", 0, "spinner-left-motor", 5));
        result.Links.Add(new BsLink("motor-output", 1, "spinner-middle-motor", 5));
        result.Links.Add(new BsLink("motor-output", 2, "spinner-right-motor", 5));

        return result;
    }
}
