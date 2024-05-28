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
    public class MapRacetrack : MapGenerator
    {
        public override BsMap Generate()
        {
            // Create map
            var result = new BsMap("Racetrack", AuthorBuiltin)
            {
                PlayArea = new Rect(-1250f, 0f, 2500f, 250f),
                GravityDirection = BsMap.DirectionDown,
                GravityStrength = 20f,
                AirResistance = .5f
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
                Color = Color.white
            });
            result.Objects.Add(new BsShape("car-wheel-1-axle")
            {
                Position = new Vector2(-6f, 7f),
                Rotation = 45f,
                Layer = 1,
                Path = Path.Square(1f),
                Material = (0f, 0f, 0f, 1f, 0f),
                Dynamic = true,
                Color = Color.white
            }
            .AppendAddon(new BsJoint("car-wheel-1-anchor", "car-body")
            {
                Type = BsJoint.TypeSlider,
                OtherAnchor = new Vector2(-6f, 0f),
                AngleValue = 90f
            })
            .AppendAddon(new BsJoint("car-wheel-1-spring", "car-body")
            {
                Type = BsJoint.TypeSpring,
                OtherAnchor = new Vector2(-6f, 0f),
                DistanceValue = 3f,
                DampingRatio = .3f,
                DampingFrequency = 15f
            }));
            result.Objects.Add(new BsShape("car-wheel-1")
            {
                Position = new Vector2(-6f, 7f),
                Path = Path.Circle(3f),
                Material = (10f, 0f, 0f, 1f, 0f),
                Dynamic = true,
                Color = new Color(.15f, .15f, .15f)
            }
            .AppendAddon(new BsJoint("car-wheel-1-motor", "car-wheel-1-axle")
            {
                Type = BsJoint.TypeHinge,
                MotorEnabled = true,
                MotorForce = 2000f
            }));
            result.Objects.Add(new BsShape("car-wheel-2-axle")
            {
                Position = new Vector2(6f, 7f),
                Rotation = 45f,
                Layer = 1,
                Path = Path.Square(1f),
                Material = (0f, 0f, 0f, 1f, 0f),
                Dynamic = true,
                Color = Color.white
            }
            .AppendAddon(new BsJoint("car-wheel-2-anchor", "car-body")
            {
                Type = BsJoint.TypeSlider,
                OtherAnchor = new Vector2(6f, 0f),
                AngleValue = 90f
            })
            .AppendAddon(new BsJoint("car-wheel-2-spring", "car-body")
            {
                Type = BsJoint.TypeSpring,
                OtherAnchor = new Vector2(6f, 0f),
                DistanceValue = 3f,
                DampingRatio = .3f,
                DampingFrequency = 15f
            }));
            result.Objects.Add(new BsShape("car-wheel-2")
            {
                Position = new Vector2(6f, 7f),
                Path = Path.Circle(3f),
                Material = (10f, 0f, 0f, 1f, 0f),
                Dynamic = true,
                Color = new Color(.15f, .15f, .15f)
            }
            .AppendAddon(new BsJoint("car-wheel-2-motor", "car-wheel-2-axle")
            {
                Type = BsJoint.TypeHinge,
                MotorEnabled = true,
                MotorForce = 2000f
            }));
            result.Objects.Add(new BsMount("car-seat-1", "car-body")
            {
                Position = new Vector2(1f, 1f)
            });
            result.Objects.Add(new BsMount("car-seat-2", "car-body")
            {
                Position = new Vector2(-1f, 1f)
            });

            // Add logic
            result.Nodes.Add(BsNode.ConstantFloat("wheel-speed", 15000f));
            result.Nodes.Add(BsNode.Multiply("wheel-output", 2));
            result.Links.Add(new BsLink("car-seat-1", 1, "wheel-output", 0));
            result.Links.Add(new BsLink("wheel-speed", 0, "wheel-output", 1));
            result.Links.Add(new BsLink("wheel-output", 0, "car-wheel-1-motor", 4));
            result.Links.Add(new BsLink("wheel-output", 0, "car-wheel-2-motor", 4));

            return result;
        }
    }
}
