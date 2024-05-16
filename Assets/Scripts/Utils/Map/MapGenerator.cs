using System.Text.RegularExpressions;
using Brutalsky;
using Brutalsky.Map;
using Brutalsky.Object;
using Core;
using UnityEngine;
using Utils.Constants;
using Utils.Ext;
using Utils.Path;
using Random = Unity.Mathematics.Random;

namespace Utils.Map
{
    public static class MapGenerator
    {
        public const string Author = "Brutalsky";

        public static BsMap Box(string title, int shape, Vector2 size)
        {
            // Create map
            var bottom = (shape & 0b1000) > 0;
            var top = (shape & 0b0100) > 0;
            var left = (shape & 0b0010) > 0;
            var right = (shape & 0b0001) > 0;
            var result = new BsMap(title, Author)
            {
                PlayArea = new Rect(size * -.5f, size),
                GravityDirection = Direction.Down,
                GravityStrength = 20f
            };

            // Add spawns
            result.AddSpawn(new BsSpawn(new Vector2(-3f, -size.y * .5f + 1.5f), 0));
            result.AddSpawn(new BsSpawn(new Vector2(-1f, -size.y * .5f + 1.5f), 0));
            result.AddSpawn(new BsSpawn(new Vector2(1f, -size.y * .5f + 1.5f), 0));
            result.AddSpawn(new BsSpawn(new Vector2(3f, -size.y * .5f + 1.5f), 0));

            // Add walls
            if (bottom)
            {
                result.AddObject(new BsShape("wall-bottom")
                {
                    Position = new Vector2(0f, size.y * -.5f + .5f),
                    Path = PathString.Rectangle(size.x, 1f)
                });
            }
            if (top)
            {
                result.AddObject(new BsShape("wall-top")
                {
                    Position = new Vector2(0f, size.y * .5f - .5f),
                    Path = PathString.Rectangle(size.x, 1f)
                });
            }
            if (left)
            {
                result.AddObject(new BsShape("wall-left")
                {
                    Position = new Vector2(size.x * -.5f + .5f, 0f),
                    Path = PathString.Rectangle(1f, size.y)
                });
            }
            if (right)
            {
                result.AddObject(new BsShape("wall-right")
                {
                    Position = new Vector2(size.x * .5f - .5f, 0f),
                    Path = PathString.Rectangle(1f, size.y)
                });
            }

            // Add corners
            if (top && left)
            {
                result.AddObject(new BsShape("corner-tl")
                {
                    Position = new Vector2(size.x * -.5f + 1f, size.y * .5f - 1f),
                    Path = PathString.Vector(0f, 0f, 0f, 3f, 0f, 1f, 0f, 0f, 0f, -3f)
                });
            }
            if (top && right)
            {
                result.AddObject(new BsShape("corner-tr")
                {
                    Position = new Vector2(size.x * .5f - 1f, size.y * .5f - 1f),
                    Path = PathString.Vector(0f, 0f, 0f, -3f, 0f, 1f, 0f, 0f, 0f, -3f)
                });
            }
            if (bottom && left)
            {
                result.AddObject(new BsShape("corner-bl")
                {
                    Position = new Vector2(size.x * -.5f + 1f, size.y * -.5f + 1f),
                    Path = PathString.Vector(0f, 0f, 0f, 3f, 0f, 1f, 0f, 0f, 0f, 3f)
                });
            }
            if (bottom && right)
            {
                result.AddObject(new BsShape("corner-br")
                {
                    Position = new Vector2(size.x * .5f - 1f, size.y * -.5f + 1f),
                    Path = PathString.Vector(0f, 0f, 0f, -3f, 0f, 1f, 0f, 0f, 0f, 3f)
                });
            }

            return result;
        }

