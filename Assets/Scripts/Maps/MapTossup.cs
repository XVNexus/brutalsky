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

            // Add logic
            /*  0 */ result.Nodes.Add(BsNode.Timer());
            /*  1 */ result.Nodes.Add(BsNode.Int(60));
            /*  2 */ result.Nodes.Add(BsNode.Divide(2));
            /*  3 */ result.Nodes.Add(BsNode.Float(Mathf.PI));
            /*  4 */ result.Nodes.Add(BsNode.Multiply(2));
            /*  5 */ result.Nodes.Add(BsNode.Sin());
            /*  6 */ result.Nodes.Add(BsNode.Float(1000f));
            /*  7 */ result.Nodes.Add(BsNode.Multiply(2));
            /*  8 */ result.Nodes.Add(BsNode.Clock(60));
            /*  9 */ result.Nodes.Add(BsNode.Monostable());
            /* 10 */ result.Nodes.Add(BsNode.RandomInt(0, 2));
            /* 11 */ result.Nodes.Add(BsNode.Delay(1));
            /* 12 */ result.Nodes.Add(BsNode.Demultiplex(3));
            // 13 Joint ("spinner-left-motor")
            // 14 Joint ("spinner-middle-motor")
            // 15 Joint ("spinner-right-motor")
            result.Links.Add(new BsLink(0, "out", 2, "i00"));
            result.Links.Add(new BsLink(1, "val", 2, "i01"));
            result.Links.Add(new BsLink(2, "out", 4, "i00"));
            result.Links.Add(new BsLink(3, "val", 4, "i01"));
            result.Links.Add(new BsLink(4, "out", 5, "inp"));
            result.Links.Add(new BsLink(5, "out", 7, "i00"));
            result.Links.Add(new BsLink(6, "val", 7, "i01"));
            result.Links.Add(new BsLink(8, "out", 9, "inp"));
            result.Links.Add(new BsLink(9, "out", 10, "gen"));
            result.Links.Add(new BsLink(10, "out", 11, "inp"));
            result.Links.Add(new BsLink(11, "out", 12, "sel"));
            result.Links.Add(new BsLink(7, "out", 12, "inp"));
            result.Links.Add(new BsLink(12, "o00", 13, "mts"));
            result.Links.Add(new BsLink(12, "o01", 14, "mts"));
            result.Links.Add(new BsLink(12, "o02", 15, "mts"));

            return result;
        }
    }
}
