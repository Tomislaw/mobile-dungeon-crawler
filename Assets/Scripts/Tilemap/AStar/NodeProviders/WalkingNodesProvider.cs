using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace RuinsRaiders
{
    public static class WalkingNodesProvider
    {
        public static List<AStarSharp.Node> GetNodes(AStar astar, WalkData data, AStarSharp.Node parent)
        {
            List<AStarSharp.Node> nodes = new();

            var floor = astar.GetNode(parent.Id + new Vector2Int(0, -1));
            if (floor == null)
                return nodes;

            if(parent.Water && data.canSwim)
                AddInWaterNodes(astar, nodes, data, parent);
            else if (parent.Ladder && data.canUseLadder)
                AddOnLadderNodes(astar, nodes, data, parent);
            else if (floor.Platform)
                AddOnPlatformNodes(astar, nodes, data, parent);
            else if (floor.Block)
                AddOnBlockNodes(astar, nodes, data, parent);
            else if (parent.jumpHeightLeft <= 0)
                AddFallingNodes(astar, nodes, data, parent);
            else if (parent.jumpHeightLeft == 1)
                AddJumpTopNodes(astar, nodes, data, parent);
            else if (parent.jumpHeightLeft > 1)
                AddJumpRisingNodes(astar, nodes, data, parent);

            return nodes;
        }

        private static void AddFallingNodes(AStar astar, in List<AStarSharp.Node> nodes, WalkData data, AStarSharp.Node parent)
        {
            AddFallNode(astar, nodes, data, parent.Id + new Vector2Int(0, -1), parent);
            if (parent.jumpDistanceLeft > 0)
            {
                AddFallNode(astar, nodes, data, parent.Id + new Vector2Int(-1, -1), parent);
                AddFallNode(astar, nodes, data, parent.Id + new Vector2Int(1, -1), parent);
            }
        }

        private static void AddJumpTopNodes(AStar astar, in List<AStarSharp.Node> nodes, WalkData data, AStarSharp.Node parent)
        {
            AddFallNode(astar, nodes, data, parent.Id + new Vector2Int(0, -1), parent);

            if (parent.jumpDistanceLeft > 0)
            {
                AddFallNode(astar, nodes, data, parent.Id + new Vector2Int(1, -1), parent);
                AddFallNode(astar, nodes, data, parent.Id + new Vector2Int(-1, -1), parent);
                AddWalkableNode(astar, nodes, data, parent.Id + new Vector2Int(1, 0), parent);
                AddWalkableNode(astar, nodes, data, parent.Id + new Vector2Int(-1, 0), parent);
            }
        }

        private static void AddJumpRisingNodes(AStar astar, in List<AStarSharp.Node> nodes, WalkData data, AStarSharp.Node parent)
        {
            AddUpNode(astar, nodes, data, parent.Id + new Vector2Int(0, 1), parent);
            if (parent.jumpDistanceLeft > 0)
            {
                AddUpNode(astar, nodes, data, parent.Id + new Vector2Int(1, 1), parent);
                AddUpNode(astar, nodes, data, parent.Id + new Vector2Int(-1, 1), parent);
            }
        }

        private static void AddInWaterNodes(AStar astar, in List<AStarSharp.Node> nodes, WalkData data, AStarSharp.Node parent)
        {
            if (!AddSwimNode(astar, nodes, data, parent.Id + new Vector2Int(0, -1), parent))
                AddFallNode(astar, nodes, data, parent.Id + new Vector2Int(0, -1), parent);

            if (!AddSwimNode(astar, nodes, data, parent.Id + new Vector2Int(0, 1), parent))
                AddJumpNode(astar, nodes, data, parent.Id + new Vector2Int(0, 1), parent);

            if (!AddSwimNode(astar, nodes, data, parent.Id + new Vector2Int(-1, 0), parent))
                AddWalkableNode(astar, nodes, data, parent.Id + new Vector2Int(-1, 0), parent);

            if (!AddSwimNode(astar, nodes, data, parent.Id + new Vector2Int(1, 0), parent))
                AddWalkableNode(astar, nodes, data, parent.Id + new Vector2Int(1, 0), parent);
        }

        private static void AddOnLadderNodes(AStar astar, in List<AStarSharp.Node> nodes, WalkData data, AStarSharp.Node parent)
        {
            AddJumpNode(astar, nodes, data, parent.Id + new Vector2Int(0, 1), parent);
            AddWalkableNode(astar, nodes, data, parent.Id + new Vector2Int(1, 0), parent);
            AddWalkableNode(astar, nodes, data, parent.Id + new Vector2Int(-1, 0), parent);
            AddWalkableNode(astar, nodes, data, parent.Id + new Vector2Int(1, 1), parent);
            AddWalkableNode(astar, nodes, data, parent.Id + new Vector2Int(-1, 1), parent);
            AddWalkableNode(astar, nodes, data, parent.Id + new Vector2Int(0, 1), parent);
            AddFallNode(astar, nodes, data, parent.Id + new Vector2Int(0, -1), parent);
            AddFallNode(astar, nodes, data, parent.Id + new Vector2Int(1, -1), parent);
            AddFallNode(astar, nodes, data, parent.Id + new Vector2Int(-1, -1), parent);
        }

        private static void AddOnPlatformNodes(AStar astar, in List<AStarSharp.Node> nodes, WalkData data, AStarSharp.Node parent)
        {
            AddWalkableNode(astar, nodes, data, parent.Id + new Vector2Int(1, 0), parent);
            AddWalkableNode(astar, nodes, data, parent.Id + new Vector2Int(-1, 0), parent);
            AddJumpNode(astar, nodes, data, parent.Id + new Vector2Int(1, 1), parent);
            AddJumpNode(astar, nodes, data, parent.Id + new Vector2Int(0, 1), parent);
            AddJumpNode(astar, nodes, data, parent.Id + new Vector2Int(-1, 1), parent);
            AddFallNode(astar, nodes, data, parent.Id + new Vector2Int(0, -1), parent);
            AddFallNode(astar, nodes, data, parent.Id + new Vector2Int(1, -1), parent);
            AddFallNode(astar, nodes, data, parent.Id + new Vector2Int(-1, -1), parent);
        }

        private static void AddOnBlockNodes(AStar astar, in List<AStarSharp.Node> nodes, WalkData data, AStarSharp.Node parent)
        {
            AddWalkableNode(astar, nodes, data, parent.Id + new Vector2Int(1, 0), parent);
            AddWalkableNode(astar, nodes, data, parent.Id + new Vector2Int(-1, 0), parent);
            AddJumpNode(astar, nodes, data, parent.Id + new Vector2Int(1, 1), parent);
            AddJumpNode(astar, nodes, data, parent.Id + new Vector2Int(0, 1), parent);
            AddJumpNode(astar, nodes, data, parent.Id + new Vector2Int(-1, 1), parent);
        }

        private static bool AddSwimNode(AStar astar, in List<AStarSharp.Node> nodes, WalkData data, Vector2Int Id, AStarSharp.Node parent)
        {
            var node = astar.GetNode(Id);
            if (!CanFitInNode(astar, node, data))
                return true;

            if(!node.Water)
                return false;

            node.Weight = 3;

            node.Parent = parent;
            nodes.Add(node);

            return true;
        }

        private static void AddFallNode(AStar astar, in List<AStarSharp.Node> nodes, WalkData data, Vector2Int Id, AStarSharp.Node parent)
        {
            var node = astar.GetNode(Id);
            if (!CanFitInNode(astar, node, data))
                return;

            var floor = astar.GetNode(new Vector2Int(Id.x, Id.y - 1));
            // stop if node is out of bounds
            if (floor == null)
                return;

            // if can stand on tile  
            if (floor.Block || floor.Platform)
            {
                node.Weight = 2;
                node.Parent = parent;
                nodes.Add(node);
            } // if ladder
            else if (floor.Ladder && data.canUseLadder)
            {
                node.Weight = 1;
                node.Parent = parent;
                nodes.Add(node);
            }
            else
            {
                node.Weight = 2;
                node.Parent = parent;
                node.jumpHeightLeft = 0;
                node.jumpDistanceLeft = Math.Max(0, parent.jumpDistanceLeft - 1);
                nodes.Add(node);
            }
        }

        private static void AddWalkableNode(AStar astar, in List<AStarSharp.Node> nodes, WalkData data, Vector2Int Id, AStarSharp.Node parent)
        {
            var node = astar.GetNode(Id);
            if (!CanFitInNode(astar, node, data))
                return;

            var floor = astar.GetNode(new Vector2Int(Id.x, Id.y - 1));
            if (floor == null)
                return;

            if (floor.Block)
            {
                node.Parent = parent;
                nodes.Add(node);
            }
            else
            {
                node.Parent = parent;
                node.jumpHeightLeft = 0;
                node.jumpDistanceLeft = 1;
                nodes.Add(node);
            }
        }

        private static void AddJumpNode(AStar astar, in List<AStarSharp.Node> nodes, WalkData data, Vector2Int Id, AStarSharp.Node parent)
        {
            var node = astar.GetNode(Id);
            if (!CanFitInNode(astar, node, data))
                return;

            if (node.Ladder && data.canUseLadder)
            {
                node.Weight = 1;
                node.Parent = parent;
                nodes.Add(node);
            }
            else
            {
                node.Weight = 3;
                node.Parent = parent;
                node.jumpHeightLeft = data.jumpHeight;
                node.jumpDistanceLeft = data.jumpDistance;
                nodes.Add(node);
            }
        }

        private static void AddUpNode(AStar astar, in List<AStarSharp.Node> nodes, WalkData data, Vector2Int Id, AStarSharp.Node parent)
        {
            var node = astar.GetNode(Id);
            if (!CanFitInNode(astar, node, data))
                return;

            if (node.Ladder && data.canUseLadder)
            {
                node.Weight = 1;
                node.Parent = parent;
                nodes.Add(node);
            }
            else if (parent.jumpHeightLeft > 0)
            {
                node.Weight = 3;
                node.Parent = parent;
                node.jumpHeightLeft = Mathf.Max(parent.jumpHeightLeft - 1, 0);
                node.jumpDistanceLeft = Mathf.Max(parent.jumpDistanceLeft - 1, 0);
                nodes.Add(node);
            }
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
