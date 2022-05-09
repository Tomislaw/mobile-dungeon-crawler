using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using System.Text;
using UnityEngine.Tilemaps;
using System.Threading;
using UnityEngine.Events;


[RequireComponent(typeof(Tilemap))]
public class AStar : MonoBehaviour
{
    public UnityEvent OnMapUpdated = new UnityEvent();
    // Start is called before the first frame update
    private Tilemap tilemap;
    private HashSet<Vector2Int> DynamicTiles = new HashSet<Vector2Int>();

    private bool mapUpdated = false;

    public void AddDynamicBlockTile(Vector2Int tileId)
    {
        DynamicTiles.Add(tileId);
        mapUpdated = true;
    }

    public void RemoveDynamicBlockTile(Vector2Int tileId)
    {
        DynamicTiles.Remove(tileId);
        mapUpdated = true;

    }

    private void Awake()
    {
        tilemap = GetComponent<Tilemap>();
    }

    private void FixedUpdate()
    {
        if (mapUpdated)
        {
            OnMapUpdated.Invoke();
            mapUpdated = false;
        }
    }

    public AStarSharp.Node GetNode(Vector2Int Id)
    {
        var collider = tilemap.GetColliderType(new Vector3Int(Id.x, Id.y, 0));
        var gameobject = tilemap.GetInstantiatedObject(new Vector3Int(Id.x, Id.y, 0));

        var node = new AStarSharp.Node();
        node.Id = Id;
        node.Destroyable = DynamicTiles.Contains(Id);
        node.Block = collider != Tile.ColliderType.None || node.Destroyable;
        if (gameobject != null)
        {
            node.Ladder = gameobject.GetComponentInChildren<LadderTile>() != null;
            node.Platform = gameobject.GetComponentInChildren<PlatformTile>() != null;
        }
        node.Weight = 1;
        node.Cost = 1;
        return node;
    }

    public Vector2 GetPositionFromId(Vector2Int id)
    {
        var pos = tilemap.CellToWorld(new Vector3Int(id.x, id.y, 0));
        return new Vector2(pos.x + 0.5f, pos.y + 0.5f);
    }

    public Vector2Int GetTileId(Vector2 position)
    {
        if(tilemap == null)
            return new Vector2Int(0, 0);

        var pos = tilemap.WorldToCell(position);
        return new Vector2Int(pos.x, pos.y);
    }


    public Vector2Int GetTileIdLocal(Vector2 position)
    {
        var pos = tilemap.LocalToCell(position);
        return new Vector2Int(pos.x, pos.y);
    }

    public List<AStarSharp.Node> GetNearbyNodes(AStarSharp.Node node, WalkData data)
    {
        var list = new List<AStarSharp.Node>();

        if (data.flying)
        {
            AddFlyingNodes(list, data, node.Id + new Vector2Int(0, -1), node);
            AddFlyingNodes(list, data, node.Id + new Vector2Int(0, 1), node);
            AddFlyingNodes(list, data, node.Id + new Vector2Int(-1, 0), node);
            AddFlyingNodes(list, data, node.Id + new Vector2Int(1, 0), node);
            return list;
        }


        var floor = GetNode(node.Id + new Vector2Int(0, -1));
        if (floor == null)
            return list;

        if (node.Ladder && data.canUseLadder)
        {
            AddJumpNodes(list, data, node.Id + new Vector2Int(0, 1), node);
            AddWalkableNode(list, data, node.Id + new Vector2Int(1, 0), node);
            AddWalkableNode(list, data, node.Id + new Vector2Int(-1, 0), node);
            AddWalkableNode(list, data, node.Id + new Vector2Int(1, 1), node);
            AddWalkableNode(list, data, node.Id + new Vector2Int(-1, 1), node);
            AddWalkableNode(list, data, node.Id + new Vector2Int(0, 1), node);
            AddFallNodes(list, data, node.Id + new Vector2Int(0, -1), node);
            AddFallNodes(list, data, node.Id + new Vector2Int(1, -1), node);
            AddFallNodes(list, data, node.Id + new Vector2Int(-1, -1), node);
        }
        else if (floor.Platform)
        {
            AddWalkableNode(list, data, node.Id + new Vector2Int(1, 0), node);
            AddWalkableNode(list, data, node.Id + new Vector2Int(-1, 0), node);
            AddJumpNodes(list, data, node.Id + new Vector2Int(1, 1), node);
            AddJumpNodes(list, data, node.Id + new Vector2Int(0, 1), node);
            AddJumpNodes(list, data, node.Id + new Vector2Int(-1, 1), node);
            AddFallNodes(list, data, node.Id + new Vector2Int(0, -1), node);
            AddFallNodes(list, data, node.Id + new Vector2Int(1, -1), node);
            AddFallNodes(list, data, node.Id + new Vector2Int(-1, -1), node);
        }
        else if (floor.Block)
        {
            AddWalkableNode(list, data, node.Id + new Vector2Int(1, 0), node);
            AddWalkableNode(list, data, node.Id + new Vector2Int(-1, 0), node);
            AddJumpNodes(list, data, node.Id + new Vector2Int(1, 1), node);
            AddJumpNodes(list, data, node.Id + new Vector2Int(0, 1), node);
            AddJumpNodes(list, data, node.Id + new Vector2Int(-1, 1), node);
        }
        else if (node.jumpHeightLeft == 0)
        {
            AddFallNodes(list, data, node.Id + new Vector2Int(0, -1), node);
            if (node.jumpDistanceLeft > 0)
            {
                AddFallNodes(list, data, node.Id + new Vector2Int(-1, -1), node);
                AddFallNodes(list, data, node.Id + new Vector2Int(1, -1), node);
            }
        }
        else if (node.jumpHeightLeft == 1)
        {
            AddFallNodes(list, data, node.Id + new Vector2Int(0, -1), node);

            if (node.jumpDistanceLeft > 0)
            {
                AddFallNodes(list, data, node.Id + new Vector2Int(1, -1), node);
                AddFallNodes(list, data, node.Id + new Vector2Int(-1, -1), node);
                AddWalkableNode(list, data, node.Id + new Vector2Int(1, 0), node);
                AddWalkableNode(list, data, node.Id + new Vector2Int(-1, 0), node);
            }
        }
        else if (node.jumpHeightLeft > 1)
        {
            AddUpNodes(list, data, node.Id + new Vector2Int(0, 1), node);
            if (node.jumpDistanceLeft > 0)
            {
                AddUpNodes(list, data, node.Id + new Vector2Int(1, 1), node);
                AddUpNodes(list, data, node.Id + new Vector2Int(-1, 1), node);
            }
        }
        return list;
    }

