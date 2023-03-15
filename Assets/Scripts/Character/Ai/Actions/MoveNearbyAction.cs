using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace RuinsRaiders.AI
{
    [CreateAssetMenu(fileName = "MoveNearbyAction", menuName = "RuinsRaiders/Ai/MoveNearbyAction", order = 1)]
    public class MoveNearbyAction : BasicAiActionData
    {
        public float minDistance;
        public float maxDistance;
        public int minHeight;
        public int maxHeight;


        public override BasicAiAction Create(ActivatorData trigger)
        {
            return new Action(this, trigger);
        }

        public class Action : BasicAiAction
        {
            private float _minDistance;
            private float _maxDistance;
            private int _minHeight;
            private int _maxHeight;

            private PathfindingController _pathfindingController;
            private MovementController _movementController;

            private GameObject _target;
            public override bool CanStop()
            {
                return IsFinished() || (_movementController.IsGrounded || _movementController.flying);
            }

            public Action(MoveNearbyAction parent, ActivatorData trigger)
            {
                _minDistance = parent.minDistance;
                _maxDistance = parent.maxDistance;

                _minHeight = parent.minHeight;
                _maxHeight = parent.maxHeight;

                _target = trigger.triggeredBy;
                if (_target == null)
                    return;

                _pathfindingController = trigger.triggeredFor.GetComponent<PathfindingController>();
                _movementController = trigger.triggeredFor.GetComponent<MovementController>();

                if (_pathfindingController == null)
                    return;

                var targetId = _pathfindingController.GetAStar().GetTileId(_target.transform.position);

                Func<AStarSharp.Node, bool> IsTarget = (node) =>
                {
                    var height = node.Id.y - targetId.y;
                    if (height < _minHeight || height > _maxHeight)
                        return false;

                    var distance = Vector2.Distance(targetId, node.Id);
                    if (distance < _minDistance || distance > _maxDistance)
                        return false;

                    return true;

                };

                Func<AStarSharp.Node, float> GetCost = (node) =>
                {
                    var cost = node.Weight + node.Parent.Cost;
                    if (_minDistance > 1)
                        cost += (1f - Mathf.Min(Vector2.Distance(targetId, node.Id) / _minDistance, 1)) * 4;
                    return cost;
                };

                Func<AStarSharp.Node, float> GetDistance = (node) =>
                {
                    return Vector2.Distance(node.Id, targetId);
                };

                _pathfindingController.MoveTo(IsTarget, GetCost, GetDistance, true);

            }

            public override bool IsFinished()
            {
                return _pathfindingController == null || _pathfindingController.IsMoving == false;
            }

            public override void Update(float dt)
            {
                if (stopped)
                    return;
                
            }
        }
    }
}
