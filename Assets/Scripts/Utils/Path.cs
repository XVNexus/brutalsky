using System;
using System.Collections.Generic;
using System.Linq;
using Extensions;
using Lcs;
using UnityEngine;
using Utils.Constants;

namespace Utils
{
    public class Path : ILcsProp
    {
        public const int TypeVector = 0;
        public const int TypePolygon = 1;
        public const int TypeSquare = 2;
        public const int TypeRectangle = 3;
        public const int TypeCircle = 4;
        public const int TypeEllipse = 5;
        public const int TypeNgon = 6;
        public const int TypeStar = 7;

        public static float ArcHandleFactor = Mathf.Pow(1f / 6f, 1f / 3f);

        public int Type { get; private set; }
        public float[] Args { get; private set; } = Array.Empty<float>();
        public List<Vector2> Points { get; private set; } = new();

        public Path(int type, float[] args)
        {
            Type = type;
            Args = args;
        }

        public Path() { }

        public static Path Vector(params float[] args)
        {
            var result = new Path(TypeVector, args);
            result.StartAt(args[0], args[1]);
            for (var i = 2; i < args.Length; i++)
            {
                switch (args[i])
                {
                    case 0f:
                        result.LineTo(args[i + 1], args[i + 2]);
                        i += 2;
                        break;
                    case 1f:
                        result.ArcTo(args[i + 1], args[i + 2], args[i + 3], args[i + 4]);
                        i += 4;
                        break;
                    case 2f:
                        result.QuadraticTo(args[i + 1], args[i + 2], args[i + 3], args[i + 4]);
                        i += 4;
                        break;
                    case 3f:
                        result.CubicTo(args[i + 1], args[i + 2], args[i + 3], args[i + 4], args[i + 5], args[i + 6]);
                        i += 6;
                        break;
                }
            }
            return result;
        }

        public static Path Polygon(params float[] args)
        {
            var result = new Path(TypePolygon, args);
            result.StartAt(args[0], args[1]);
            for (var i = 2; i < args.Length; i += 2)
            {
                result.LineTo(args[i], args[i + 1]);
            }
            return result;
        }

        public static Path Square(float diameter)
        {
            var radius = diameter * .5f;
            var result = new Path(TypeSquare, new[] { radius });
            result.StartAt(-radius, radius);
            result.LineTo(radius, radius);
            result.LineTo(radius, -radius);
            result.LineTo(-radius, -radius);
            return result;
        }

        public static Path Rectangle(float diameterX, float diameterY)
        {
            var radiusX = diameterX * .5f;
            var radiusY = diameterY * .5f;
            var result = new Path(TypeRectangle, new[] { radiusX, radiusY });
            result.StartAt(-radiusX, radiusY);
            result.LineTo(radiusX, radiusY);
            result.LineTo(radiusX, -radiusY);
            result.LineTo(-radiusX, -radiusY);
            return result;
        }

        public static Path Circle(float diameter)
        {
            var radius = diameter * .5f;
            var result = new Path(TypeCircle, new[] { radius });
            result.StartAt(0f, radius);
            result.ArcTo(radius, radius, radius, 0f);
            result.ArcTo(radius, -radius, 0f, -radius);
            result.ArcTo(-radius, -radius, -radius, 0f);
            result.ArcTo(-radius, radius, 0f, radius);
            return result;
        }

        public static Path Ellipse(float diameterX, float diameterY)
        {
            var radiusX = diameterX * .5f;
            var radiusY = diameterY * .5f;
            var result = new Path(TypeEllipse, new[] { radiusX, radiusY });
            result.StartAt(0f, radiusY);
            result.ArcTo(radiusX, radiusY, radiusX, 0f);
            result.ArcTo(radiusX, -radiusY, 0f, -radiusY);
            result.ArcTo(-radiusX, -radiusY, -radiusX, 0f);
            result.ArcTo(-radiusX, radiusY, 0f, radiusY);
            return result;
        }

        public static Path Ngon(int points, float diameter)
        {
            var radius = diameter * .5f;
            var result = new Path(TypeNgon, new[] { points, radius });
            result.StartAt(0f, radius);
            for (var i = 1; i < points; i++)
            {
                var vertexAngle = (i / (float)points * 2f + .5f) * Mathf.PI;
                result.LineTo(new Vector2(Mathf.Cos(vertexAngle), Mathf.Sin(vertexAngle)) * radius);
            }
            return result;
        }

