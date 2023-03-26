using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Threading;
using System;

namespace RuinsRaiders
{
    [ExecuteAlways]
    public class PathfindingController : MonoBehaviour
    {
        private static float MinDistanceToMove = 0.1f;
        private static int MinNodesToFinishWhenMoving = 3;

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
        private Stack<AStarSharp.Node> _nodes = new();

        private void Start()
        {
            data.canSwim = character.canSwim;
            data.canUseLadder = character.canUseLadder;
            data.canUsePlatform = character.canUsePlatform;
            data.flying = character.flying;
        }

        public Vector2Int Target
        {
            get
            {
                if (!IsMoving)
                    return TileId;

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

        public void MoveTo(Vector2Int id, bool moveToClosestTileIfFail = false)
        {
            _coroutine = StartCoroutine(MoveToCoroutine((Vector2)id, moveToClosestTileIfFail));
        }

        public void MoveTo(Vector2 position, bool moveToClosestTileIfFail = false)
        {
            _coroutine = StartCoroutine(MoveToCoroutine(position, moveToClosestTileIfFail));
        }

        public void MoveTo(
            Func<AStarSharp.Node, bool> IsTarget,
            Func<AStarSharp.Node, float> GetCost,
            Func<AStarSharp.Node, float> GetDistance,
            bool moveToClosestTileIfFail = false)
        {
            _coroutine = StartCoroutine(MoveToCoroutine(IsTarget, GetCost, GetDistance, moveToClosestTileIfFail));
        }

        private IEnumerator MoveToCoroutine(Vector2 position, bool moveToClosestTileIfFail = false)
        {
            return MoveToCoroutine(astar.GetTileId(position), moveToClosestTileIfFail);
        }

        private IEnumerator MoveToCoroutine(Vector2Int id, bool moveToClosestTileIfFail = false)
        {
            return MoveToCoroutine(n => n.Id == id, n => n.Weight + n.Parent.Cost, n => Vector2.Distance(n.Id, id));
        }

        private IEnumerator MoveToCoroutine(
            Func<AStarSharp.Node, bool> IsTarget,
            Func<AStarSharp.Node, float> GetCost,
            Func<AStarSharp.Node, float> GetDistance,
            bool moveToClosestTileIfFail = false)
        {
            _cancellationTokenSource = new CancellationTokenSource();
            var start = StartingTileId();

            var enumerator = astar.GetPath(start, IsTarget, GetCost, GetDistance, data, maxNodesPerBatch, maxNodes, _cancellationTokenSource.Token);
            while (enumerator.MoveNext())
            {
                if (enumerator.Current.Finished != true)
                {
                    yield return new WaitForFixedUpdate();
                    continue;
                }

                if (!enumerator.Current.PathFound && !moveToClosestTileIfFail)
                {
                    _coroutine = null;
                    yield break;
                }

                if (enumerator.Current.Path == null || enumerator.Current.Path.Count == 0)
                {
                    yield break;
                }

                var startNode = _nodes.FirstOrDefault(it => it.Id == TileId);

                // case whe target is already moving
                if (_nodes.Count > 0)
                {
                    var pathContinue = enumerator.Current.Path.ToList();
                    var newFirst = pathContinue.First();
                    var startingNodes = _nodes.TakeWhile(it => it.Id != newFirst.Id).ToList();
                    if (startingNodes.Count() == _nodes.Count())
                    {
                        _nodes.Clear();
                        yield break;
                    }
                    var startLast = startingNodes.LastOrDefault();
                    if (startLast != null)
                        newFirst.Parent = startLast;

                    var adjustedPath = startingNodes.Concat(pathContinue).Reverse().ToList();
                    _nodes = new Stack<AStarSharp.Node>(adjustedPath);
                }
                else // case when target is not moving
                {
                    _nodes = enumerator.Current.Path;
                }
                _movingToTile = _nodes.Last().Id;
                _timeLeftOnNode = data.maxTimeOnNode;
                _coroutine = null;
                yield break;
            }
        }


        private Vector2Int StartingTileId()
        {
            var start = TileId;
            // scenario when moving
            if (_nodes.Count > 0)
            {
                var startingNode = _nodes.Skip(MinNodesToFinishWhenMoving).FirstOrDefault(it=> CanMoveToTile(it.Id));
                if (startingNode != null)
                    start = startingNode.Id;
            }
            else if (character.IsGrounded && !CanMoveToTile(start))
            {
                // scenario when character is standing on corner of tile
                if (CanMoveToTile(start - new Vector2Int(1, 0)))
                    start -= new Vector2Int(1, 0);
                else if (CanMoveToTile(start + new Vector2Int(1, 0)))
                    start += new Vector2Int(1, 0);
            }
            return start;
        }

        public void Stop()
        {
            if (_cancellationTokenSource != null)
                _cancellationTokenSource.Cancel();
            character.Stop();
            _nodes.Clear();

            if (_coroutine != null)
                StopCoroutine(_coroutine);

            _coroutine = null;
        }

        public bool IsMoving { get => _coroutine != null  || _nodes.Count > 0; }

        // Update is called once per frame
        private void FixedUpdate()
        {
            if (_timeLeftOnNode < 0)
            {
                _nodes.Clear();
                character.Stop();
            }

            if (_nodes.Count > 0)
            {
                _timeLeftOnNode -= Time.fixedDeltaTime;

                var node = _nodes.Peek();

                if (IsNodeReached(node))
                {
                    _nodes.Pop();
                    _timeLeftOnNode = data.maxTimeOnNode;
                }

                character.Move(GetMovement(node));

                if (_nodes.Count == 0)
                    character.Stop();
            }
        }

        private Vector2 GetMovement(AStarSharp.Node node)
        {
            var tilePosition = astar.GetPositionFromId(node.Id);
            var position = new Vector2(transform.position.x, transform.position.y);
            var distance = tilePosition - position;
            Vector2 move = new();

            if (Mathf.Abs(distance.x) > MinDistanceToMove)
                move.x = Mathf.Clamp(distance.x, -1, 1);

            if (character.flying
                || character.IsSwimming && character.canSwim
                || character.canUseLadder && character.ladders.Count > 0)
            {
                if (Mathf.Abs(distance.y) > MinDistanceToMove)
                    move.y = Mathf.Clamp(distance.y, -1, 1);
            }
            else
            {
                var tileDifference = node.Id.y - TileId.y;
                if (tileDifference > 0)
                    move.y = 1;
                else if (tileDifference < 0 &&
                    (
                     (node.Platform == true && character.canUsePlatform)
                    || (node.Ladder && character.canUseLadder)
                    ))
                    move.y = -1;
            }

            return move;
        }

        private bool IsNodeReached(AStarSharp.Node node)
        {
            var currentTileId = TileId;
            if (character.flying // when flying
                || character.canSwim && node.Water // when swimming
                || character.canUseLadder && node.Ladder) // when on ladder
            {
                return node.Id.x == currentTileId.x && node.Id.y == currentTileId.y;
            }
            else if ((node.Block || node.Platform)  // when walking
                && (node.jumpDistanceLeft <= 0 || node.jumpHeightLeft <= 0)
                )
            {
                return node.Id.x == currentTileId.x
                    && node.Id.y == currentTileId.y || node.Id.y == currentTileId.y + 1
                    && character.IsGrounded;
            }
            else // when jumping or falling
            {
                bool yReached = currentTileId.y >= node.Id.y && character.Velocity.y > 0
                    || currentTileId.y <= node.Id.y && character.Velocity.y < 0
                    || currentTileId.y == node.Id.y;

                bool xReached = currentTileId.x >= node.Id.x && character.Velocity.x > 0
                    || currentTileId.x <= node.Id.x && character.Velocity.x < 0
                    || currentTileId.x == node.Id.x;
                return xReached && yReached;
            }
        }

        private void OnDrawGizmos()
        {
            if (astar == null)
                return;
            foreach (var node in _nodes)
                DrawNode(node);
        }

        public int RemainingNodes()
        {
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

        public bool CanMoveOnPosition(Vector2 position)
        {
            return CanMoveToTile(GetTileId(position));
        }
        public bool CanMoveToTile(Vector2Int Id)
        {
            for (int i = Id.y; i < Id.y + data.height; i++)
            {
                var above = astar.GetNode(new Vector2Int(Id.x, i));
                if (above.Block || above.Spike)
                    return false;
            }

            var node = astar.GetNode(Id);
            if (node.Water)
                return character.canSwim;

            if (character.flying)
                return true;

            if(character.canUseLadder && node.Ladder)
                return true;

            if(node.Spike)
                return false;

            var floor = astar.GetNode(new Vector2Int(Id.x, Id.y - 1));
            return CanStand(Id);
        }

        public bool CanStand(Vector2Int Id)
        {
            var floor = astar.GetNode(new Vector2Int(Id.x, Id.y - 1));
            return floor.Platform || floor.Block;
        }

        public Vector2Int GetTileId(Vector2 position)
        {
            return astar.GetTileId(position + new Vector2(0, 0.1F));
        }

        public Vector2Int TileId { get => GetTileId(transform.position); }

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