    private void AddFallNodes(in List<AStarSharp.Node> nodes, WalkData data, Vector2Int Id, AStarSharp.Node parent)
    {
        var node = GetNode(Id);
        node.Weight = 3;
        if (node == null || node.Block)
            return;

        for (int i = Id.y + 1; i < Id.y + data.height; i++)
        {
            var above = GetNode(new Vector2Int(Id.x, i));
            if (node == null || above.Block)
                return;
        }

        var floor = GetNode(new Vector2Int(Id.x, Id.y - 1));

        if (floor == null)
            return;

        if (floor.Block || floor.Platform)
        {
            node.Parent = parent;
            nodes.Add(node);
        }
        else if (floor.Ladder && data.canUseLadder)
        {
            node.Weight = 1;
            node.Parent = parent;
            nodes.Add(node);
        }
        else
        {
            node.Parent = parent;
            node.jumpHeightLeft = 0;
            node.jumpDistanceLeft = Math.Max(0, parent.jumpDistanceLeft - 1);
            nodes.Add(node);
        }
    }

    private void AddWalkableNode(in List<AStarSharp.Node> nodes, WalkData data, Vector2Int Id, AStarSharp.Node parent)
    {
        var node = GetNode(Id);
        if (node == null || node.Block)
            return;

        for (int i = Id.y + 1; i < Id.y + data.height; i++)
        {
            var above = GetNode(new Vector2Int(Id.x, i));
            if (node == null || above.Block)
                return;
        }

        var floor = GetNode(new Vector2Int(Id.x, Id.y - 1));

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

    private void AddJumpNodes(in List<AStarSharp.Node> nodes, WalkData data, Vector2Int Id, AStarSharp.Node parent)
    {
        var node = GetNode(Id);
        if (node == null || node.Block)
            return;
        node.Weight = 3;
        for (int i = Id.y + 1; i < Id.y + data.height; i++)
        {
            var above = GetNode(new Vector2Int(Id.x, i));
            if (node == null || above.Block)
                return;
        }

        if (node.Ladder && data.canUseLadder)
        {
            node.Weight = 1;
            node.Parent = parent;
            nodes.Add(node);
        }
        else
        {
            node.Parent = parent;
            node.jumpHeightLeft = data.jumpHeight;
            node.jumpDistanceLeft = data.jumpDistance;
            nodes.Add(node);
        }
    }

    public void AddFlyingNodes(in List<AStarSharp.Node> nodes, WalkData data, Vector2Int Id, AStarSharp.Node parent)
    {
        var node = GetNode(Id);
        if (node == null || node.Block)
            return;

        node.Weight = 1;
        for (int i = Id.y + 1; i < Id.y + data.height; i++)
        {
            var above = GetNode(new Vector2Int(Id.x, i));
            if (node == null || above.Block)
                return;
        }

        // if blocks are nearby, then dont like coming near them
        for (int x = -1; x <= 1; x++)
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 || y == 0)
                    continue;

                var above = GetNode(new Vector2Int(Id.x + x, Id.y + y));
                if (node == null || above.Block)
                    node.Weight += 4;
            }

       
        node.Parent = parent;
        nodes.Add(node);
    }

    private void AddUpNodes(in List<AStarSharp.Node> nodes, WalkData data, Vector2Int Id, AStarSharp.Node parent)
    {
        var node = GetNode(Id);
        if (node == null || node.Block)
            return;

        node.Weight = 3;

        for (int i = Id.y + 1; i < Id.y + data.height; i++)
        {
            var above = GetNode(new Vector2Int(Id.x, i));
            if (node == null || above.Block)
                return;
        }

        if (node.Ladder && data.canUseLadder)
        {
            node.Weight = 1;
            node.Parent = parent;
            nodes.Add(node);
        }
        else if (parent.jumpHeightLeft > 0)
        {
            node.Parent = parent;
            node.jumpHeightLeft = Mathf.Max(parent.jumpHeightLeft - 1, 0);
            node.jumpDistanceLeft = Mathf.Max(parent.jumpDistanceLeft - 1, 0);
            nodes.Add(node);
        }
    }

    public Stack<AStarSharp.Node> GetPath(Vector2Int start, Vector2Int end, WalkData data, CancellationToken ct = default)
    {
        var astar = new AStarSharp.Astar();
        astar.GetAdjacentNodes = (it) => { return GetNearbyNodes(it, data); };
        astar.GetNode = (it) => { return GetNode(it); };
        return astar.FindPath(start, end, ct);
    }
}

