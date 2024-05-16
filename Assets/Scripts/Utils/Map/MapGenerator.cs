using Brutalsky;
using Brutalsky.Map;
using Brutalsky.Object;
using UnityEngine;
using Utils.Constants;
using Utils.Ext;
using Utils.Object;
using Utils.Shape;
using Random = Unity.Mathematics.Random;

namespace Utils.Map
{
    public static class MapGenerator
    {
        public static readonly Color MapBackground = new(.25f, .25f, .25f);
        public static readonly Color MapLighting = new(1f, 1f, 1f, .8f);
        public static readonly ShapeMaterial ObjectMaterial = ShapeMaterial.Stone;
        public static readonly Color ObjectColor = ColorExt.Stone;
        public const string Author = "Brutalsky";

        public static BsMap Box(string title, int shape, Vector2 size)
        {
            var bottom = (shape & 0b1000) > 0;
            var top = (shape & 0b0100) > 0;
            var left = (shape & 0b0010) > 0;
            var right = (shape & 0b0001) > 0;
            var result = new BsMap(title, Author)
            {
                PlayArea = new Rect(size * -.5f, size),
                BackgroundColor = MapBackground,
                LightingColor = MapLighting,
                GravityDirection = Direction.Down,
                GravityStrength = 20f,
                PlayerHealth = 100f,
                AllowDummies = true
            };
            result.AddSpawn(new BsSpawn(new Vector2(-3f, -size.y * .5f + 1.5f), 0));
            result.AddSpawn(new BsSpawn(new Vector2(-1f, -size.y * .5f + 1.5f), 0));
            result.AddSpawn(new BsSpawn(new Vector2(1f, -size.y * .5f + 1.5f), 0));
            result.AddSpawn(new BsSpawn(new Vector2(3f, -size.y * .5f + 1.5f), 0));
            if (bottom) result.AddObject(new BsShape
            (
                "wall-bottom",
                new ObjectTransform(0f, size.y * -.5f + .5f),
                ObjectLayer.Midground,
                true,
                Form.Rectangle(size.x, 1f),
                ObjectMaterial,
                false,
                ObjectColor
            ));
            if (top) result.AddObject(new BsShape
            (
                "wall-top",
                new ObjectTransform(0f, size.y * .5f - .5f),
                ObjectLayer.Midground,
                true,
                Form.Rectangle(size.x, 1f),
                ObjectMaterial,
                false,
                ObjectColor
            ));
            if (left) result.AddObject(new BsShape
            (
                "wall-left",
                new ObjectTransform(size.x * -.5f + .5f, 0f),
                ObjectLayer.Midground,
                true,
                Form.Rectangle(1f, size.y),
                ObjectMaterial,
                false,
                ObjectColor
            ));
            if (right) result.AddObject(new BsShape
            (
                "wall-right",
                new ObjectTransform(size.x * .5f - .5f, 0f),
                ObjectLayer.Midground,
                true,
                Form.Rectangle(1f, size.y),
                ObjectMaterial,
                false,
                ObjectColor
            ));
            if (top && left) result.AddObject(new BsShape
            (
                "corner-tl",
                new ObjectTransform(size.x * -.5f + 1f, size.y * .5f - 1f),
                ObjectLayer.Midground,
                true,
                Form.Vector(new[] { 0f, 0f, 0f, 3f, 0f, 1f, 0f, 0f, 0f, -3f }),
                ObjectMaterial,
                false,
                ObjectColor
            ));
            if (top && right) result.AddObject(new BsShape
            (
                "corner-tr",
                new ObjectTransform(size.x * .5f - 1f, size.y * .5f - 1f),
                ObjectLayer.Midground,
                true,
                Form.Vector(new[] { 0f, 0f, 0f, -3f, 0f, 1f, 0f, 0f, 0f, -3f }),
                ObjectMaterial,
                false,
                ObjectColor
            ));
            if (bottom && left) result.AddObject(new BsShape
            (
                "corner-bl",
                new ObjectTransform(size.x * -.5f + 1f, size.y * -.5f + 1f),
                ObjectLayer.Midground,
                true,
                Form.Vector(new[] { 0f, 0f, 0f, 3f, 0f, 1f, 0f, 0f, 0f, 3f }),
                ObjectMaterial,
                false,
                ObjectColor
            ));
            if (bottom && right) result.AddObject(new BsShape
            (
                "corner-br",
                new ObjectTransform(size.x * .5f - 1f, size.y * -.5f + 1f),
                ObjectLayer.Midground,
                true,
                Form.Vector(new[] { 0f, 0f, 0f, -3f, 0f, 1f, 0f, 0f, 0f, 3f }),
                ObjectMaterial,
                false,
                ObjectColor
            ));
            return result;
        }

