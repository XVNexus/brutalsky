using System.Collections.Generic;
using UnityEngine;

namespace Brutalsky
{
    public class BsPath
    {
        public BsPathNode startNode { get; set; }

        public BsPath(Vector2 startpoint, List<BsPathNode> nodes)
        {
            startNode = new BsPathStart(startpoint);
            var currentNode = startNode;
            foreach (var node in nodes)
            {
                node.previous = currentNode;
                currentNode = node;
            }
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
            return result.ToArray();
        }
    }
}
