using System.Collections.Generic;
using System.Linq;
using UnityEngine;
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

        public static Form Vector(float[] args)
        {
            var startPoint = new Vector2(args[0], args[1]);
            var nodes = new List<FormNode>();
            for (var i = 2; i < args.Length; i++)
            {
                switch (args[i])
                {
                    case 0f:
                        nodes.Add(new FormLine(args[i + 1], args[i + 2]));
                        i += 2;
                        break;
                    case 1f:
                        nodes.Add(new FormArc(args[i + 1], args[i + 2], args[i + 3], args[i + 4]));
                        i += 4;
                        break;
                    case 2f:
                        nodes.Add(new FormBezier(args[i + 1], args[i + 2], args[i + 3], args[i + 4],
                            args[i + 5], args[i + 6]));
                        i += 6;
                        break;
                }
            }
            var result = new Form(startPoint, nodes)
            {
                FormType = FormType.Vector,
                FormString = args.Aggregate("", (current, arg) => current + $" {arg}")[1..]
            };
            return result;
        }

        public static Form Polygon(float[] args)
        {
            var startPoint = new Vector2(args[0], args[1]);
            var nodes = new FormLine[args.Length / 2 - 1];
            for (var i = 2; i < args.Length; i += 2)
            {
                nodes[i / 2 - 1] = new FormLine(new Vector2(args[i], args[i + 1]));
            }
            var result = new Form(startPoint, nodes)
            {
                FormType = FormType.Polygon,
                FormString = args.Aggregate("", (current, arg) => current + $" {arg}")[1..]
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
            var result = Polygon(new[]
            {
                -width * .5f, height * .5f,
                width * .5f, height * .5f,
                width * .5f, -height * .5f,
                -width * .5f, -height * .5f
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
            var result = new Form(new Vector2(0f, height * .5f), new FormNode[]
            {
                new FormArc(width * .5f, height * .5f, width * .5f, 0f),
                new FormArc(width * .5f, -height * .5f, 0f, -height * .5f),
                new FormArc(-width * .5f, -height * .5f, -width * .5f, 0f),
                new FormArc(-width * .5f, height * .5f, 0f, height * .5f)
            })
            {
                FormType = FormType.Ellipse,
                FormString = $"{width} {height}"
            };
            return result;
        }

        public static Form Ngon(int sides, float diameter)
        {
            var args = new float[sides * 2];
            var scale = diameter * .5f;
            for (var i = 0; i < sides; i++)
            {
                var vertexAngle = i / (float)sides * 2f * Mathf.PI;
                args[i * 2] = Mathf.Cos(vertexAngle) * scale;
                args[i * 2 + 1] = Mathf.Sin(vertexAngle) * scale;
            }
            var result = Polygon(args);
            result.FormType = FormType.Ngon;
            result.FormString = $"{sides} {diameter}";
            return result;
        }

        public static Form Star(int points, float outerDiameter, float innerDiameter)
        {
            var args = new float[points * 4];
            var scales = new[] { outerDiameter * .5f, innerDiameter * .5f };
            for (var i = 0; i < points * 2; i++)
            {
                var vertexAngle = i / (float)(points * 2) * 2f * Mathf.PI;
                var scale = scales[i % 2];
                args[i * 2] = Mathf.Cos(vertexAngle) * scale;
                args[i * 2 + 1] = Mathf.Sin(vertexAngle) * scale;
            }
            var result = Polygon(args);
            result.FormType = FormType.Star;
            result.FormString = $"{points} {outerDiameter} {innerDiameter}";
            return result;
        }

        public static Form Invalid()
        {
            return Star(10, 100f, 1f);
        }

        public Vector2[] ToFillPoints(float rotation)
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

        public Vector2[] ToStrokePoints(float rotation, float width)
        {
            var result = new List<Vector2>();
            var fillPoints = ToFillPoints(rotation);
            foreach (var point in fillPoints)
            {
                // TODO: CONNECT THE DOTS
            }
            return result.ToArray();
        }
    }
}
