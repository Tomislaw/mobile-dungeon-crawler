using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Threading;

namespace RuinsRaiders
{
    [ExecuteAlways]
    public class PathfindingController : MonoBehaviour
    {
        [SerializeField]
        private AStar astar;
        [SerializeField]
        private WalkData data;

        [SerializeField]
        private MovementController character;

        [SerializeField]
        private int maxNodesPerBatch = 10;
        [SerializeField]
        private int maxNodes = 600;

        private Coroutine _coroutine;
        private CancellationTokenSource _cancellationTokenSource;

        private float _timeLeftOnNode;
        private Vector2Int _movingToTile;
        private Stack<AStarSharp.Node> _nodes;

        public Vector2Int Target
        {
            get
            {
                if (!IsMoving)
                    return GetCurrentTileId;

                return _movingToTile;
            }
        }

        private void OnEnable()
        {
            if (character == null)
                character = GetComponent<MovementController>();
            if (astar == null)
                astar = FindAStar();
        }

        public void MoveToId(Vector2Int id, bool moveToClosestTileIfFail = false)
        {
            StopMoving();
            _coroutine = StartCoroutine(MoveToCoroutine(id, moveToClosestTileIfFail));
        }

        public void MoveTo(Vector2 position, bool moveToClosestTileIfFail = false)
        {
            StopMoving();
            _coroutine = StartCoroutine(MoveToCoroutine(position, moveToClosestTileIfFail));
        }

        private IEnumerator MoveToCoroutine(Vector2 position, bool moveToClosestTileIfFail = false)
        {
            return MoveToIdCoroutine(astar.GetTileId(position), moveToClosestTileIfFail);
        }

        private IEnumerator MoveToIdCoroutine(Vector2Int id, bool moveToClosestTileIfFail = false)
        {
            _cancellationTokenSource = new CancellationTokenSource();
            var start = StartingTileId();
            var end = id;

            var enumerator = astar.GetPath(start, end, data, maxNodesPerBatch, maxNodes, _cancellationTokenSource.Token);
            while (enumerator.MoveNext())
            {
                if (enumerator.Current.Finished)
                {
                    if (!enumerator.Current.PathFound && !moveToClosestTileIfFail)
                    {
                        _coroutine = null;
                        yield break;
                    }

                    _nodes = enumerator.Current.Path;
                    _movingToTile = id;
                    _timeLeftOnNode = data.maxTimeOnNode;
                    _coroutine = null;
                    yield break;
                }
                yield return new WaitForFixedUpdate();
            }
        }

        private Vector2Int StartingTileId()
        {
            var start = GetCurrentTileId;
            if (data.flying)
            {

            }
            else if (character.IsGrounded && !CanWalkOnTile(start))
            {
                // scenario when character is standing on corner of tile
                if (CanWalkOnTile(start - new Vector2Int(1, 0)))
                    start -= new Vector2Int(1, 0);
                else if (CanWalkOnTile(start + new Vector2Int(1, 0)))
                    start += new Vector2Int(1, 0);
            }
            return start;
        }

        public void StopMoving()
        {
            if (_cancellationTokenSource != null)
                _cancellationTokenSource.Cancel();
            character.Stop();
            if (_nodes != null)
                _nodes.Clear();

            if (_coroutine != null)
                StopCoroutine(_coroutine);

            _coroutine = null;
        }

        public bool IsMoving { get => _coroutine != null || _nodes != null && _nodes.Count > 0; }

        // Update is called once per frame
        private void FixedUpdate()
        {
            if (_timeLeftOnNode < 0 && _nodes != null)
            {
                _nodes.Clear();
                character.Stop();
            }

            if (_nodes != null && _nodes.Count > 0)
            {
                _timeLeftOnNode -= Time.fixedDeltaTime;

                var node = _nodes.Peek();

                var current = GetCurrentTileId;
                if (node.Id.x == current.x && Mathf.Abs(node.Id.y - current.y) <= 1)
                {
                    _nodes.Pop();
                    _timeLeftOnNode = data.maxTimeOnNode;
                }

                Vector2 move = new();

                if (node.Id.x > current.x)
                    move.x = 1;
                else if (node.Id.x < current.x)
                    move.x = -1;

                if (node.Id.y > current.y)
                    move.y = 1;

                if (node.Parent?.Platform == true && !node.Platform || node.Ladder && character.canUseLadder || character.IsOnLadder)
                    if (node.Id.y < current.y)
                        move.y = -1;

                character.Move(move);

                if (_nodes.Count == 0)
                    character.Stop();

            }
        }

        private void OnDrawGizmos()
        {
            if (astar == null || _nodes == null)
                return;
            foreach (var node in _nodes)
                DrawNode(node);
        }

        public int RemainingNodes()
        {
            if (_nodes == null)
                return 0;

            return _nodes.Count;
        }

        private void DrawNode(AStarSharp.Node node)
        {

            var pos = astar.GetPositionFromId(node.Id);
            if (node.jumpDistanceLeft > 0 || node.jumpHeightLeft > 0)
                Gizmos.color = Color.blue;
            Gizmos.DrawSphere(new Vector3(pos.x, pos.y, 0), 0.2f);
            Gizmos.color = Color.white;
        }

        public bool CanWalkOnPosition(Vector2 position)
        {
            return CanWalkOnTile(GetTileId(position));
        }
        public bool CanWalkOnTile(Vector2Int Id)
        {
            for (int i = Id.y; i < Id.y + data.height; i++)
            {
                var above = astar.GetNode(new Vector2Int(Id.x, i));
                if (above.Block || above.Spike)
                    return false;
            }

            var floor = astar.GetNode(new Vector2Int(Id.x, Id.y - 1));

            return (floor.Block || floor.Platform) && !floor.Spike;
        }

        public Vector2Int GetTileId(Vector2 position)
        {
            return astar.GetTileId(position + new Vector2(0, 0.1F));
        }

        public Vector2Int GetCurrentTileId { get => GetTileId(transform.position); }

        private AStar FindAStar()
        {
            var astars = FindObjectsOfType<AStar>().OrderBy(it => Vector2.Distance(it.transform.position, transform.position));
            if (astars.Count() == 0)
                return null;
            return astars.First();
        }

        //Public Getters (Used for tests at the moment)
        public AStar GetAStar()
        {
            return astar;
        }

        public WalkData GetWalkData()
        {
            return data;
        }
    }
}