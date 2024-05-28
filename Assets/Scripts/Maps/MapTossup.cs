using Data;
using Data.Addon;
using Data.Logic;
using Data.Map;
using Data.Object;
using Extensions;
using UnityEngine;
using Utils;

namespace Maps
{
    public class MapTossup : MapGenerator
    {
        public override BsMap Generate()
        {
            // Create map
            var result = new BsMap("Tossup", AuthorBuiltin)
            {
                PlayArea = new Rect(-20f, -10f, 40f, 20f),
                GravityDirection = BsMap.DirectionDown,
                GravityStrength = 20f,
                AirResistance = .5f
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
            }
            .AppendAddon(new BsJoint("spinner-left-motor")
            {
                Type = BsJoint.TypeHinge,
                MountAnchor = new Vector2(-17f, -7f),
                MotorEnabled = true,
                MotorForce = 10000f
            }));
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
            }
            .AppendAddon(new BsJoint("spinner-middle-motor")
            {
                Type = BsJoint.TypeHinge,
                MountAnchor = new Vector2(0f, 7f),
                MotorEnabled = true,
                MotorForce = 10000f
            }));
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
            }
            .AppendAddon(new BsJoint("spinner-right-motor")
            {
                Type = BsJoint.TypeHinge,
                MountAnchor = new Vector2(17f, -7f),
                MotorEnabled = true,
                MotorForce = 10000f
            }));

            // Add logic
            result.Nodes.Add(BsNode.Timer("map-timer"));
            result.Nodes.Add(BsNode.ConstantFloat("pi", Mathf.PI));
            result.Nodes.Add(BsNode.Multiply("timer-scaled", 2));
            result.Nodes.Add(BsNode.Sin("speed-scale"));
            result.Nodes.Add(BsNode.ConstantFloat("speed-max", 1000f));
            result.Nodes.Add(BsNode.Multiply("speed-output", 2));
            result.Nodes.Add(BsNode.Clock("spinner-timer", 50));
            result.Nodes.Add(BsNode.Monostable("spinner-switcher"));
            result.Nodes.Add(BsNode.RandomInt("spinner-selector", 0, 2));
            result.Nodes.Add(BsNode.Demultiplexer("spinner-controller", 3));
            result.Links.Add(new BsLink("map-timer", 0, "timer-scaled", 0));
            result.Links.Add(new BsLink("pi", 0, "timer-scaled", 1));
            result.Links.Add(new BsLink("timer-scaled", 0, "speed-scale", 0));
            result.Links.Add(new BsLink("speed-scale", 0, "speed-output", 0));
            result.Links.Add(new BsLink("speed-max", 0, "speed-output", 1));
            result.Links.Add(new BsLink("speed-output", 0, "spinner-controller", 1));
            result.Links.Add(new BsLink("spinner-timer", 0, "spinner-switcher", 0));
            result.Links.Add(new BsLink("spinner-switcher", 0, "spinner-selector", 0));
            result.Links.Add(new BsLink("spinner-selector", 0, "spinner-controller", 0));
            result.Links.Add(new BsLink("spinner-controller", 0, "spinner-left-motor", 4));
            result.Links.Add(new BsLink("spinner-controller", 1, "spinner-middle-motor", 4));
            result.Links.Add(new BsLink("spinner-controller", 2, "spinner-right-motor", 4));

            return result;
        }
    }
}
