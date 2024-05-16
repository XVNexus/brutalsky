using System.Collections.Generic;
using UnityEngine;
using Utils.Ext;

namespace Utils.Path
{
    public class PathString
    {
        public PathType Type { get; private set; }
        public float[] Args { get; private set; }
        public PathNode StartNode { get; set; }

        public PathString(Vector2 startPoint, IEnumerable<PathNode> nodes)
        {
            StartNode = new PathStart(startPoint);
            var currentNode = StartNode;
            foreach (var node in nodes)
            {
                node.Previous = currentNode;
                currentNode = node;
            }
        }

        public static PathString Vector(float[] args)
        {
            var startPoint = new Vector2(args[0], args[1]);
            var nodes = new List<PathNode>();
            for (var i = 2; i < args.Length; i++)
            {
                switch (args[i])
                {
                    case 0f:
                        nodes.Add(new PathLine(args[i + 1], args[i + 2]));
                        i += 2;
                        break;
                    case 1f:
                        nodes.Add(new PathArc(args[i + 1], args[i + 2], args[i + 3], args[i + 4]));
                        i += 4;
                        break;
                    case 2f:
                        nodes.Add(new PathBezier(args[i + 1], args[i + 2], args[i + 3], args[i + 4],
                            args[i + 5], args[i + 6]));
                        i += 6;
                        break;
                }
            }
            var result = new PathString(startPoint, nodes)
            {
                Type = PathType.Vector,
                Args = args
            };
            return result;
        }

        public static PathString Polygon(float[] args)
        {
            var startPoint = new Vector2(args[0], args[1]);
            var nodes = new PathLine[args.Length / 2 - 1];
            for (var i = 2; i < args.Length; i += 2)
            {
                nodes[i / 2 - 1] = new PathLine(new Vector2(args[i], args[i + 1]));
            }
            var result = new PathString(startPoint, nodes)
            {
                Type = PathType.Polygon,
                Args = args
            };
            return result;
        }

        public static PathString Square(float diameter)
        {
            var result = Rectangle(diameter, diameter);
            result.Type = PathType.Square;
            result.Args = new[] { diameter };
            return result;
        }

        public static PathString Rectangle(float width, float height)
        {
            var result = Polygon(new[]
            {
                -width * .5f, height * .5f,
                width * .5f, height * .5f,
                width * .5f, -height * .5f,
                -width * .5f, -height * .5f
            });
            result.Type = PathType.Rectangle;
            result.Args = new[] { width, height };
            return result;
        }

        public static PathString Circle(float diameter)
        {
            var result = Ellipse(diameter, diameter);
            result.Type = PathType.Circle;
            result.Args = new[] { diameter };
            return result;
        }

        public static PathString Ellipse(float width, float height)
        {
            var result = Vector(new[]
            {
                0f, height * .5f,
                1f, width * .5f, height * .5f, width * .5f, 0f,
                1f, width * .5f, -height * .5f, 0f, -height * .5f,
                1f, -width * .5f, -height * .5f, -width * .5f, 0f,
                1f, -width * .5f, height * .5f, 0f, height * .5f
            });
            result.Type = PathType.Ellipse;
            result.Args = new[] { width, height };
            return result;
        }

        public static PathString Ngon(int sides, float diameter)
        {
            var args = new float[sides * 2];
            var scale = diameter * .5f;
            for (var i = 0; i < sides; i++)
            {
                var vertexAngle = (i / (float)sides * 2f + .5f) * Mathf.PI;
                args[i * 2] = Mathf.Cos(vertexAngle) * scale;
                args[i * 2 + 1] = Mathf.Sin(vertexAngle) * scale;
            }
            var result = Polygon(args);
            result.Type = PathType.Ngon;
            result.Args = new[] { sides, diameter };
            return result;
        }

        public static PathString Star(int points, float outerDiameter, float innerDiameter)
        {
            var args = new float[points * 4];
            var scales = new[] { outerDiameter * .5f, innerDiameter * .5f };
            for (var i = 0; i < points * 2; i++)
            {
                var vertexAngle = (i / (float)(points * 2) * 2f + .5f) * Mathf.PI;
                var scale = scales[i % 2];
                args[i * 2] = Mathf.Cos(vertexAngle) * scale;
                args[i * 2 + 1] = Mathf.Sin(vertexAngle) * scale;
            }
            var result = Polygon(args);
            result.Type = PathType.Star;
            result.Args = new[] { points, outerDiameter, innerDiameter };
            return result;
        }

        public Vector2[] ToPoints(float rotation)
        {
            var result = new List<Vector2>{MathfExt.RotateVector(StartNode.EndPoint, rotation)};
            var currentNode = StartNode.Next;
            while (currentNode != null)
            {
                for (var i = 1; i <= currentNode.DetailLevel; i++)
                {
                    result.Add(MathfExt.RotateVector(
                        currentNode.SamplePoint(i / (float)currentNode.DetailLevel), rotation));
                }
                currentNode = currentNode.Next;
            }
            if (result[^1] == result[0]) result.RemoveAt(result.Count - 1);
            return result.ToArray();
        }
    }
}