        public static BsMap Platformer(string title, float difficulty, uint seed, bool hasNextLevel)
        {
            // Create map
            var length = difficulty * 50f + 50f;
            var height = difficulty * 10f + 20f;
            var rand = new Random(seed);
            var result = new BsMap(title, Author)
            {
                PlayArea = new Rect(-10f, height * -.5f, length + 40f, height),
                GravityDirection = Direction.Down,
                GravityStrength = 20f
            };

            // Add spawns
            result.AddSpawn(new BsSpawn(new Vector2(-3f, .5f), 0));
            result.AddSpawn(new BsSpawn(new Vector2(-1f, .5f), 0));
            result.AddSpawn(new BsSpawn(new Vector2(1f, .5f), 0));
            result.AddSpawn(new BsSpawn(new Vector2(3f, .5f), 0));

            // Add start and finish platforms
            result.AddObject(new BsDecal("spawn-bg")
            {
                Layer = -1,
                Path = PathString.Vector(-5f, 0f, 0, 5f, 0f, 1, 5f, -3f, 0f, -3f, 1, -5f, -3f, -5f, 0f),
                Color = ColorExt.Lava.MultiplyTint(.5f)
            });
            result.AddObject(new BsShape("spawn-platform")
            {
                Path = PathString.Polygon(-2.5f, -1f, -5f, 0f, 5f, 0f, 2.5f, -1f),
                Material = MaterialExt.Metal,
                Color = ColorExt.Lava
            });
            result.AddObject(new BsDecal("spawn-beacon")
            {
                Position = new Vector2(0f, height * .5f),
                Layer = 1,
                Path = PathString.Rectangle(10f, height),
                Color = ColorExt.Lava.SetAlpha(.1f)
            });
            result.AddObject(new BsDecal("goal-bg")
            {
                Position = new Vector2(length + 20f, 0f),
                Path = PathString.Vector(-5f, 0f, 0, 5f, 0f, 1, 5f, -3f, 0f, -3f, 1, -5f, -3f, -5f, 0f),
                Layer = -1,
                Color = ColorExt.Medicine.MultiplyTint(.5f)
            });
            result.AddObject(new BsShape("goal-platform")
            {
                Position = new Vector2(length + 20f, 0f),
                Path = PathString.Polygon(-2.5f, -1f, -5f, 0f, 5f, 0f, 2.5f, -1f),
                Material = MaterialExt.Metal,
                Color = ColorExt.Medicine
            });
            result.AddObject(new BsDecal("goal-beacon")
            {
                Position = new Vector2(length + 20f, height * .5f),
                Layer = 1,
                Path = PathString.Rectangle(10f, height),
                Color = ColorExt.Medicine.SetAlpha(.1f)
            });

            // Add main platforms
            var cursor = 5f;
            var limit = length + 15f;
            var platform = 0;
            while (cursor < limit)
            {
                cursor += Mathf.Pow(rand.NextFloat(-1.5f, 1.5f), 3f) + difficulty * 5f;
                if (cursor >= limit) break;
                var platformLength = Mathf.Min(Mathf.Pow(rand.NextFloat(-1.5f, 1.5f), 3f)
                    + Mathf.Max(20f - difficulty, 4f), limit - cursor - 1f);
                var platformHeight = rand.NextFloat(height * -.25f, height * .25f);
                var platformIce = rand.NextFloat(Mathf.Max(26f - difficulty * 2f, 1f)) < 1f;
                platform++;
                cursor += platformLength * .5f;
                var material = platformIce ? MaterialExt.Ice : MaterialExt.Stone;
                var color = platformIce ? ColorExt.Ice : ColorExt.Stone;
                result.AddObject(new BsShape($"platform-{platform}")
                {
                    Position = new Vector2(cursor, platformHeight),
                    Path = PathString.Polygon(0f, platformLength * difficulty * -.5f, platformLength * -.5f, 0f,
                        platformLength * -.5f, 1f, platformLength * .5f, 1f, platformLength * .5f, 0f),
                    Material = material,
                    Color = color
                });
                result.AddObject(new BsDecal($"beacon-{platform}")
                {
                    Position = new Vector2(cursor, platformHeight + 1f + height * .5f),
                    Layer = 1,
                    Path = PathString.Rectangle(platformLength, height),
                    Color = color.SetAlpha(.05f)
                });
                cursor += platformLength * .5f;
            }

            // Add goal redirect
            var nextLevelTitle = hasNextLevel
                ? Regex.Replace(title, @"\d+", match => $"{int.Parse(match.Value) + 1}")
                : Regex.Replace(title, @"\d+", "Goal");
            result.AddObject(new BsGoal("goal")
            {
                Position = new Vector2(length + 20f, 0f),
                Size = 8f,
                Color = ColorExt.Medicine,
                Redirect = MapSystem.GenerateId(nextLevelTitle, Author)
            });

            return result;
        }

        public static BsMap PlatformerGoal(string title)
        {
            // Create map
            var result = new BsMap(title, Author)
            {
                PlayArea = new Rect(-15f, -15f, 30f, 30f),
                GravityDirection = Direction.Down,
                GravityStrength = 20f
            };

            // Add spawns
            result.AddSpawn(new BsSpawn(new Vector2(-3f, .5f), 0));
            result.AddSpawn(new BsSpawn(new Vector2(-1f, .5f), 0));
            result.AddSpawn(new BsSpawn(new Vector2(1f, .5f), 0));
            result.AddSpawn(new BsSpawn(new Vector2(3f, .5f), 0));

            // Add finish platform
            result.AddObject(new BsDecal("finish-bg")
            {
                Layer = -1,
                Path = PathString.Vector(-5f, 0f, 0, 5f, 0f, 1, 5f, -5f, 0f, -5f, 1, -5f, -5f, -5f, 0f),
                Color = ColorExt.Medkit.MultiplyTint(.5f)
            });
            result.AddObject(new BsShape("finish-platform")
            {
                Path = PathString.Polygon(-2.5f, -1f, -5f, 0f, 5f, 0f, 2.5f, -1f),
                Material = MaterialExt.Metal,
                Color = ColorExt.Medkit
            });
            result.AddObject(new BsDecal("finish-beacon")
            {
                Position = new Vector2(0f, 15f),
                Layer = 1,
                Path = PathString.Rectangle(10f, 30f),
                Color = ColorExt.Medkit.SetAlpha(.1f)
            });

            return result;
        }
    }
}
