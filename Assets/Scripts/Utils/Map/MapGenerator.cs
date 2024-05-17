using System.Collections.Generic;
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
                BackgroundColor = new Color(.1f, .1f, .2f),
                LightingColor = new Color(.75f, .75f, 1f, .8f),
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
                    Path = PathString.Rectangle(size.x, 1f),
                    Material = MaterialExt.Stone
                });
            }
            if (top)
            {
                result.AddObject(new BsShape("wall-top")
                {
                    Position = new Vector2(0f, size.y * .5f - .5f),
                    Path = PathString.Rectangle(size.x, 1f),
                    Material = MaterialExt.Stone
                });
            }
            if (left)
            {
                result.AddObject(new BsShape("wall-left")
                {
                    Position = new Vector2(size.x * -.5f + .5f, 0f),
                    Path = PathString.Rectangle(1f, size.y),
                    Material = MaterialExt.Stone
                });
            }
            if (right)
            {
                result.AddObject(new BsShape("wall-right")
                {
                    Position = new Vector2(size.x * .5f - .5f, 0f),
                    Path = PathString.Rectangle(1f, size.y),
                    Material = MaterialExt.Stone
                });
            }

            // Add corners
            if (top && left)
            {
                result.AddObject(new BsShape("corner-tl")
                {
                    Position = new Vector2(size.x * -.5f + 1f, size.y * .5f - 1f),
                    Path = PathString.Vector(0f, 0f, 0f, 3f, 0f, 1f, 0f, 0f, 0f, -3f),
                    Material = MaterialExt.Stone
                });
            }
            if (top && right)
            {
                result.AddObject(new BsShape("corner-tr")
                {
                    Position = new Vector2(size.x * .5f - 1f, size.y * .5f - 1f),
                    Path = PathString.Vector(0f, 0f, 0f, -3f, 0f, 1f, 0f, 0f, 0f, -3f),
                    Material = MaterialExt.Stone
                });
            }
            if (bottom && left)
            {
                result.AddObject(new BsShape("corner-bl")
                {
                    Position = new Vector2(size.x * -.5f + 1f, size.y * -.5f + 1f),
                    Path = PathString.Vector(0f, 0f, 0f, 3f, 0f, 1f, 0f, 0f, 0f, 3f),
                    Material = MaterialExt.Stone
                });
            }
            if (bottom && right)
            {
                result.AddObject(new BsShape("corner-br")
                {
                    Position = new Vector2(size.x * .5f - 1f, size.y * -.5f + 1f),
                    Path = PathString.Vector(0f, 0f, 0f, -3f, 0f, 1f, 0f, 0f, 0f, 3f),
                    Material = MaterialExt.Stone
                });
            }

            return result;
        }

        public static BsMap Parkour(string title, float difficulty, uint seed, bool hasNextLevel)
        {
            // Create map
            var size = new Vector2(difficulty * 50f + 50f, difficulty * 10f + 20f);
            var spawnX = size.x * -.5f + 10f;
            var goalX = size.x * .5f - 10f;
            var diffFraction = (difficulty - 1f) / 11f;
            var rand = new Random(seed);
            var result = new BsMap(title, Author)
            {
                PlayArea = new Rect(size * -.5f, size),
                BackgroundColor = Color.HSVToRGB((1f - diffFraction) * 1f / 3f, .25f, .2f),
                LightingColor = Color.white.SetAlpha(.75f),
                GravityDirection = Direction.Down,
                GravityStrength = 20f,
                AirResistance = .5f
            };

            // Add spawns
            result.AddSpawn(new BsSpawn(new Vector2(spawnX - 3f, .5f), 0));
            result.AddSpawn(new BsSpawn(new Vector2(spawnX - 1f, .5f), 1));
            result.AddSpawn(new BsSpawn(new Vector2(spawnX + 1f, .5f), 2));
            result.AddSpawn(new BsSpawn(new Vector2(spawnX + 3f, .5f), 3));

            // Add start and finish platforms
            result.AddObject(new BsDecal("spawn-bg")
            {
                Position = new Vector2(spawnX, 0f),
                Layer = -1,
                Path = PathString.Vector(-5f, 0f, 0, 5f, 0f, 1, 5f, -3f, 0f, -3f, 1, -5f, -3f, -5f, 0f),
                Color = ColorExt.Lava.MultiplyTint(.5f)
            });
            result.AddObject(new BsShape("spawn-platform")
            {
                Position = new Vector2(spawnX, 0f),
                Path = PathString.Polygon(-2.5f, -1f, -5f, 0f, 5f, 0f, 2.5f, -1f),
                Material = MaterialExt.Metal,
                Color = ColorExt.Lava,
                Glow = true
            });
            result.AddObject(new BsDecal("spawn-beacon")
            {
                Position = new Vector2(spawnX, size.y * .5f),
                Layer = -1,
                Path = PathString.Rectangle(10f, size.y),
                Color = ColorExt.Lava.SetAlpha(.1f)
            });
            result.AddObject(new BsDecal("goal-bg")
            {
                Position = new Vector2(goalX, 0f),
                Path = PathString.Vector(-5f, 0f, 0, 5f, 0f, 1, 5f, -3f, 0f, -3f, 1, -5f, -3f, -5f, 0f),
                Layer = -1,
                Color = ColorExt.Medicine.MultiplyTint(.5f)
            });
            result.AddObject(new BsShape("goal-platform")
            {
                Position = new Vector2(goalX, 0f),
                Path = PathString.Polygon(-2.5f, -1f, -5f, 0f, 5f, 0f, 2.5f, -1f),
                Material = MaterialExt.Metal,
                Color = ColorExt.Medicine,
                Glow = true
            });
            result.AddObject(new BsDecal("goal-beacon")
            {
                Position = new Vector2(goalX, size.y * .5f),
                Layer = -1,
                Path = PathString.Rectangle(10f, size.y),
                Color = ColorExt.Medicine.SetAlpha(.1f)
            });

            // Generate placements for main platforms
            var limit = goalX - 5f;
            var cursor = spawnX + 5f;
            var placements = new List<Rect>();
            while (cursor < limit)
            {
                var spacing = MathfExt.BellPoint(rand.NextFloat(-1f, 1f), difficulty * 5f, difficulty * 2f);
                var length = MathfExt.BellPoint(rand.NextFloat(-1f, 1f), 26f - difficulty * 2f, 13f - difficulty);
                if (cursor + spacing + length >= limit) break;
                var ice = rand.NextFloat(1f / diffFraction) < 1f;
                placements.Add(new Rect(cursor + spacing, rand.NextFloat(size.y * -.25f, size.y * .25f),
                    length, ice ? 1f : 0f));
                cursor += spacing + length;
            }
            var posMin = placements[0].xMin;
            var posMax = placements[^1].xMax;
            var centerOffset = (posMin + posMax) / 2f;
            for (var i = 0; i < placements.Count; i++)
            {
                var placement = placements[i];
                placement.x -= centerOffset;
                placements[i] = placement;
            }

            // Add main platforms
            for (var i = 0; i < placements.Count; i++)
            {
                var rect = placements[i];
                var position = MathfExt.Round(rect.center);
                var length = Mathf.Round(rect.width / 2f) * 2f;
                var material = Mathf.Approximately(rect.height, 1f) ? MaterialExt.Ice : MaterialExt.Stone;
                var color = Mathf.Approximately(rect.height, 1f) ? ColorExt.Ice : ColorExt.Stone;
                result.AddObject(new BsShape($"platform-{i + 1}")
                {
                    Position = position + new Vector2(0f, .5f),
                    Path = PathString.Rectangle(length, 1f),
                    Material = material,
                    Color = color
                });
                result.AddObject(new BsShape($"spike-{i + 1}")
                {
                    Position = position,
                    Layer = -1,
                    Path = PathString.Polygon(0f, length * difficulty * -.5f, length * -.5f, 0f, length * .5f, 0f),
                    Material = (0f, 0f, 0f, 0f, -100f),
                    Color = color.MultiplyTint(.5f)
                });
                result.AddObject(new BsDecal($"decal-{i + 1}")
                {
                    Position = position,
                    Path = PathString.Polygon(0f, length * difficulty * -.25f, length * -.4f, 0f, length * .4f, 0f),
                    Color = color.MultiplyTint(.75f)
                });
                result.AddObject(new BsDecal($"beacon-{i + 1}")
                {
                    Position = position + new Vector2(0f, 1f + size.y * .5f),
                    Layer = -1,
                    Path = PathString.Rectangle(length, size.y),
                    Color = color.SetAlpha(.05f)
                });
            }

            // Add goal redirect
            var nextLevelTitle = hasNextLevel
                ? Regex.Replace(title, @"\d+", match => $"{int.Parse(match.Value) + 1}")
                : Regex.Replace(title, @"\d+", "Finish");
            result.AddObject(new BsGoal("goal")
            {
                Position = new Vector2(goalX, size.y * .5f),
                Size = new Vector2(6f, size.y),
                Color = ColorExt.Medicine,
                Redirect = MapSystem.GenerateId(nextLevelTitle, Author)
            });

            return result;
        }

        public static BsMap ParkourFinish(string title)
        {
            // Create map
            var result = new BsMap(title, Author)
            {
                PlayArea = new Rect(-15f, -15f, 30f, 30f),
                BackgroundColor = Color.HSVToRGB(.5f, .25f, .2f),
                LightingColor = Color.white.SetAlpha(.75f),
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
                Color = ColorExt.Medkit,
                Glow = true
            });
            result.AddObject(new BsDecal("finish-beacon")
            {
                Position = new Vector2(0f, 15f),
                Layer = -1,
                Path = PathString.Rectangle(10f, 30f),
                Color = ColorExt.Medkit.SetAlpha(.1f)
            });

            return result;
        }
    }
}
