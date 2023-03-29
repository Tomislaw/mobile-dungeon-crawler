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

            var floor = astar.GetTileData(parent.Id + new Vector2Int(0, -1));

            if(parent.Tile.Water && data.canSwim)
                AddInWaterNodes(astar, nodes, data, parent);
            else if (parent.Tile.Ladder && data.canUseLadder)
                AddOnLadderNodes(astar, nodes, data, parent);
            else if (parent.jumpHeightLeft == 1)
                AddJumpTopNodes(astar, nodes, data, parent);
            else if (parent.jumpHeightLeft > 1)
                AddJumpRisingNodes(astar, nodes, data, parent);
            else if (floor.Platform && data.canUsePlatform)
                AddOnPlatformNodes(astar, nodes, data, parent);
            else if (floor.Block)
                AddOnBlockNodes(astar, nodes, data, parent);
            else if (parent.jumpHeightLeft <= 0)
                AddFallingNodes(astar, nodes, data, parent);

            return nodes;
        }

        private static void AddFallingNodes(AStar astar, in List<AStarSharp.Node> nodes, WalkData data, AStarSharp.Node parent)
        {
            AddDownNode(astar, nodes, data, parent.Id + new Vector2Int(0, -1), parent);
            if (parent.jumpDistanceLeft > 0)
            {
                AddDownNode(astar, nodes, data, parent.Id + new Vector2Int(-1, -1), parent);
                AddDownNode(astar, nodes, data, parent.Id + new Vector2Int(1, -1), parent);
                AddWalkableNode(astar, nodes, data, parent.Id + new Vector2Int(1, 0), parent);
                AddWalkableNode(astar, nodes, data, parent.Id + new Vector2Int(-1, 0), parent);
            }
        }

        private static void AddJumpTopNodes(AStar astar, in List<AStarSharp.Node> nodes, WalkData data, AStarSharp.Node parent)
        {
            AddDownNode(astar, nodes, data, parent.Id + new Vector2Int(0, -1), parent);

            if (parent.jumpDistanceLeft > 0)
            {
                AddDownNode(astar, nodes, data, parent.Id + new Vector2Int(1, -1), parent);
                AddDownNode(astar, nodes, data, parent.Id + new Vector2Int(-1, -1), parent);
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
                AddDownNode(astar, nodes, data, parent.Id + new Vector2Int(0, -1), parent);
            if (!AddSwimNode(astar, nodes, data, parent.Id + new Vector2Int(1, -1), parent))
                AddDownNode(astar, nodes, data, parent.Id + new Vector2Int(1, -1), parent);
            if (!AddSwimNode(astar, nodes, data, parent.Id + new Vector2Int(-1, -1), parent))
                AddDownNode(astar, nodes, data, parent.Id + new Vector2Int(-1, -1), parent);

            if (!AddSwimNode(astar, nodes, data, parent.Id + new Vector2Int(0, 1), parent))
                AddJumpNode(astar, nodes, data, parent.Id + new Vector2Int(0, 1), parent);
            AddSwimNode(astar, nodes, data, parent.Id + new Vector2Int(1, 1), parent);
            AddSwimNode(astar, nodes, data, parent.Id + new Vector2Int(-1, 1), parent);

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
            AddWalkableNode(astar, nodes, data, parent.Id + new Vector2Int(0, 1), parent);
            AddDownNode(astar, nodes, data, parent.Id + new Vector2Int(0, -1), parent);
            AddDownNode(astar, nodes, data, parent.Id + new Vector2Int(1, -1), parent);
            AddDownNode(astar, nodes, data, parent.Id + new Vector2Int(-1, -1), parent);
        }

        private static void AddOnPlatformNodes(AStar astar, in List<AStarSharp.Node> nodes, WalkData data, AStarSharp.Node parent)
        {
            AddWalkableNode(astar, nodes, data, parent.Id + new Vector2Int(1, 0), parent);
            AddWalkableNode(astar, nodes, data, parent.Id + new Vector2Int(-1, 0), parent);
            AddJumpNode(astar, nodes, data, parent.Id + new Vector2Int(1, 1), parent);
            AddJumpNode(astar, nodes, data, parent.Id + new Vector2Int(0, 1), parent);
            AddJumpNode(astar, nodes, data, parent.Id + new Vector2Int(-1, 1), parent);
            AddDownNode(astar, nodes, data, parent.Id + new Vector2Int(0, -1), parent);
            AddDownNode(astar, nodes, data, parent.Id + new Vector2Int(1, -1), parent);
            AddDownNode(astar, nodes, data, parent.Id + new Vector2Int(-1, -1), parent);
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
            var node = new AStarSharp.Node(astar.GetTileData(Id));
            if (!CanFitInNode(astar, node, data))
                return true;

            if(!node.Tile.Water)
                return false;

            node.Weight = 3;

            node.action = AStarSharp.Node.Type.Swim;

            node.Parent = parent;
            nodes.Add(node);

            return true;
        }

        private static void AddDownNode(AStar astar, in List<AStarSharp.Node> nodes, WalkData data, Vector2Int Id, AStarSharp.Node parent)
        {
            var node = new AStarSharp.Node(astar.GetTileData(Id));
            if (!CanFitInNode(astar, node, data))
                return;

            var floor = astar.GetTileData(new Vector2Int(Id.x, Id.y - 1));

            if (floor.Ladder && data.canUseLadder)
            {
                node.Weight = 1;
                node.action = AStarSharp.Node.Type.Climb;
            }
            else if(node.Tile.Platform && data.canUsePlatform)
            {
                node.Weight = 2;
                node.jumpHeightLeft = 0;
                if (parent.jumpDistanceLeft == -1)
                    node.jumpDistanceLeft = 1;
                else
                    node.jumpDistanceLeft = Math.Max(0, parent.jumpDistanceLeft - 1);
                node.action = AStarSharp.Node.Type.Drop;
            }
            else if (floor.Block || floor.Platform)
            {
                node.Weight = 2;
                node.action = AStarSharp.Node.Type.Walk;
            }
            else
            {
                node.Weight = 2;
                node.jumpHeightLeft = 0;
                if (parent.jumpDistanceLeft == -1)
                    node.jumpDistanceLeft = 1;
                else
                    node.jumpDistanceLeft = Math.Max(0, parent.jumpDistanceLeft - 1);
                node.action = AStarSharp.Node.Type.Fall;
            }

            node.Parent = parent;
            nodes.Add(node);
        }

        private static void AddWalkableNode(AStar astar, in List<AStarSharp.Node> nodes, WalkData data, Vector2Int Id, AStarSharp.Node parent)
        {
            var node = new AStarSharp.Node(astar.GetTileData(Id));

            if (!CanFitInNode(astar, node, data))
                return;

            var floor = astar.GetTileData(new Vector2Int(Id.x, Id.y - 1));

            if (!floor.Block && !floor.Platform)
                return;

            node.action = AStarSharp.Node.Type.Walk;
            node.Parent = parent;
            nodes.Add(node);
        }

        private static void AddJumpNode(AStar astar, in List<AStarSharp.Node> nodes, WalkData data, Vector2Int Id, AStarSharp.Node parent)
        {
            var node = new AStarSharp.Node(astar.GetTileData(Id));
            if (!CanFitInNode(astar, node, data))
                return;

            node.Weight = 3;
            node.jumpHeightLeft = data.jumpHeight;
            node.jumpDistanceLeft = data.jumpDistance;
            node.action = AStarSharp.Node.Type.Jump;
            node.Parent = parent;
            nodes.Add(node);
        }

        private static void AddUpNode(AStar astar, in List<AStarSharp.Node> nodes, WalkData data, Vector2Int Id, AStarSharp.Node parent)
        {
            var node = new AStarSharp.Node(astar.GetTileData(Id));
            if (!CanFitInNode(astar, node, data))
                return;

            if (node.Tile.Ladder && data.canUseLadder)
            {
                node.Weight = 1;
                node.action = AStarSharp.Node.Type.Climb;
            }
            else if (parent.jumpHeightLeft > 0)
            {
                node.Weight = 3;

                node.jumpHeightLeft = Mathf.Max(parent.jumpHeightLeft - 1, 0);
                node.jumpDistanceLeft = Mathf.Max(parent.jumpDistanceLeft - 1, 0);
                node.action = AStarSharp.Node.Type.Jump;
            }

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
        private static bool IsNodeBlocking(AStarSharp.Tile tile, WalkData data)
        {
            return tile.Block || tile.Water && !data.canSwim;
        }
    }
}