[Serializable]
public struct WalkData
{
    public float maxTimeOnNode;
    public bool flying;
    public bool canSwim;
    public bool canUseLadder;
    public int height;
    public int jumpHeight;
    public int jumpDistance;
}

namespace AStarSharp
{
    public class Node
    {
        public Node Parent;
        public Vector2Int Id;

        public float DistanceToTarget;
        public float Cost;
        public float Weight;

        public float F
        {
            get
            {
                if (DistanceToTarget != -1 && Cost != -1)
                    return DistanceToTarget + Cost;
                else
                    return -1;
            }
        }

        public bool Block;
        public bool Ladder;
        public bool Platform;
        public bool Destroyable;
        public int jumpHeightLeft = -1;
        public int jumpDistanceLeft = -1;

        // override object.Equals
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }
            var compare = obj as Node;
            return this.Id == compare.Id
                && this.jumpHeightLeft == compare.jumpHeightLeft
                && this.jumpDistanceLeft == compare.jumpDistanceLeft;
        }
    }

    public class Astar
    {
        public Func<Vector2Int, Node> GetNode;
        public Func<Node, List<Node>> GetAdjacentNodes;

        public int MaxSize = 500;

        public Stack<Node> FindPath(Vector2Int Start, Vector2Int End, CancellationToken ct = default)
        {
            Node start = GetNode(Start);
            Node end = GetNode(End);

            Stack<Node> Path = new Stack<Node>();
            List<Node> OpenList = new List<Node>();
            List<Node> ClosedList = new List<Node>();
            List<Node> adjacencies;
            Node current = start;

            // add start node to Open List
            OpenList.Add(start);

            while (OpenList.Count != 0 && !ClosedList.Exists(x => x.Id == end.Id) && ClosedList.Count < MaxSize)
            {
                if (ct.IsCancellationRequested)
                    return null;
                current = OpenList[0];
                OpenList.Remove(current);
                ClosedList.Add(current);
                adjacencies = GetAdjacentNodes(current);

                foreach (Node n in adjacencies)
                {
                    if (ct.IsCancellationRequested)
                        return null;
                    if (!ClosedList.Contains(n))
                    {
                        if (!OpenList.Contains(n))
                        {
                            n.Parent = current;
                            n.DistanceToTarget = Vector2.Distance(n.Id, end.Id);
                            n.Cost = n.Weight + n.Parent.Cost;
                            OpenList.Add(n);
                            OpenList = OpenList.OrderBy(node => node.F).ToList<Node>();
                        }
                    }
                }
            }

            if (ct.IsCancellationRequested)
                return null;

            // construct path, if end was not closed return null
            if (!ClosedList.Exists(x => x.Id == end.Id))
            {
                return null;
            }

            // if all good, return path
            Node temp = ClosedList[ClosedList.IndexOf(current)];
            if (temp == null) return null;
            do
            {
                if (ct.IsCancellationRequested)
                    return null;
                Path.Push(temp);
                temp = temp.Parent;
            } while (temp != start && temp != null);
            return Path;
        }
    }
}