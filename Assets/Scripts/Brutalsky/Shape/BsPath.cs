using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using Utils;

namespace Brutalsky.Shape
{
    public class BsPath
    {
        public BsPathNode startNode { get; set; }
        public string pathString { get; private set; } = "";

        public BsPath(Vector2 startpoint, IEnumerable<BsPathNode> nodes)
        {
            startNode = new BsPathStart(startpoint);
            var currentNode = startNode;
            foreach (var node in nodes)
            {
                node.previous = currentNode;
                currentNode = node;
            }
        }

        public static BsPath Path(string pathString)
        {
            var parts = new Regex(@" (?=[LC])").Split(pathString.ToUpper());
            if (parts.Length < 3)
            {
                throw Errors.MissingPathParams("path", 3);
            }
            var startValues = parts[0].Split(' ');
            var startpoint = new Vector2(float.Parse(startValues[0]), float.Parse(startValues[1]));
            var nodes = new List<BsPathNode>();
            for (var i = 1; i < parts.Length; i++)
            {
                var values = parts[i].Split(' ');
                var nodeType = values[0];
                switch (nodeType)
                {
                    case "L":
                        if (values.Length < 3)
                        {
                            throw Errors.MissingPathParams("line node", 2);
                        }
                        nodes.Add(new BsPathLine(float.Parse(values[1]), float.Parse(values[2])));
                        break;
                    case "C":
                        if (values.Length < 5)
                        {
                            throw Errors.MissingPathParams("curve node", 4);
                        }
                        nodes.Add(new BsPathCurve(float.Parse(values[1]), float.Parse(values[2]),
                            float.Parse(values[3]), float.Parse(values[4])));
                        break;
                }
            }

            var result = new BsPath(startpoint, nodes);
            result.pathString = $"X {pathString.ToUpper()}";
            return result;
        }

        public static BsPath Polygon(Vector2[] points)
        {
            if (points.Length < 3)
            {
                throw Errors.MissingPathParams("polygon", 3);
            }
            var startpoint = points[0];
            var nodes = new BsPathLine[points.Length - 1];
            for (var i = 1; i < points.Length; i++)
            {
                nodes[i - 1] = new BsPathLine(points[i]);
            }

            var result = new BsPath(startpoint, nodes);
            result.pathString = points.Aggregate("Y", (current, point) => current + $" {point.x} {point.y}");
            return result;
        }

        public static BsPath Square(float diameter)
        {
            var result = Rectangle(diameter, diameter);
            result.pathString = $"S {diameter}";
            return result;
        }

        public static BsPath Rectangle(float width, float height)
        {
            var result = Polygon(new Vector2[]
            {
                new(-width / 2f, height / 2f),
                new(width / 2f, height / 2f),
                new(width / 2f, -height / 2f),
                new(-width / 2f, -height / 2f)
            });
            result.pathString = $"R {width} {height}";
            return result;
        }

        public static BsPath Circle(float diameter)
        {
            var result = Ellipse(diameter, diameter);
            result.pathString = $"C {diameter}";
            return result;
        }

        public static BsPath Ellipse(float width, float height)
        {
            var result = new BsPath(new Vector2(0f, height / 2f), new BsPathNode[]
            {
                new BsPathCurve(width / 2f, height / 2f, width / 2f, 0f),
                new BsPathCurve(width / 2f, -height / 2f, 0f, -height / 2f),
                new BsPathCurve(-width / 2f, -height / 2f, -width / 2f, 0f),
                new BsPathCurve(-width / 2f, height / 2f, 0f, height / 2f)
            });
            result.pathString = $"E {width} {height}";
            return result;
        }

        public static BsPath Ngon(int sides, float diameter)
        {
            var vertices = new Vector2[sides];
            for (var i = 0; i < sides; i++)
            {
                var vertexAngle = i / (float)sides * 2f * Mathf.PI;
                vertices[i] = new Vector2(Mathf.Cos(vertexAngle), Mathf.Sin(vertexAngle)) * (diameter / 2f);
            }

            var result = Polygon(vertices);
            result.pathString = $"N {sides} {diameter}";
            return result;
        }

        public static BsPath Star(int points, float outerDiameter, float innerDiameter)
        {
            var vertices = new Vector2[points * 2];
            for (var i = 0; i < points * 2; i++)
            {
                var vertexAngle = i / (float)(points * 2) * 2f * Mathf.PI;
                var diameter = i % 2 == 0 ? outerDiameter : innerDiameter;
                vertices[i] = new Vector2(Mathf.Cos(vertexAngle), Mathf.Sin(vertexAngle)) * (diameter / 2f);
            }

            var result = Polygon(vertices);
            result.pathString = $"T {points} {outerDiameter} {innerDiameter}";
            return result;
        }

        public Vector2[] ToPoints()
        {
            var result = new List<Vector2>{startNode.endpoint};
            var currentNode = startNode.next;
            while (currentNode != null)
            {
                for (var i = 1; i <= currentNode.detailLevel; i++)
                {
                    result.Add(currentNode.SamplePoint(i / (float)currentNode.detailLevel));
                }
                currentNode = currentNode.next;
            }
            if (result[^1] == result[0]) result.RemoveAt(result.Count - 1);

            return result.ToArray();
        }

        public static BsPath Parse(string raw)
        {
            BsPath result;
            var pathType = raw[0];
            var pathData = raw[2..];
            var pathParts = pathData.Split(' ');
            switch (pathType)
            {
                case 'X':
                    result = Path(pathData);
                    break;
                case 'Y':
                    List<Vector2> polygonPoints = new();
                    for (var i = 0; i < pathParts.Length; i += 2)
                    {
                        polygonPoints.Add(new Vector2(float.Parse(pathParts[i]), float.Parse(pathParts[i + 1])));
                    }
                    result = Polygon(polygonPoints.ToArray());
                    break;
                case 'S':
                    result = Square(float.Parse(pathParts[0]));
                    break;
                case 'R':
                    result = Rectangle(float.Parse(pathParts[0]), float.Parse(pathParts[1]));
                    break;
                case 'C':
                    result = Circle(float.Parse(pathParts[0]));
                    break;
                case 'E':
                    result = Ellipse(float.Parse(pathParts[0]), float.Parse(pathParts[1]));
                    break;
                case 'N':
                    result = Ngon(int.Parse(pathParts[0]), float.Parse(pathParts[1]));
                    break;
                case 'T':
                    result = Star(int.Parse(pathParts[0]), float.Parse(pathParts[1]), float.Parse(pathParts[2]));
                    break;
                default:
                    result = Ngon(3, 1f);
                    break;
            }
            return result;
        }

        public override string ToString()
        {
            return pathString;
        }

        /*
         * X: Path
         * Y: Polygon
         * S: Square
         * R: Rectangle
         * C: Circle
         * E: Ellipse
         * N: Ngon
         * T: Star
         */
    }
}