        public static BsMap Platformer(string title, float difficulty, uint seed, string nextTitle = "")
        {
            var length = difficulty * 50f + 50f;
            var height = difficulty * 10f + 20f;
            var rand = new Random(seed);
            var result = new BsMap(title, Author)
            {
                PlayArea = new Rect(-10f, height * -.5f, length + 40f, height),
                BackgroundColor = MapBackground,
                LightingColor = MapLighting,
                GravityDirection = Direction.Down,
                GravityStrength = 20f,
                PlayerHealth = 100f,
                AllowDummies = false
            };
            result.AddSpawn(new BsSpawn(new Vector2(-3f, .5f), 0));
            result.AddSpawn(new BsSpawn(new Vector2(-1f, .5f), 0));
            result.AddSpawn(new BsSpawn(new Vector2(1f, .5f), 0));
            result.AddSpawn(new BsSpawn(new Vector2(3f, .5f), 0));
            result.AddObject(new BsShape
            (
                "platform-spawn",
                new ObjectTransform(0f, 0f),
                ObjectLayer.Background,
                true,
                Form.Vector(new[] { -5f, 0f, 0, 5f, 0f, 1, 5f, -5f, 0f, -5f, 1, -5f, -5f, -5f, 0f }),
                ObjectMaterial,
                false,
                ColorExt.Lava.MultiplyTint(.5f),
                true
            ));
            result.AddObject(new BsShape
            (
                "platform-spawn-fg",
                new ObjectTransform(0f, 0f),
                ObjectLayer.Midground,
                false,
                Form.Polygon(new[] { -2.5f, -1f, -5f, 0f, 5f, 0f, 2.5f, -1f }),
                ObjectMaterial,
                false,
                ColorExt.Lava,
                true
            ));
            result.AddObject(new BsShape
            (
                "platform-spawn-beacon",
                new ObjectTransform(0f, height * .5f),
                ObjectLayer.Foreground,
                false,
                Form.Rectangle(10f, height),
                ObjectMaterial,
                false,
                ColorExt.Lava.SetAlpha(.1f),
                true
            ));
            result.AddObject(new BsShape
            (
                "platform-goal",
                new ObjectTransform(length + 20f, 0f),
                ObjectLayer.Background,
                true,
                Form.Vector(new[] { -5f, 0f, 0, 5f, 0f, 1, 5f, -5f, 0f, -5f, 1, -5f, -5f, -5f, 0f }),
                ObjectMaterial,
                false,
                ColorExt.Medicine.MultiplyTint(.5f),
                true
            ));
            result.AddObject(new BsShape
            (
                "platform-goal-fg",
                new ObjectTransform(length + 20f, 0f),
                ObjectLayer.Midground,
                false,
                Form.Polygon(new[] { -2.5f, -1f, -5f, 0f, 5f, 0f, 2.5f, -1f }),
                ObjectMaterial,
                false,
                ColorExt.Medicine,
                true
            ));
            result.AddObject(new BsShape
            (
                "platform-goal-beacon",
                new ObjectTransform(length + 20f, height * .5f),
                ObjectLayer.Foreground,
                false,
                Form.Rectangle(10f, height),
                ObjectMaterial,
                false,
                ColorExt.Medicine.SetAlpha(.1f),
                true
            ));
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
                var material = platformIce ? ShapeMaterial.Ice : ShapeMaterial.Stone;
                var color = platformIce ? ColorExt.Ice : ColorExt.Stone;
                result.AddObject(new BsShape
                (
                    $"platform-{platform}",
                    new ObjectTransform(cursor, platformHeight),
                    ObjectLayer.Midground,
                    true,
                    Form.Polygon(new []
                    {
                        0f, platformLength * difficulty * -.5f,
                        platformLength * -.5f, 0f,
                        platformLength * -.5f, 1f,
                        platformLength * .5f, 1f,
                        platformLength * .5f, 0f
                    }),
                    material,
                    false,
                    color
                ));
                result.AddObject(new BsShape
                (
                    $"platform-{platform}-beacon",
                    new ObjectTransform(cursor, platformHeight + 1f),
                    ObjectLayer.Foreground,
                    false,
                    Form.Polygon(new []
                    {
                        platformLength * -.5f, 0f,
                        platformLength * -.5f, height,
                        platformLength * .5f, height,
                        platformLength * .5f, 0f
                    }),
                    ObjectMaterial,
                    false, 
                    color.SetAlpha(.05f)
                ));
                cursor += platformLength * .5f;
            }
            if (nextTitle.Length > 0)
            {
                result.AddObject(new BsGoal(
                    "goal",
                    new ObjectTransform(length + 20f, 0f),
                    true,
                    8f,
                    ColorExt.Medicine,
                    nextTitle,
                    Author
                ));
            }
            return result;
        }
    }
}
