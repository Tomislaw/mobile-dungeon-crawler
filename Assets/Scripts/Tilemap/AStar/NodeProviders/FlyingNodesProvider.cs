using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace RuinsRaiders
{
    public static class FlyingNodesProvider
    {
        public static List<AStarSharp.Node> GetNodes(AStar astar, WalkData data, AStarSharp.Node parent)
        {
            List<AStarSharp.Node> nodes = new();
            AddNode(astar, nodes, data, parent.Id + new Vector2Int(0, -1), parent);
            AddNode(astar, nodes, data, parent.Id + new Vector2Int(0, 1), parent);
            AddNode(astar, nodes, data, parent.Id + new Vector2Int(-1, 0), parent);
            AddNode(astar, nodes, data, parent.Id + new Vector2Int(1, 0), parent);
            return nodes;
        }

        private static void AddNode(AStar astar, in List<AStarSharp.Node> nodes, WalkData data, Vector2Int Id, AStarSharp.Node parent)
        {
            var node = astar.GetNode(Id);
            if (!CanFitInNode(astar, node, data))
                return;

            node.Weight = 1;

            // if blocks are nearby, then dont like coming near them
            for (int x = -1; x <= 1; x++)
                for (int y = -1; y <= 1; y++)
                {
                    if (x == 0 || y == 0)
                        continue;

                    var above = astar.GetNode(new Vector2Int(Id.x + x, Id.y + y));
                    if (node == null || above.Block)
                        node.Weight += 4;
                }


            node.Parent = parent;
            nodes.Add(node);
        }

        private static bool CanFitInNode(AStar astar, AStarSharp.Node node, WalkData data)
        {
            if (IsNodeBlocking(node, data))
                return false;

            for (int i = node.Id.y + 1; i < node.Id.y + data.height; i++)
                if (IsNodeBlocking(astar.GetNode(new Vector2Int(node.Id.x, i)), data))
                    return false;

            return true;
        }
        private static bool IsNodeBlocking(AStarSharp.Node node, WalkData data)
        {
            return node == null || node.Block || node.Water && !data.canSwim;
        }
    }
}
