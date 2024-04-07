using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using Utils.Constants;

namespace Utils.Shape
{
    public class Form
    {
        public FormNode StartNode { get; set; }
        public string VectorString { get; private set; } = "";

        public Form(Vector2 startPoint, IEnumerable<FormNode> nodes)
        {
            StartNode = new FormStart(startPoint);
            var currentNode = StartNode;
            foreach (var node in nodes)
            {
                node.Previous = currentNode;
                currentNode = node;
            }
        }

        public static Form Vector(string path)
        {
            var parts = Regex.Split(path.ToUpper(), " (?=[LC])");
            if (parts.Length < 3)
            {
                throw Errors.MissingPathParams("path", 3);
            }
            var startValues = parts[0].Split(' ');
            var startPoint = new Vector2(float.Parse(startValues[0]), float.Parse(startValues[1]));
            var nodes = new List<FormNode>();
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
                        nodes.Add(new FormLine(float.Parse(values[1]), float.Parse(values[2])));
                        break;
                    case "C":
                        if (values.Length < 5)
                        {
                            throw Errors.MissingPathParams("curve node", 4);
                        }
                        nodes.Add(new FormCurve(float.Parse(values[1]), float.Parse(values[2]),
                            float.Parse(values[3]), float.Parse(values[4])));
                        break;
                }
            }

            var result = new Form(startPoint, nodes);
            result.VectorString = $"X {path.ToUpper()}";
            return result;
        }

        public static Form Polygon(Vector2[] points)
        {
            if (points.Length < 3)
            {
                throw Errors.MissingPathParams("polygon", 3);
            }
            var startPoint = points[0];
            var nodes = new FormLine[points.Length - 1];
            for (var i = 1; i < points.Length; i++)
            {
                nodes[i - 1] = new FormLine(points[i]);
            }

            var result = new Form(startPoint, nodes);
            result.VectorString = points.Aggregate("Y", (current, point) => current + $" {point.x} {point.y}");
            return result;
        }

        public static Form Square(float diameter)
        {
            var result = Rectangle(diameter, diameter);
            result.VectorString = $"S {diameter}";
            return result;
        }

        public static Form Rectangle(float width, float height)
        {
            var result = Polygon(new Vector2[]
            {
                new(-width / 2f, height / 2f),
                new(width / 2f, height / 2f),
                new(width / 2f, -height / 2f),
                new(-width / 2f, -height / 2f)
            });
            result.VectorString = $"R {width} {height}";
            return result;
        }

        public static Form Circle(float diameter)
        {
            var result = Ellipse(diameter, diameter);
            result.VectorString = $"C {diameter}";
            return result;
        }

        public static Form Ellipse(float width, float height)
        {
            var result = new Form(new Vector2(0f, height / 2f), new FormNode[]
            {
                new FormCurve(width / 2f, height / 2f, width / 2f, 0f),
                new FormCurve(width / 2f, -height / 2f, 0f, -height / 2f),
                new FormCurve(-width / 2f, -height / 2f, -width / 2f, 0f),
                new FormCurve(-width / 2f, height / 2f, 0f, height / 2f)
            });
            result.VectorString = $"E {width} {height}";
            return result;
        }

        public static Form Ngon(int sides, float diameter)
        {
            var vertices = new Vector2[sides];
            for (var i = 0; i < sides; i++)
            {
                var vertexAngle = i / (float)sides * 2f * Mathf.PI;
                vertices[i] = new Vector2(Mathf.Cos(vertexAngle), Mathf.Sin(vertexAngle)) * (diameter / 2f);
            }

            var result = Polygon(vertices);
            result.VectorString = $"N {sides} {diameter}";
            return result;
        }

        public static Form Star(int points, float outerDiameter, float innerDiameter)
        {
            var vertices = new Vector2[points * 2];
            for (var i = 0; i < points * 2; i++)
            {
                var vertexAngle = i / (float)(points * 2) * 2f * Mathf.PI;
                var diameter = i % 2 == 0 ? outerDiameter : innerDiameter;
                vertices[i] = new Vector2(Mathf.Cos(vertexAngle), Mathf.Sin(vertexAngle)) * (diameter / 2f);
            }

            var result = Polygon(vertices);
            result.VectorString = $"T {points} {outerDiameter} {innerDiameter}";
            return result;
        }

        public Vector2[] ToPoints()
        {
            var result = new List<Vector2>{StartNode.EndPoint};
            var currentNode = StartNode.Next;
            while (currentNode != null)
            {
                for (var i = 1; i <= currentNode.DetailLevel; i++)
                {
                    result.Add(currentNode.SamplePoint(i / (float)currentNode.DetailLevel));
                }
                currentNode = currentNode.Next;
            }
            if (result[^1] == result[0]) result.RemoveAt(result.Count - 1);

            return result.ToArray();
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