        public static Path Star(int points, float diameterInner, float diameterOuter)
        {
            var radiusInner = diameterInner * .5f;
            var radiusOuter = diameterOuter * .5f;
            var result = new Path(TypeStar, new[] { points, radiusInner, radiusOuter });
            result.StartAt(0f, radiusInner);
            var pointsReal = points * 2;
            var radii = new[] { radiusInner, radiusOuter };
            for (var i = 1; i < pointsReal; i++)
            {
                var vertexAngle = (i / (float)pointsReal * 2f + .5f) * Mathf.PI;
                var radius = radii[i % 2];
                result.LineTo(new Vector2(Mathf.Cos(vertexAngle), Mathf.Sin(vertexAngle)) * radius);
            }
            return result;
        }

        public Vector2[] GetPoints(float rotation)
        {
            return Points.Select(point => MathfExt.RotateVector(point, rotation)).ToArray();
        }

        public void StartAt(float x, float y)
        {
            StartAt(new Vector2(x, y));
        }

        public void StartAt(Vector2 point)
        {
            Points.Add(point);
        }

        public void LineTo(float x, float y)
        {
            LineTo(new Vector2(x, y));
        }

        public void LineTo(Vector2 point)
        {
            Points.Add(point);
        }

        public void ArcTo(float handleX, float handleY, float x, float y)
        {
            ArcTo(new Vector2(handleX, handleY), new Vector2(x, y));
        }

        public void ArcTo(Vector2 handle, Vector2 point)
        {
            CubicTo(MathfExt.Lerp(Points[^1], handle, ArcHandleFactor),
                MathfExt.Lerp(point, handle, ArcHandleFactor), point);
        }

        public void QuadraticTo(float handleX, float handleY, float x, float y)
        {
            QuadraticTo(new Vector2(handleX, handleY), new Vector2(x, y));
        }

        public void QuadraticTo(Vector2 handle, Vector2 point)
        {
            var from = Points[^1];
            var length = ((point - from).magnitude + (handle - from).magnitude + (point - handle).magnitude) * .5f;
            var detail = Mathf.CeilToInt(length * 2f * Mathf.PI);
            for (var i = 1; i <= detail; i++)
            {
                var t = i / (float)detail;
                var p01 = MathfExt.Lerp(from, handle, t);
                var p12 = MathfExt.Lerp(handle, point, t);
                Points.Add(MathfExt.Lerp(p01, p12, t));
            }
        }

        public void CubicTo(float handle1X, float handle1Y, float handle2X, float handle2Y, float x, float y)
        {
            CubicTo(new Vector2(handle1X, handle1Y), new Vector2(handle2X, handle2Y), new Vector2(x, y));
        }

        public void CubicTo(Vector2 handle1, Vector2 handle2, Vector2 point)
        {
            var from = Points[^1];
            var length = ((point - from).magnitude + (handle1 - from).magnitude
                + (handle2 - handle1).magnitude + (point - handle2).magnitude) * .5f;
            var detail = Mathf.CeilToInt(length * 2f * Mathf.PI);
            for (var i = 1; i <= detail; i++)
            {
                var t = i / (float)detail;
                var p01 = MathfExt.Lerp(from, handle1, t);
                var p12 = MathfExt.Lerp(handle1, handle2, t);
                var p23 = MathfExt.Lerp(handle2, point, t);
                var p012 = MathfExt.Lerp(p01, p12, t);
                var p123 = MathfExt.Lerp(p12, p23, t);
                Points.Add(MathfExt.Lerp(p012, p123, t));
            }
        }

        public object _ToLcs()
        {
            var result = new object[Args.Length + 1];
            result[0] = Type;
            for (var i = 0; i < Args.Length; i++)
            {
                result[i + 1] = Args[i];
            }
            return result;
        }

        public void _FromLcs(object prop)
        {
            var parts = (object[])prop;
            var type = (int)parts[0];
            var args = new float[parts.Length - 1];
            for (var i = 1; i < parts.Length; i++)
            {
                args[i - 1] = (float)parts[i];
            }
            var result = type switch
            {
                TypeVector => Vector(args),
                TypePolygon => Vector(args),
                TypeSquare => Vector(args),
                TypeRectangle => Vector(args),
                TypeCircle => Vector(args),
                TypeEllipse => Vector(args),
                TypeNgon => Vector(args),
                TypeStar => Vector(args),
                _ => throw Errors.InvalidItem("path type", type)
            };
            Type = result.Type;
            Args = result.Args;
            Points = result.Points;
        }
    }
}
