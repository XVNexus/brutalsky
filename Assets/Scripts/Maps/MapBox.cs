using Data;
using Data.Object;
using Extensions;
using UnityEngine;
using Utils;

namespace Maps
{
    public class MapBox : MapGenerator
    {
        public int Number { get; }
        public byte Shape { get; }
        public Vector2 Size { get; }

        public MapBox(int number, byte shape, Vector2 size)
        {
            Number = number;
            Shape = shape;
            Size = size;
        }

        public override BsMap Generate()
        {
            // Create map
            var bottom = (Shape & 0b1000) > 0;
            var top = (Shape & 0b0100) > 0;
            var left = (Shape & 0b0010) > 0;
            var right = (Shape & 0b0001) > 0;
            var result = new BsMap($"Box {Number}", AuthorGenerated)
            {
                PlayArea = new Rect(Size * -.5f, Size),
                BackgroundColor = new Color(.2f, .2f, .2f),
                GravityDirection = BsMap.DirectionDown,
                GravityStrength = 20f
            };

            // Add spawns
            result.Spawns.Add(new BsSpawn(new Vector2(-3f, -Size.y * .5f + 1.5f), 0));
            result.Spawns.Add(new BsSpawn(new Vector2(-1f, -Size.y * .5f + 1.5f), 0));
            result.Spawns.Add(new BsSpawn(new Vector2(1f, -Size.y * .5f + 1.5f), 0));
            result.Spawns.Add(new BsSpawn(new Vector2(3f, -Size.y * .5f + 1.5f), 0));

            // Add walls
            if (bottom)
            {
                result.Objects.Add(new BsShape("wall-bottom")
                {
                    Position = new Vector2(0f, Size.y * -.5f + .5f),
                    Path = Path.Rectangle(Size.x, 1f),
                    Material = MaterialExt.Stone
                });
            }
            if (top)
            {
                result.Objects.Add(new BsShape("wall-top")
                {
                    Position = new Vector2(0f, Size.y * .5f - .5f),
                    Path = Path.Rectangle(Size.x, 1f),
                    Material = MaterialExt.Stone
                });
            }
            if (left)
            {
                result.Objects.Add(new BsShape("wall-left")
                {
                    Position = new Vector2(Size.x * -.5f + .5f, 0f),
                    Path = Path.Rectangle(1f, Size.y),
                    Material = MaterialExt.Stone
                });
            }
            if (right)
            {
                result.Objects.Add(new BsShape("wall-right")
                {
                    Position = new Vector2(Size.x * .5f - .5f, 0f),
                    Path = Path.Rectangle(1f, Size.y),
                    Material = MaterialExt.Stone
                });
            }

            // Add corners
            if (top && left)
            {
                result.Objects.Add(new BsShape("corner-tl")
                {
                    Position = new Vector2(Size.x * -.5f + 1f, Size.y * .5f - 1f),
                    Path = Path.Vector(0f, 0f, 0f, 3f, 0f, 1f, 0f, 0f, 0f, -3f),
                    Material = MaterialExt.Stone
                });
            }
            if (top && right)
            {
                result.Objects.Add(new BsShape("corner-tr")
                {
                    Position = new Vector2(Size.x * .5f - 1f, Size.y * .5f - 1f),
                    Path = Path.Vector(0f, 0f, 0f, -3f, 0f, 1f, 0f, 0f, 0f, -3f),
                    Material = MaterialExt.Stone
                });
            }
            if (bottom && left)
            {
                result.Objects.Add(new BsShape("corner-bl")
                {
                    Position = new Vector2(Size.x * -.5f + 1f, Size.y * -.5f + 1f),
                    Path = Path.Vector(0f, 0f, 0f, 3f, 0f, 1f, 0f, 0f, 0f, 3f),
                    Material = MaterialExt.Stone
                });
            }
            if (bottom && right)
            {
                result.Objects.Add(new BsShape("corner-br")
                {
                    Position = new Vector2(Size.x * .5f - 1f, Size.y * -.5f + 1f),
                    Path = Path.Vector(0f, 0f, 0f, -3f, 0f, 1f, 0f, 0f, 0f, 3f),
                    Material = MaterialExt.Stone
                });
            }

            return result;
        }
    }
}
