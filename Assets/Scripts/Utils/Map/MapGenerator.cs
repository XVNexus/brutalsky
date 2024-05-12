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
        public static readonly Color MapBackground = new(.2f, .2f, .2f);
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

        public static BsMap Platformer(string title, float length, uint seed, string nextTitle = "")
        {
            var rand = new Random(seed);
            var result = new BsMap(title, Author)
            {
                PlayArea = new Rect(-10f, -10f, length + 40f, 20f),
                BackgroundColor = MapBackground,
                LightingColor = MapLighting,
                GravityDirection = Direction.Down,
                GravityStrength = 20f,
                PlayerHealth = 100f,
                AllowDummies = false
            };
            result.AddSpawn(new BsSpawn(new Vector2(-3f, 1f), 0));
            result.AddSpawn(new BsSpawn(new Vector2(-1f, 1f), 0));
            result.AddSpawn(new BsSpawn(new Vector2(1f, 1f), 0));
            result.AddSpawn(new BsSpawn(new Vector2(3f, 1f), 0));
            result.AddObject(new BsShape
            (
                "platform-spawn",
                new ObjectTransform(0f, 0f),
                ObjectLayer.Midground,
                true,
                Form.Rectangle(10f, 1f),
                ObjectMaterial,
                false,
                ColorExt.Lava,
                true
            ));
            result.AddObject(new BsShape
            (
                "platform-goal",
                new ObjectTransform(length + 20f, 0f),
                ObjectLayer.Midground,
                true,
                Form.Rectangle(10f, 1f),
                ObjectMaterial,
                false,
                ColorExt.Medicine,
                true
            ));
            var cursor = 10f;
            var platform = 0;
            while (cursor < length)
            {
                var platformSpacing = Mathf.Pow(rand.NextFloat(), 3f) * 10f + 10f;
                var platformLength = Mathf.Pow(rand.NextFloat(-1.5f, 1.5f), 3f) + 5f;
                var platformHeight = rand.NextFloat(-5f, 5f);
                cursor += platformSpacing;
                if (cursor >= length) break;
                platform++;
                result.AddObject(new BsShape
                (
                    $"platform-{platform}",
                    new ObjectTransform(cursor + platformSpacing * .5f, platformHeight),
                    ObjectLayer.Midground,
                    true,
                    Form.Rectangle(platformLength, 1f),
                    ObjectMaterial,
                    false,
                    ObjectColor
                ));
                cursor += platformLength;
            }
            if (nextTitle.Length > 0)
            {
                result.AddObject(new BsGoal(
                    "goal",
                    new ObjectTransform(length + 20f, 2.5f),
                    true,
                    5f,
                    ColorExt.Medicine,
                    nextTitle,
                    Author
                ));
            }
            return result;
        }
    }
}
