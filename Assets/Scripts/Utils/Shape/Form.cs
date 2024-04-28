using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utils.Constants;
using Utils.Ext;

namespace Utils.Shape
{
    public class Form
    {
        public FormType FormType { get; private set; }
        public string FormString { get; private set; } = "";

        public FormNode StartNode { get; set; }

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

        public static Form Vector(float[] points)
        {
            var startPoint = new Vector2(points[0], points[1]);
            var nodes = new List<FormNode>();
            for (var i = 2; i < points.Length; i++)
            {
                switch (points[i])
                {
                    case 0f:
                        nodes.Add(new FormLine(points[i + 1], points[i + 2]));
                        i += 2;
                        break;
                    case 1f:
                        nodes.Add(new FormArc(points[i + 1], points[i + 2], points[i + 3], points[i + 4]));
                        i += 4;
                        break;
                    case 2f:
                        nodes.Add(new FormBezier(points[i + 1], points[i + 2], points[i + 3], points[i + 4],
                            points[i + 5], points[i + 6]));
                        i += 6;
                        break;
                }
            }
            var result = new Form(startPoint, nodes)
            {
                FormType = FormType.Vector,
                FormString = points.Aggregate("", (current, point) => current + $" {point}")[1..]
            };
            return result;
        }

        public static Form Polygon(Vector2[] points)
        {
            if (points.Length < 3)
            {
                throw Errors.MissingFormParams("polygon", 3);
            }
            var startPoint = points[0];
            var nodes = new FormLine[points.Length - 1];
            for (var i = 1; i < points.Length; i++)
            {
                nodes[i - 1] = new FormLine(points[i]);
            }
            var result = new Form(startPoint, nodes)
            {
                FormType = FormType.Polygon,
                FormString = points.Aggregate("", (current, point) => current + $" {point.x} {point.y}")[1..]
            };
            return result;
        }

        public static Form Square(float diameter)
        {
            var result = Rectangle(diameter, diameter);
            result.FormType = FormType.Square;
            result.FormString = $"{diameter}";
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
            result.FormType = FormType.Rectangle;
            result.FormString = $"{width} {height}";
            return result;
        }

        public static Form Circle(float diameter)
        {
            var result = Ellipse(diameter, diameter);
            result.FormType = FormType.Circle;
            result.FormString = $"{diameter}";
            return result;
        }

        public static Form Ellipse(float width, float height)
        {
            var result = new Form(new Vector2(0f, height / 2f), new FormNode[]
            {
                new FormArc(width / 2f, height / 2f, width / 2f, 0f),
                new FormArc(width / 2f, -height / 2f, 0f, -height / 2f),
                new FormArc(-width / 2f, -height / 2f, -width / 2f, 0f),
                new FormArc(-width / 2f, height / 2f, 0f, height / 2f)
            })
            {
                FormType = FormType.Ellipse,
                FormString = $"{width} {height}"
            };
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
            result.FormType = FormType.Ngon;
            result.FormString = $"{sides} {diameter}";
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
            result.FormType = FormType.Star;
            result.FormString = $"{points} {outerDiameter} {innerDiameter}";
            return result;
        }

        public static Form Invalid()
        {
            return Star(24, 24f, .24f);
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
