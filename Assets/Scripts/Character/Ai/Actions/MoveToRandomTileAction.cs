using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace RuinsRaiders.AI
{
    [CreateAssetMenu(fileName = "MoveToRandomTileAction", menuName = "RuinsRaiders/Ai/MoveToRandomTileAction", order = 1)]
    public class MoveToRandomTileAction : BasicAiActionData
    {
        public float AwaitTime = 1;

        [SerializeField]
        private int searchDistanceX = 4;
        [SerializeField]
        private int searchDistanceY = 1;

        [SerializeField]
        private bool onlyGround;

        public override BasicAiAction Create(ActivatorData trigger)
        {
            return new Action(trigger, this);
        }

        public class Action : BasicAiAction
        {
            readonly private MovementController _movementController;
            readonly private PathfindingController _pathfindingController;
            private float awaitTime;
            public Action(ActivatorData trigger, MoveToRandomTileAction parent)
            {
                _movementController = trigger.triggeredFor.GetComponent<MovementController>();
                _pathfindingController = trigger.triggeredFor.GetComponent<PathfindingController>();
                awaitTime = parent.AwaitTime;
                var targetId = _pathfindingController.GetAStar().GetTileId(trigger.triggeredFor.transform.position);

                int random = UnityEngine.Random.Range(0, parent.searchDistanceX * 2 + 1) - parent.searchDistanceX;
                if (random == 0)
                    random = UnityEngine.Random.Range(0, 2) * 2 - parent.searchDistanceX;

                Func<AStarSharp.Node, bool> IsTarget = (node) =>
                {
                    var height = node.Id.y - targetId.y;
                    if (height < -parent.searchDistanceY || height > parent.searchDistanceY)
                        return false;

                    var width = node.Id.x - targetId.x;
                    if (width != random)
                        return false;

                    if (!_movementController.canSwim && node.Tile.Water)
                        return false;

                    if (!_movementController.canUseLadder && node.Tile.Ladder && !_movementController.flying)
                        return false;

                    if (node.Tile.Spike)
                        return false;

                    if (!_movementController.flying || parent.onlyGround)
                        return _pathfindingController.CanStand(node.Id);

                    return true;

                };

                Func<AStarSharp.Node, float> GetCost = (node) =>
                {
                    var cost = node.Weight + node.Parent.Cost;
                    return cost;
                };

                Func<AStarSharp.Node, float> GetDistance = (node) =>
                {
                    return Vector2.Distance(node.Id, targetId);
                };

                _pathfindingController.MoveTo(IsTarget, GetCost, GetDistance, false);

            }


            public override bool CanStop()
            {
                return IsFinished() || (_movementController.IsGrounded || _movementController.flying || _movementController.IsSwimming);
            }
            public override bool IsFinished()
            {
                return !_pathfindingController.IsMoving && awaitTime <= 0;
            }


            public override void Update(float dt)
            {
                if (!_pathfindingController.IsMoving && awaitTime > 0)
                    awaitTime -= dt;
            }

            public override string ToString()
            {
                return "MoveToRandomTileAction;" +
                    "\nawait: " + awaitTime.ToString() +
                    "\nmoving: " + _pathfindingController.IsMoving +
                    "\nnodes: " + _pathfindingController.RemainingNodes() +
                    "\nfinished: " + IsFinished();
            }
        }
    }
}
