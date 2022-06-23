using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using UnityEngine.Tilemaps;
using System.Threading;
using UnityEngine.Events;
using static RuinsRaiders.AStarSharp.Astar;

namespace RuinsRaiders
{
    [RequireComponent(typeof(Tilemap))]
    public class AStar : MonoBehaviour
    {
        public UnityEvent onMapUpdated = new();

        private Tilemap _tilemap;
        private readonly HashSet<Vector2Int> _dynamicTiles = new();
        private readonly Dictionary<Vector2Int, AStarSharp.Node> _cache = new();

        private bool _mapUpdated = false;

        public void AddDynamicBlockTile(Vector2Int tileId)
        {
            _dynamicTiles.Add(tileId);
            _mapUpdated = true;
        }

        public void RemoveDynamicBlockTile(Vector2Int tileId)
        {
            _dynamicTiles.Remove(tileId);
            _mapUpdated = true;
        }

        private void Awake()
        {
            _tilemap = GetComponent<Tilemap>();
        }

        private void FixedUpdate()
        {
            if (_mapUpdated)
            {
                onMapUpdated.Invoke();
                _mapUpdated = false;
            }
        }

        public AStarSharp.Node GetNode(Vector2Int Id)
        {
            var cachedNode = _cache.GetValueOrDefault(Id);
            if (cachedNode == null)
            {
                cachedNode = new AStarSharp.Node
                {
                    Id = Id
                };
                var collider = _tilemap.GetColliderType(new Vector3Int(Id.x, Id.y, 0));
                var gameobject = _tilemap.GetInstantiatedObject(new Vector3Int(Id.x, Id.y, 0));
                if (gameobject != null)
                {
                    cachedNode.Spike = gameobject.GetComponentInChildren<SpikeTile>() != null;
                    cachedNode.Ladder = gameobject.GetComponentInChildren<LadderTile>() != null;
                    cachedNode.Platform = gameobject.GetComponentInChildren<PlatformTile>() != null;
                }
                cachedNode.Block = collider != Tile.ColliderType.None;
                cachedNode.Weight = 1;
                cachedNode.Cost = 1;

                _cache.Add(Id, cachedNode);
            }

            var node = cachedNode.Clone();

            if (node.Block)
                return node;

            node.Destroyable = _dynamicTiles.Contains(Id);
            node.Block = node.Destroyable;

            return node;
        }

        public Vector2 GetPositionFromId(Vector2Int id)
        {
            var pos = _tilemap.CellToWorld(new Vector3Int(id.x, id.y, 0));
            return new Vector2(pos.x + 0.5f, pos.y + 0.5f);
        }

        public Vector2Int GetTileId(Vector2 position)
        {
            if (_tilemap == null)
                return new Vector2Int(0, 0);

            var pos = _tilemap.WorldToCell(position);
            return new Vector2Int(pos.x, pos.y);
        }


        public Vector2Int GetTileIdLocal(Vector2 position)
        {
            var pos = _tilemap.LocalToCell(position);
            return new Vector2Int(pos.x, pos.y);
        }

