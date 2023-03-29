using System.Collections.Generic;
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

            AddNode(astar, nodes, data, parent.Id + new Vector2Int(1, -1), parent);
            AddNode(astar, nodes, data, parent.Id + new Vector2Int(1, 1), parent);
            AddNode(astar, nodes, data, parent.Id + new Vector2Int(-1, 1), parent);
            AddNode(astar, nodes, data, parent.Id + new Vector2Int(-1, -1), parent);
            return nodes;
        }

        private static void AddNode(AStar astar, in List<AStarSharp.Node> nodes, WalkData data, Vector2Int Id, AStarSharp.Node parent)
        {
            var node = new AStarSharp.Node(astar.GetTileData(Id));
            if (!CanFitInNode(astar, node, data))
                return;

            node.Weight = 1;

            // if blocks are nearby, then dont like coming near them
            for (int y = -2; y <= 1; y++)
            {
                if (y == 0)
                    continue;

                var near = astar.GetTileData(new Vector2Int(Id.x, Id.y + y));
                if (node == null || near.Block)
                    node.Weight += 4;
            }

            node.action = node.Tile.Water && data.canSwim ? AStarSharp.Node.Type.Swim : AStarSharp.Node.Type.Fly;
            node.Parent = parent;
            nodes.Add(node);
        }

        private static bool CanFitInNode(AStar astar, AStarSharp.Node node, WalkData data)
        {
            if (IsNodeBlocking(node.Tile, data))
                return false;

            for (int i = node.Id.y + 1; i < node.Id.y + data.height; i++)
                if (IsNodeBlocking(astar.GetTileData(new Vector2Int(node.Id.x, i)), data))
                    return false;

            return true;
        }
        private static bool IsNodeBlocking(AStarSharp.Tile node, WalkData data)
        {
            return node.Block || node.Water && !data.canSwim;
        }
    }
}
