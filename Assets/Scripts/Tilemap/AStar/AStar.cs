﻿using System.Collections.Generic;
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
        private readonly HashSet<GameObject> _dynamicTiles = new();
        private readonly Dictionary<Vector2Int, AStarSharp.Node> _cache = new();

        private bool _mapUpdated = false;

        public void AddDynamicBlockTile(GameObject gameObject)
        {
            var size = _dynamicTiles.Count;
            _dynamicTiles.Add(gameObject);
            _mapUpdated = size != _dynamicTiles.Count;
        }

        public void RemoveDynamicBlockTile(GameObject gameObject)
        {
            var size = _dynamicTiles.Count;
            _dynamicTiles.Remove(gameObject);
            _mapUpdated = size != _dynamicTiles.Count;
        }

        private void Awake()
        {
            _tilemap = GetComponent<Tilemap>();
        }

        private void LateUpdate()
        {
            if (_mapUpdated)
            {
                _mapUpdated = false;
                onMapUpdated.Invoke();
            }
        }

        private void OnDrawGizmos()
        {
            foreach (var node in _cache)
                DrawNode(node.Value);
        }

        private void DrawNode(AStarSharp.Node node)
        {
            var pos = GetPositionFromId(node.Id);

            if (node.Water)
                Gizmos.color = Color.blue;
            else if(node.Spike)
                Gizmos.color = Color.red;
            else if (node.Block)
                Gizmos.color = Color.black;
            else if (node.Platform)
                Gizmos.color = Color.yellow;
            else if (node.Ladder)
                Gizmos.color = Color.green;
            else
                Gizmos.color = Color.white;

            Gizmos.DrawSphere(new Vector3(pos.x, pos.y, 0), 0.1f);
            Gizmos.color = Color.white;
        }

        public GameObject GetGameObject(Vector2Int Id)
        {
            return _tilemap.GetInstantiatedObject(new Vector3Int(Id.x, Id.y, 0));
        }

        public TileBase GetTile(Vector2Int Id)
        {
            return _tilemap.GetTile(new Vector3Int(Id.x, Id.y, 0));
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
                var gameobject = GetGameObject(Id);
                if (gameobject != null)
                {
                    cachedNode.Spike = gameobject.GetComponentInChildren<SpikeTile>() != null;
                    cachedNode.Ladder = gameobject.GetComponentInChildren<LadderTile>() != null;
                    cachedNode.Platform = gameobject.GetComponentInChildren<PlatformTile>() != null;
                    cachedNode.Water = gameobject.GetComponentInChildren<WaterTile>() != null;
                }
                cachedNode.Block = collider != Tile.ColliderType.None;
                cachedNode.Weight = 1;
                cachedNode.Cost = 1;
                _cache.Add(Id, cachedNode);
            }

            var node = cachedNode.Clone();

            if (node.Block)
                return node;

            node.Destroyable = _dynamicTiles.Any(it => GetTileId(it.transform.position) == node.Id);
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
                return FlyingNodesProvider.GetNodes(this, data, node);
            else
                return WalkingNodesProvider.GetNodes(this, data, node);

        }

        public IEnumerator<FindPathStatus> GetPath(Vector2Int start, Vector2Int end, WalkData data, int maxNodesPerBatch = 10, int maxNodes = 600, CancellationToken ct = default)
        {
            var astar = new AStarSharp.Astar
            {
                GetAdjacentNodes = n => GetNearbyNodes(n, data),
                GetNode = n => GetNode(n),
                IsEnd = n => n.Id == end,
                GetCost = n => n.Weight + n.Parent.Cost,
                GetDistance = n => Vector2.Distance(n.Id, end),
                MaxSize = maxNodes,
                SizePerBatch = maxNodesPerBatch,
            };
            return astar.FindPath(start, ct);
        }

        public IEnumerator<FindPathStatus> GetPath(
            Vector2Int start, 
            Func<AStarSharp.Node, bool> IsEnd,
            Func<AStarSharp.Node, float> GetCost,
            Func<AStarSharp.Node, float> GetDistance,
            WalkData data, 
            int maxNodesPerBatch = 10, 
            int maxNodes = 600, 
            CancellationToken ct = default)
        {
            var astar = new AStarSharp.Astar
            {
                GetAdjacentNodes = n => GetNearbyNodes(n, data),
                GetNode = n => GetNode(n),
                IsEnd = n => IsEnd(n),
                GetCost = n => GetCost(n),
                GetDistance = n => GetDistance(n),
                MaxSize = maxNodes,
                SizePerBatch = maxNodesPerBatch,
            };
            return astar.FindPath(start, ct);
        }
    }




}