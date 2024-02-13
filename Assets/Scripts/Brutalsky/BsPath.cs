using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Brutalsky
{
    public class BsPath
    {
        public BsPathNode startNode { get; set; }

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

        public static BsPath FromString(string pathString)
        {
            var parts = new Regex(@" (?=[LC])").Split(pathString.ToUpper());
            if (parts.Length < 3) throw new ArgumentException("Cannot make a path with less than 3 points");
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
                        if (values.Length < 3) throw new ArgumentException("Cannot make a line node with less than 2 parameters");
                        nodes.Add(new BsPathLine(float.Parse(values[1]), float.Parse(values[2])));
                        break;
                    case "C":
                        if (values.Length < 3) throw new ArgumentException("Cannot make a curve node with less than 4 parameters");
                        nodes.Add(new BsPathCurve(float.Parse(values[1]), float.Parse(values[2]),
                            float.Parse(values[3]), float.Parse(values[4])));
                        break;
                }
            }
            return new BsPath(startpoint, nodes);
        }

        public static BsPath Triangle(Vector2 point1, Vector2 point2, Vector2 point3)
        {
            return Polygon(new[] { point1, point2, point3 });
        }

        public static BsPath Square(float diameter)
        {
            return Rectangle(diameter, diameter);
        }

        public static BsPath Rectangle(float width, float height)
        {
            return Polygon(new Vector2[]
            {
                new(-width / 2f, height / 2f),
                new(width / 2f, height / 2f),
                new(width / 2f, -height / 2f),
                new(-width / 2f, -height / 2f)
            });
        }

        public static BsPath Circle(float diameter)
        {
            return Ellipse(diameter, diameter);
        }

        public static BsPath Ellipse(float width, float height)
        {
            return new BsPath(new Vector2(0f, height / 2f), new BsPathNode[]
            {
                new BsPathCurve(width / 2f, height / 2f, width / 2f, 0f),
                new BsPathCurve(width / 2f, -height / 2f, 0f, -height / 2f),
                new BsPathCurve(-width / 2f, -height / 2f, -width / 2f, 0f),
                new BsPathCurve(-width / 2f, height / 2f, 0f, height / 2f)
            });
        }

        public static BsPath Ngon(int sides, float diameter)
        {
            var vertices = new Vector2[sides];
            for (var i = 0; i < sides; i++)
            {
                var vertexAngle = i / (float)sides * 2f * Mathf.PI;
                vertices[i] = new Vector2(Mathf.Cos(vertexAngle), Mathf.Sin(vertexAngle)) * (diameter / 2f);
            }
            return Polygon(vertices);
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
            return Polygon(vertices);
        }

        public static BsPath Polygon(Vector2[] points)
        {
            if (points.Length < 3) throw new ArgumentException("Cannot make a polygon with less than 3 points");
            var startpoint = points[0];
            var nodes = new BsPathLine[points.Length - 1];
            for (var i = 1; i < points.Length; i++)
            {
                nodes[i - 1] = new BsPathLine(points[i]);
            }
            return new BsPath(startpoint, nodes);
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
    }
}
