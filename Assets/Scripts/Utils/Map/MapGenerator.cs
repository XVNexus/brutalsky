using Brutalsky;
using Brutalsky.Map;
using Brutalsky.Object;
using UnityEngine;
using Utils.Constants;
using Utils.Ext;
using Utils.Object;
using Utils.Shape;

namespace Utils.Map
{
    public static class MapGenerator
    {
        public static readonly Color MapBackground = new(.3f, .3f, .3f);
        public static readonly Color MapLighting = new(1f, 1f, 1f, .8f);
        public static readonly ShapeMaterial ObjectMaterial = ShapeMaterial.Stone;
        public static readonly Color ObjectColor = ColorExt.Stone;

        public static BsMap Box(string title, int shape, Vector2 size)
        {
            var bottom = (shape & 0b1000) > 0;
            var top = (shape & 0b0100) > 0;
            var left = (shape & 0b0010) > 0;
            var right = (shape & 0b0001) > 0;
            var result = new BsMap(title, "Brutalsky")
            {
                PlayArea = new Rect(size * -.5f, size),
                BackgroundColor = MapBackground,
                LightingColor = MapLighting,
                GravityDirection = Direction.Down,
                GravityStrength = 20f,
                PlayerHealth = 100f
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
                ObjectColor
            ));
            return result;
        }
    }
}
