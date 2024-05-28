using Data;
using Data.Addon;
using Data.Map;
using Data.Object;
using Extensions;
using UnityEngine;
using Utils;

namespace Maps
{
    public class MapDoomring : MapGenerator
    {
        public override BsMap Generate()
        {
            // Create map
            var result = new BsMap("Doomring", AuthorBuiltin)
            {
                PlayArea = new Rect(-15f, -15f, 30f, 30f)
            };

            // Add spawns
            result.Spawns.Add(new BsSpawn(new Vector2(0f, 5f), 0));
            result.Spawns.Add(new BsSpawn(new Vector2(5f, 0f), 0));
            result.Spawns.Add(new BsSpawn(new Vector2(0f, -5f), 0));
            result.Spawns.Add(new BsSpawn(new Vector2(-5f, 0f), 0));

            // Add objects
            result.Objects.Add(new BsDecal("background-top")
            {
                Rotation = 45f,
                Layer = -1,
                Path = Path.Vector(0f, 0f, 0f, 0f, 15f, 1f, 15f, 15f, 15f, 0f),
                Color = new Color(1f, .2f, .2f, .05f)
            });
            result.Objects.Add(new BsShape("wall-top")
            {
                Rotation = 45f,
                Path = Path.Vector(0f, 15f, 1f, 15f, 15f, 15f, 0f, 0f, 14f, 0f, 1f, 14f, 14f, 0f, 14f),
                Material = (2f, 2f, 0f, 10f, -10f),
                Color = new Color(1f, .2f, .2f)
            });
            result.Objects.Add(new BsDecal("background-right")
            {
                Rotation = -45f,
                Layer = -1,
                Path = Path.Vector(0f, 0f, 0f, 0f, 15f, 1f, 15f, 15f, 15f, 0f),
                Color = new Color(.6f, 1f, .2f, .05f)
            });
            result.Objects.Add(new BsShape("wall-right")
            {
                Rotation = -45f,
                Path = Path.Vector(0f, 15f, 1f, 15f, 15f, 15f, 0f, 0f, 14f, 0f, 1f, 14f, 14f, 0f, 14f),
                Material = (2f, 2f, 0f, 10f, -10f),
                Color = new Color(.6f, 1f, .2f)
            });
            result.Objects.Add(new BsDecal("background-bottom")
            {
                Rotation = -135f,
                Layer = -1,
                Path = Path.Vector(0f, 0f, 0f, 0f, 15f, 1f, 15f, 15f, 15f, 0f),
                Color = new Color(.2f, 1f, 1f, .05f)
            });
            result.Objects.Add(new BsShape("wall-bottom")
            {
                Rotation = -135f,
                Path = Path.Vector(0f, 15f, 1f, 15f, 15f, 15f, 0f, 0f, 14f, 0f, 1f, 14f, 14f, 0f, 14f),
                Material = (2f, 2f, 0f, 10f, -10f),
                Color = new Color(.2f, 1f, 1f)
            });
            result.Objects.Add(new BsDecal("background-left")
            {
                Rotation = -225f,
                Layer = -1,
                Path = Path.Vector(0f, 0f, 0f, 0f, 15f, 1f, 15f, 15f, 15f, 0f),
                Color = new Color(.6f, .2f, 1f, .05f)
            });
            result.Objects.Add(new BsShape("wall-left")
            {
                Rotation = -225f,
                Path = Path.Vector(0f, 15f, 1f, 15f, 15f, 15f, 0f, 0f, 14f, 0f, 1f, 14f, 14f, 0f, 14f),
                Material = (2f, 2f, 0f, 10f, -10f),
                Color = new Color(.6f, .2f, 1f)
            });
            result.Objects.Add(new BsShape("spinner")
            {
                Path = Path.Star(4, 4f, 1f),
                Material = (5f, 0f, 0f, 100f, 0f),
                Dynamic = true,
                Color = ColorExt.Metal
            }
            .AppendAddon(new BsJoint("spinner-motor")
            {
                Type = BsJoint.TypeHinge,
                MotorEnabled = true,
                MotorSpeed = 50f,
                MotorForce = 1000000f
            }));
            result.Objects.Add(new BsShape("spinner-arm-top")
            {
                Position = new Vector2(0f, 10f),
                Path = Path.Ngon(3, 4f),
                Material = MaterialExt.Metal,
                Dynamic = true,
                Color = ColorExt.Metal
            }
            .AppendAddon(new BsJoint("spinner-arm-top-link", "spinner")
            {
                Type = BsJoint.TypeHinge,
                OtherAnchor = new Vector2(0f, 10f),
                MotorEnabled = true,
                MotorSpeed = -1000f,
                MotorForce = 100f
            }));
            result.Objects.Add(new BsShape("spinner-arm-right")
            {
                Position = new Vector2(10f, 0f),
                Rotation = -90f,
                Path = Path.Ngon(3, 4f),
                Material = MaterialExt.Metal,
                Dynamic = true,
                Color = ColorExt.Metal
            }
            .AppendAddon(new BsJoint("spinner-arm-right-link", "spinner")
            {
                Type = BsJoint.TypeHinge,
                OtherAnchor = new Vector2(10f, 0f),
                MotorEnabled = true,
                MotorSpeed = -1000f,
                MotorForce = 100f
            }));
            result.Objects.Add(new BsShape("spinner-arm-bottom")
            {
                Position = new Vector2(0f, -10f),
                Rotation = -180f,
                Path = Path.Ngon(3, 4f),
                Material = MaterialExt.Metal,
                Dynamic = true,
                Color = ColorExt.Metal
            }
            .AppendAddon(new BsJoint("spinner-arm-bottom-link", "spinner")
            {
                Type = BsJoint.TypeHinge,
                OtherAnchor = new Vector2(0f, -10f),
                MotorEnabled = true,
                MotorSpeed = -1000f,
                MotorForce = 100f
            }));
            result.Objects.Add(new BsShape("spinner-arm-left")
            {
                Position = new Vector2(-10f, 0f),
                Rotation = -270f,
                Path = Path.Ngon(3, 4f),
                Material = MaterialExt.Metal,
                Dynamic = true,
                Color = ColorExt.Metal
            }
            .AppendAddon(new BsJoint("spinner-arm-left-link", "spinner")
            {
                Type = BsJoint.TypeHinge,
                OtherAnchor = new Vector2(-10f, 0f),
                MotorEnabled = true,
                MotorSpeed = -1000f,
                MotorForce = 100f
            }));

            return result;
        }
    }
}
