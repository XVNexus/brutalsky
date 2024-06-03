using Data;
using Data.Object;
using Extensions;
using UnityEngine;
using Utils;

namespace Maps
{
    public class MapBrutalsky : MapGenerator
    {
        public override BsMap Generate()
        {
            // Create map
            var result = new BsMap("Brutalsky", AuthorBuiltin)
            {
                PlayArea = new Rect(-20f, -10f, 40f, 20f),
                GravityDirection = BsMap.DirectionDown,
                GravityStrength = 20f,
                AirResistance = .5f
            };

            // Add spawns
            result.Spawns.Add(new BsSpawn(new Vector2(-16f, 1f), 1));
            result.Spawns.Add(new BsSpawn(new Vector2(-14f, 1f), 0));
            result.Spawns.Add(new BsSpawn(new Vector2(14f, 1f), 0));
            result.Spawns.Add(new BsSpawn(new Vector2(16f, 1f), 1));

            // Add objects
            result.Objects.Add(new BsShape("wall-left")
            {
                Position = new Vector2(-19.5f, 0f),
                Path = Path.Rectangle(1f, 20f),
                Material = MaterialExt.Stone,
                Color = ColorExt.Stone
            });
            result.Objects.Add(new BsShape("wall-right")
            {
                Position = new Vector2(19.5f, 0f),
                Path = Path.Rectangle(1f, 20f),
                Material = MaterialExt.Stone,
                Color = ColorExt.Stone
            });
            result.Objects.Add(new BsShape("wall-top")
            {
                Position = new Vector2(0f, 10f),
                Path = Path.Polygon(-15f, 0f, 15f, 0f, 14f, -.5f, -14f, -.5f),
                Material = MaterialExt.Stone,
                Color = ColorExt.Stone
            });
            result.Objects.Add(new BsShape("wall-bottom")
            {
                Position = new Vector2(0f, -10f),
                Path = Path.Polygon(-7f, 0f, -7f, .5f, 0f, 3f, 7f, .5f, 7f, 0f),
                Material = MaterialExt.Stone,
                Color = ColorExt.Stone
            });
            result.Objects.Add(new BsShape("platform-left")
            {
                Position = new Vector2(-19f, 0f),
                Path = Path.Vector(-1f, 2.5f, 0f, 0f, 2.5f, 1f, 0f, .5f, 2f, .5f, 0f, 8f, .5f, 0f, 7.5f, 0f, 0f,
                    8f, -.5f, 0f, 2f, -.5f, 1f, 0f, -.5f, 0f, -2.5f, 0f, -1f, -2.5f),
                Material = MaterialExt.Stone,
                Color = ColorExt.Stone
            });
            result.Objects.Add(new BsShape("platform-right")
            {
                Position = new Vector2(19f, 0f),
                Path = Path.Vector(1f, 2.5f, 0f, 0f, 2.5f, 1f, 0f, .5f, -2f, .5f, 0f, -8f, .5f, 0f, -7.5f, 0f, 0f,
                    -8f, -.5f, 0f, -2f, -.5f, 1f, 0f, -.5f, 0f, -2.5f, 0f, 1f, -2.5f),
                Material = MaterialExt.Stone,
                Color = ColorExt.Stone
            });
            result.Objects.Add(new BsShape("glue-left")
            {
                Position = new Vector2(-9f, 9.25f),
                Path = Path.Rectangle(10f, .5f),
                Material = MaterialExt.Glue,
                Color = ColorExt.Glue
            });
            result.Objects.Add(new BsShape("glue-right")
            {
                Position = new Vector2(9f, 9.25f),
                Path = Path.Rectangle(10f, .5f),
                Material = MaterialExt.Glue,
                Color = ColorExt.Glue
            });
            result.Objects.Add(new BsShape("ice-left")
            {
                Position = new Vector2(-14.5f, 9.5f),
                Path = Path.Polygon(-.5f, .5f, .5f, 0f, .5f, -.5f, -.5f, 0f),
                Material = MaterialExt.Ice,
                Color = ColorExt.Ice
            });
            result.Objects.Add(new BsShape("ice-right")
            {
                Position = new Vector2(14.5f, 9.5f),
                Path = Path.Polygon(.5f, .5f, -.5f, 0f, -.5f, -.5f, .5f, 0f),
                Material = MaterialExt.Ice,
                Color = ColorExt.Ice
            });
            result.Objects.Add(new BsShape("ice-bottom")
            {
                Position = new Vector2(0f, -9.5f),
                Path = Path.Polygon(-7f, 0f, -7f, .5f, 0f, 3f, 7f, .5f, 7f, 0f, 0f, 2.5f),
                Material = MaterialExt.Ice,
                Color = ColorExt.Ice
            });
            result.Objects.Add(new BsShape("rubber-top")
            {
                Position = new Vector2(0f, 9.25f),
                Path = Path.Polygon(4f, .25f, 4f, -.25f, 0f, -1.25f, -4f, -.25f, -4f, .25f),
                Material = MaterialExt.Rubber,
                Color = ColorExt.Rubber
            });
            result.Objects.Add(new BsShape("electric-left")
            {
                Position = new Vector2(-11f, 0f),
                Rotation = 45f,
                Path = Path.Square(.7f),
                Material = MaterialExt.Electric,
                Color = ColorExt.Electric
            });
            result.Objects.Add(new BsShape("electric-right")
            {
                Position = new Vector2(11f, 0f),
                Rotation = 45f,
                Path = Path.Square(.7f),
                Material = MaterialExt.Electric,
                Color = ColorExt.Electric
            });
            result.Objects.Add(new BsDecal("spinner-left-bg")
            {
                Position = new Vector2(-2.5f, 0f),
                Layer = -1,
                Path = Path.Circle(5f),
                Color = Color.white.SetAlpha(.05f)
            });
            result.Objects.Add(new BsShape("spinner-left")
            {
                Position = new Vector2(-2.5f, 0f),
                Path = Path.Star(6, 5f, 3f),
                Material = MaterialExt.Metal,
                Dynamic = true,
                Color = ColorExt.Metal
            });
            result.Objects.Add(new BsJoint("spinner-left-motor", "spinner-left")
            {
                Type = BsJoint.TypeHinge,
                OtherAnchor = new Vector2(-2.5f, 0f),
                Motor = true,
                MotorForce = 500f
            });
            result.Objects.Add(new BsDecal("spinner-right-bg")
            {
                Position = new Vector2(2.5f, 0f),
                Layer = -1,
                Path = Path.Circle(5f),
                Color = Color.white.SetAlpha(.05f)
            });
            result.Objects.Add(new BsShape("spinner-right")
            {
                Position = new Vector2(2.5f, 0f),
                Rotation = 30f,
                Path = Path.Star(6, 5f, 3f),
                Material = MaterialExt.Metal,
                Dynamic = true,
                Color = ColorExt.Metal
            });
            result.Objects.Add(new BsJoint("spinner-right-motor", "spinner-right")
            {
                Type = BsJoint.TypeHinge,
                OtherAnchor = new Vector2(2.5f, 0f),
                Motor = true,
                MotorForce = 500f
            });
            result.Objects.Add(new BsSensor("spinner-sensor")
            {
                Position = new Vector2(0f, 5f),
                Size = new Vector2(10f, 10f)
            });
            result.Objects.Add(new BsPool("water-left")
            {
                Position = new Vector2(-17f, 11.25f),
                Rotation = 180f,
                Layer = 1,
                Size = new Vector2(4f, 3.5f),
                Chemical = ChemicalExt.Water,
                Color = ColorExt.Water
            });
            result.Objects.Add(new BsShape("water-left-top")
            {
                Position = new Vector2(-17f, 13.25f),
                Path = Path.Rectangle(5f, .5f),
                Material = MaterialExt.Medkit,
                Color = ColorExt.Water.MultiplyTint(.5f)
            });
            result.Objects.Add(new BsShape("water-left-left")
            {
                Position = new Vector2(-19.25f, 11.5f),
                Path = Path.Rectangle(.5f, 3f),
                Material = MaterialExt.Stone,
                Color = ColorExt.Water.MultiplyTint(.5f)
            });
            result.Objects.Add(new BsShape("water-left-right")
            {
                Position = new Vector2(-14.75f, 11.5f),
                Path = Path.Rectangle(.5f, 3f),
                Material = MaterialExt.Stone,
                Color = ColorExt.Water.MultiplyTint(.5f)
            });
            result.Objects.Add(new BsPool("water-right")
            {
                Position = new Vector2(17f, 11.25f),
                Rotation = 180f,
                Layer = 1,
                Size = new Vector2(4f, 3.5f),
                Chemical = ChemicalExt.Water,
                Color = ColorExt.Water
            });
            result.Objects.Add(new BsShape("water-right-top")
            {
                Position = new Vector2(17f, 13.25f),
                Path = Path.Rectangle(5f, .5f),
                Material = MaterialExt.Medkit,
                Color = ColorExt.Water.MultiplyTint(.5f)
            });
            result.Objects.Add(new BsShape("water-right-left")
            {
                Position = new Vector2(14.75f, 11.5f),
                Path = Path.Rectangle(.5f, 3f),
                Material = MaterialExt.Stone,
                Color = ColorExt.Water.MultiplyTint(.5f)
            });
            result.Objects.Add(new BsShape("water-right-right")
            {
                Position = new Vector2(19.25f, 11.5f),
                Path = Path.Rectangle(.5f, 3f),
                Material = MaterialExt.Stone,
                Color = ColorExt.Water.MultiplyTint(.5f)
            });
            result.Objects.Add(new BsPool("lava-left")
            {
                Position = new Vector2(-13f, -11f),
                Layer = 1,
                Size = new Vector2(12f, 4f),
                Chemical = ChemicalExt.Lava,
                Color = ColorExt.Lava,
                Glow = true
            });
            result.Objects.Add(new BsShape("lava-left-bottom")
            {
                Position = new Vector2(-13f, -13.25f),
                Path = Path.Rectangle(13f, .5f),
                Material = MaterialExt.Electric,
                Color = ColorExt.Lava.MultiplyTint(.5f)
            });
            result.Objects.Add(new BsShape("lava-left-left")
            {
                Position = new Vector2(-19.25f, -11.5f),
                Path = Path.Rectangle(.5f, 3f),
                Material = MaterialExt.Stone,
                Color = ColorExt.Lava.MultiplyTint(.5f)
            });
            result.Objects.Add(new BsShape("lava-left-right")
            {
                Position = new Vector2(-6.75f, -11.5f),
                Path = Path.Rectangle(.5f, 3f),
                Material = MaterialExt.Stone,
                Color = ColorExt.Lava.MultiplyTint(.5f)
            });
            result.Objects.Add(new BsPool("lava-right")
            {
                Position = new Vector2(13f, -11f),
                Layer = 1,
                Size = new Vector2(12f, 4f),
                Chemical = ChemicalExt.Lava,
                Color = ColorExt.Lava,
                Glow = true
            });
            result.Objects.Add(new BsShape("lava-right-bottom")
            {
                Position = new Vector2(13f, -13.25f),
                Path = Path.Rectangle(13f, .5f),
                Material = MaterialExt.Electric,
                Color = ColorExt.Lava.MultiplyTint(.5f)
            });
            result.Objects.Add(new BsShape("lava-right-left")
            {
                Position = new Vector2(6.75f, -11.5f),
                Path = Path.Rectangle(.5f, 3f),
                Material = MaterialExt.Stone,
                Color = ColorExt.Lava.MultiplyTint(.5f)
            });
            result.Objects.Add(new BsShape("lava-right-right")
            {
                Position = new Vector2(19.25f, -11.5f),
                Path = Path.Rectangle(.5f, 3f),
                Material = MaterialExt.Stone,
                Color = ColorExt.Lava.MultiplyTint(.5f)
            });

            // Add logic
            /* 0 */ result.Nodes.Add(BsNode.Float(1f));
            /* 1 */ result.Nodes.Add(BsNode.Float(-1f));
            /* 2 */ result.Nodes.Add(BsNode.Float(50f));
            /* 3 */ result.Nodes.Add(BsNode.Float(5000f));
            /* 4 */ result.Nodes.Add(BsNode.Multiplex(2));
            /* 5 */ result.Nodes.Add(BsNode.Multiply(2));
            /* 6 */ result.Nodes.Add(BsNode.Multiply(2));
            // 7 Joint ("spinner-left-motor")
            // 8 Joint ("spinner-right-motor")
            // 9 Sensor ("spinner-sensor")
            result.Links.Add(new BsLink(9, "triggered", 4, "select"));
            result.Links.Add(new BsLink(2, "value", 4, "input-0"));
            result.Links.Add(new BsLink(3, "value", 4, "input-1"));
            result.Links.Add(new BsLink(4, "value", 5, "input-0"));
            result.Links.Add(new BsLink(4, "value", 6, "input-0"));
            result.Links.Add(new BsLink(0, "value", 5, "input-1"));
            result.Links.Add(new BsLink(1, "value", 6, "input-1"));
            result.Links.Add(new BsLink(5, "value", 7, "motor-speed"));
            result.Links.Add(new BsLink(6, "value", 8, "motor-speed"));

            return result;
        }
    }
}
