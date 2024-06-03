using Data;
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
            });
            result.Objects.Add(new BsJoint("spinner-left-motor", "spinner-left")
            {
                Type = BsJoint.TypeHinge,
                OtherAnchor = new Vector2(-17f, -7f),
                Motor = true,
                MotorForce = 10000f
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
            result.Objects.Add(new BsJoint("spinner-middle-motor", "spinner-middle")
            {
                Type = BsJoint.TypeHinge,
                OtherAnchor = new Vector2(0f, 7f),
                Motor = true,
                MotorForce = 10000f
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
            result.Objects.Add(new BsJoint("spinner-right-motor", "spinner-right")
            {
                Type = BsJoint.TypeHinge,
                OtherAnchor = new Vector2(17f, -7f),
                Motor = true,
                MotorForce = 10000f
            });

            return result;
        }
    }
}