        public List<AStarSharp.Node> GetNearbyNodes(AStarSharp.Node node, WalkData data)
        {
            var list = new List<AStarSharp.Node>();

            if (node.Spike)
                return list;

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
            else if (node.jumpHeightLeft <= 0)
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
            node.Weight = 2;

            // stop if node is blocked
            if (node == null || node.Block)
                return;

            // stop if character is too tall
            for (int i = Id.y + 1; i < Id.y + data.height; i++)
            {
                var above = GetNode(new Vector2Int(Id.x, i));
                if (node == null || above.Block)
                    return;
            }

            var floor = GetNode(new Vector2Int(Id.x, Id.y - 1));

            // stop if node is out of bounds
            if (floor == null)
                return;

            // if can stand on tile  
            if (floor.Block || floor.Platform)
            {
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

        //public Stack<AStarSharp.Node> GetPath(Vector2Int start, Vector2Int end, WalkData data, int maxNodes = 600, CancellationToken ct = default)
        //{
        //    var astar = new AStarSharp.Astar();
        //    astar.GetAdjacentNodes = (it) => { return GetNearbyNodes(it, data); };
        //    astar.GetNode = (it) => { return GetNode(it); };
        //    return astar.FindPath(start, end, maxNodes, ct);
        //}

        public IEnumerator<FindPathStatus> GetPath(Vector2Int start, Vector2Int end, WalkData data, int maxNodesPerBatch = 10, int maxNodes = 600, CancellationToken ct = default)
        {
            var astar = new AStarSharp.Astar
            {
                GetAdjacentNodes = (it) => { return GetNearbyNodes(it, data); },
                GetNode = (it) => { return GetNode(it); }
            };
            return astar.FindPath(start, end, maxNodesPerBatch, maxNodes, ct);
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

            public bool Spike;
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

            public Node Clone()
            {
                return (Node)this.MemberwiseClone();
            }

            public override int GetHashCode()
            {
                return base.GetHashCode();
            }
        }

        public class Astar
        {
            public Func<Vector2Int, Node> GetNode;
            public Func<Node, List<Node>> GetAdjacentNodes;

            public struct FindPathStatus
            {
                public bool Finished;
                public bool PathFound;
                public Stack<Node> Path;
            }

            public IEnumerator<FindPathStatus> FindPath(Vector2Int Start, Vector2Int End, int SizePerBatch = 10, int MaxSize = 600, CancellationToken ct = default)
            {
                Node start = GetNode(Start);
                Node end = GetNode(End);

                Stack<Node> Path = new();
                List<Node> OpenList = new();
                List<Node> ClosedList = new();
                List<Node> adjacencies;
                Node current = start;

                // add start node to Open List
                OpenList.Add(start);

                while (OpenList.Count != 0 && !ClosedList.Exists(x => x.Id == end.Id) && ClosedList.Count < MaxSize)
                {
                    if (ct.IsCancellationRequested || ClosedList.Count % SizePerBatch == 0)
                        yield return new FindPathStatus();

                    current = OpenList[0];
                    OpenList.Remove(current);
                    ClosedList.Add(current);
                    adjacencies = GetAdjacentNodes(current);

                    foreach (Node n in adjacencies)
                    {
                        if (ct.IsCancellationRequested)
                            yield return new FindPathStatus();

                        if (ClosedList.Contains(n))
                            continue;
                        if (OpenList.Contains(n))
                            continue;

                        n.Parent = current;
                        n.DistanceToTarget = Vector2.Distance(n.Id, end.Id);
                        n.Cost = n.Weight + n.Parent.Cost;
                        OpenList.Add(n);
                        OpenList = OpenList.OrderBy(node => node.F).ToList<Node>();

                    }
                };

                var pathFound = true;
                // construct path, if end was not closed return null
                if (!ClosedList.Exists(x => x.Id == end.Id))
                {
                    current = ClosedList.Where(it => it.jumpHeightLeft == -1 && !it.Spike).OrderBy(it => Vector2Int.Distance(it.Id, End)).First();
                    pathFound = false;
                }

                // if all good, return path
                Node temp = ClosedList[ClosedList.IndexOf(current)];
                if (temp == null)
                {
                    yield return new FindPathStatus() { Finished = true, PathFound = false };
                    yield break;
                }

                do
                {
                    Path.Push(temp);
                    temp = temp.Parent;
                } while (temp != start && temp != null);

                yield return new FindPathStatus() { Finished = true, PathFound = pathFound, Path = Path };
                yield break;

            }


            public (bool, Stack<Node>) FindPath(Vector2Int Start, Vector2Int End, int MaxSize = 600, CancellationToken ct = default)
            {
                Node start = GetNode(Start);
                Node end = GetNode(End);

                Stack<Node> Path = new();
                List<Node> OpenList = new();
                List<Node> ClosedList = new();
                List<Node> adjacencies;
                Node current = start;

                // add start node to Open List
                OpenList.Add(start);

                while (OpenList.Count != 0 && !ClosedList.Exists(x => x.Id == end.Id) && ClosedList.Count < MaxSize)
                {
                    if (ct.IsCancellationRequested)
                        return (false, null);
                    current = OpenList[0];
                    OpenList.Remove(current);
                    ClosedList.Add(current);
                    adjacencies = GetAdjacentNodes(current);

                    foreach (Node n in adjacencies)
                    {
                        if (ct.IsCancellationRequested)
                            return (false, null);
                        if (ClosedList.Contains(n))
                            continue;
                        if (OpenList.Contains(n))
                            continue;

                        n.Parent = current;
                        n.DistanceToTarget = Vector2.Distance(n.Id, end.Id);
                        n.Cost = n.Weight + n.Parent.Cost;
                        OpenList.Add(n);
                        OpenList = OpenList.OrderBy(node => node.F).ToList<Node>();
                    }
                }

                if (ct.IsCancellationRequested)
                    return (false, null);


                // construct path, if end was not closed return null
                if (!ClosedList.Exists(x => x.Id == end.Id))
                {
                    current = ClosedList.Where(it => it.jumpHeightLeft == -1 && !it.Spike).OrderBy(it => Vector2Int.Distance(it.Id, End)).First();
                }

                // if all good, return path
                Node temp = ClosedList[ClosedList.IndexOf(current)];
                if (temp == null) return (false, null);
                do
                {
                    if (ct.IsCancellationRequested)
                        return (false, null);

                    Path.Push(temp);
                    temp = temp.Parent;
                } while (temp != start && temp != null);
                return (true, Path);
            }
        }
    }
}