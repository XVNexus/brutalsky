using System;
using System.Collections.Generic;
using Brutalsky.Scripts.Data;
using Brutalsky.Scripts.Extensions;
using Brutalsky.Scripts.Utils;
using Godot;

namespace Brutalsky.Scripts.Maps;

public class MapParkour : MapGenerator
{
    public int Number { get; set; }
    public float Difficulty { get; set; }
    public int Seed { get; set; }
    public bool HasNext { get; set; }

    public override BsMap Generate()
    {
        // Create map
        var size = new Vector2(Difficulty * 50f + 50f, Difficulty * 10f + 20f);
        var spawnX = size.X * -.5f + 10f;
        var goalX = size.X * .5f - 10f;
        var diffFraction = (Difficulty - 1f) / 11f;
        var rand = new Random(Seed);
        var result = new BsMap($"Parkour {Number}", AuthorGenerated)
        {
            PlayArea = new Rect2(size * -.5f, size),
            BackgroundColor = Color.FromHsv((1f - diffFraction) * 2f / 3f, .1f, .2f),
            LightingColor = new Color(.75f, .75f, .75f),
            InitialGravity = new Vector2(0f, -20f),
            InitialAtmosphere = .5f
        };

        // Add spawns
        result.Spawns.Add(new BsSpawn(new Vector2(spawnX - 3f, .5f), 0));
        result.Spawns.Add(new BsSpawn(new Vector2(spawnX - 1f, .5f), 1));
        result.Spawns.Add(new BsSpawn(new Vector2(spawnX + 1f, .5f), 2));
        result.Spawns.Add(new BsSpawn(new Vector2(spawnX + 3f, .5f), 3));

        // Add start and finish platforms
        result.Objects.Add(new BsDecal("spawn-bg")
        {
            Position = new Vector2(spawnX, 0f),
            Layer = -1,
            Path = Path.Vector(-5f, 0f, 0, 5f, 0f, 1, 5f, -3f, 0f, -3f, 1, -5f, -3f, -5f, 0f),
            Color = ColorExt.Lava.MultiplyTint(.5f)
        });
        result.Objects.Add(new BsShape("spawn-platform")
        {
            Position = new Vector2(spawnX, 0f),
            Path = Path.Polygon(-2.5f, -1f, -5f, 0f, 5f, 0f, 2.5f, -1f),
            Material = MaterialExt.Metal,
            Color = ColorExt.Lava,
            Glow = true
        });
        result.Objects.Add(new BsDecal("spawn-beacon")
        {
            Position = new Vector2(spawnX, size.Y * .5f),
            Layer = -1,
            Path = Path.Rectangle(10f, size.Y),
            Color = ColorExt.Lava.SetAlpha(.1f)
        });
        result.Objects.Add(new BsDecal("goal-bg")
        {
            Position = new Vector2(goalX, 0f),
            Path = Path.Vector(-5f, 0f, 0, 5f, 0f, 1, 5f, -3f, 0f, -3f, 1, -5f, -3f, -5f, 0f),
            Layer = -1,
            Color = ColorExt.Medicine.MultiplyTint(.5f)
        });
        result.Objects.Add(new BsShape("goal-platform")
        {
            Position = new Vector2(goalX, 0f),
            Path = Path.Polygon(-2.5f, -1f, -5f, 0f, 5f, 0f, 2.5f, -1f),
            Material = MaterialExt.Metal,
            Color = ColorExt.Medicine,
            Glow = true
        });
        result.Objects.Add(new BsDecal("goal-beacon")
        {
            Position = new Vector2(goalX, size.Y * .5f),
            Layer = -1,
            Path = Path.Rectangle(10f, size.Y),
            Color = ColorExt.Medicine.SetAlpha(.1f)
        });

        // Generate placements for main platforms
        var limit = goalX - 5f;
        var cursor = spawnX + 5f;
        var placements = new List<Rect2>();
        while (cursor < limit)
        {
            var spacing = MathfExt.BellPoint(rand.NextSingle(-1f, 1f), Difficulty * 5f, Difficulty * 2f);
            var length = MathfExt.BellPoint(rand.NextSingle(-1f, 1f), 26f - Difficulty * 2f, 13f - Difficulty);
            if (cursor + spacing + length >= limit) break;
            var ice = rand.NextSingle(1f / Mathf.Pow(diffFraction, 2f)) < 1f;
            placements.Add(new Rect2(cursor + spacing, rand.NextSingle(size.Y * -.25f, size.Y * .25f),
                length, ice ? 1f : 0f));
            cursor += spacing + length;
        }

        var averageSpacing = 0f;
        for (var i = 1; i < placements.Count; i++)
        {
            averageSpacing += placements[i].Position.X - placements[i - 1].Corner().X;
        }

        averageSpacing /= placements.Count - 1f;
        var rangeFrom = new Vector2(placements[0].Position.X, placements[^1].Corner().X);
        var rangeTo = new Vector2(spawnX + 5f + averageSpacing, goalX - 5f - averageSpacing);
        var scaleFactor = (rangeTo.Y - rangeFrom.X) / (rangeFrom.Y - rangeFrom.X);
        for (var i = 0; i < placements.Count; i++)
        {
            var placement = placements[i];
            placement.Position += new Vector2(
                MathfExt.Relerp(placement.GetCenter().X, rangeFrom, rangeTo) - placement.GetCenter().X, 0f);
            placement = placement.Resize(new Vector2(placement.Size.X * scaleFactor, placement.Size.Y));
            placements[i] = placement;
        }

        // Add main platforms
        for (var i = 0; i < placements.Count; i++)
        {
            var rect = placements[i];
            var position = MathfExt.Round(rect.GetCenter());
            var length = Mathf.Round(rect.Size.X / 2f) * 2f;
            var material = MathfExt.Approximately(rect.Size.Y, 1f) ? MaterialExt.Ice : MaterialExt.Stone;
            var color = MathfExt.Approximately(rect.Size.Y, 1f) ? ColorExt.Ice : ColorExt.Stone;
            result.Objects.Add(new BsShape($"platform-{i + 1}")
            {
                Position = position + new Vector2(0f, .5f),
                Path = Path.Rectangle(length, 1f),
                Material = material,
                Color = color
            });
            result.Objects.Add(new BsShape($"spike-{i + 1}")
            {
                Position = position,
                Layer = -1,
                Path = Path.Polygon(0f, length * Difficulty * -.5f, length * -.5f, 0f, length * .5f, 0f),
                Material = (0f, 0f, 0f, 0f, -100f),
                Color = color.MultiplyTint(.5f)
            });
            result.Objects.Add(new BsDecal($"decal-{i + 1}")
            {
                Position = position,
                Path = Path.Polygon(0f, length * Difficulty * -.25f, length * -.4f, 0f, length * .4f, 0f),
                Color = color.MultiplyTint(.75f)
            });
            result.Objects.Add(new BsDecal($"beacon-{i + 1}")
            {
                Position = position + new Vector2(0f, 1f + size.Y * .5f),
                Layer = -1,
                Path = Path.Rectangle(length, size.Y),
                Color = color.SetAlpha(.05f)
            });
        }

        // Add goal redirect
        var nextLevelTitle = HasNext ? $"Parkour {Number + 1}" : "Parkour Finish";
        result.Objects.Add(new BsGoal("goal")
        {
            Position = new Vector2(goalX + 2.75f, size.Y * .5f),
            Size = new Vector2(4.5f, size.Y),
            Color = ColorExt.Medicine,
            Redirect = Tags.GenerateId(nextLevelTitle, AuthorGenerated)
        });

        return result;
    }
}
