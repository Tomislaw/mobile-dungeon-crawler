using System.Threading;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace RuinsRaiders.AI
{
    [CreateAssetMenu(fileName = "ChaseTargetAction", menuName = "RuinsRaiders/Ai/ChaseTargetAction", order = 1)]
    public class ChaseAction : BasicAiActionData
    {
        public float timeBetweenRetryingFindingPath = 0.5f;

        [SerializeField]
        private float resignDistance = -1;
        [SerializeField]
        private float resignTime = 1;

        public override BasicAiAction Create(ActivatorData trigger)
        {
            return new Action(this, trigger);
        }

        public class Action : BasicAiAction
        {
            internal float timeForResign = 1;
            internal float timeToNextPathfinding = 0;
            internal GameObject target;
            internal Character targetCharacter;

            internal Character character;
            internal MovementController movementController;

            internal PathfindingController pathfinding;
            internal ChaseAction parent;

            readonly private CancellationTokenSource _token = new();
            public Action(ChaseAction parent, ActivatorData trigger)
            {
                character = trigger.triggeredFor.GetComponent<Character>();
                pathfinding = trigger.triggeredFor.GetComponent<PathfindingController>();
                movementController = trigger.triggeredFor.GetComponent<MovementController>();

                target = trigger.triggeredBy;
                targetCharacter = target.GetComponent<Character>();
                timeForResign = parent.resignTime;
                this.parent = parent;
            }

            public override bool CanStop()
            {
                return IsFinished() || movementController.IsGrounded;
            }
            public override bool IsFinished()
            {
                return target == null;
            }

            public override void Stop()
            {
                _token.Cancel();
                base.Stop();
            }

            protected bool ShouldResign(float dt)
            {
                if (IsFinished())
                    return true;

                if (stopped)
                    return true;

                if (targetCharacter != null && targetCharacter.IsDead
                    || targetCharacter == null || target == null)
                    return true;

                if (parent.resignDistance > 0)
                    if (Vector2.Distance(target.transform.position, character.transform.position) > parent.resignDistance)
                        return true;

                if (parent.resignTime > 0)
                {
                    bool seeingTarget = true;
                    foreach (var item in Physics2D.LinecastAll(target.transform.position + new Vector3(0, 0.5f), character.transform.position + new Vector3(0, 0.95f, 0)))
                    {
                        if (item.collider is TilemapCollider2D || item.collider is CompositeCollider2D)
                        {
                            timeForResign -= dt;
                            seeingTarget = false;
                            if (timeForResign < 0)
                                return true;
                        }
                        else
                            continue;
                    }

                    if (seeingTarget)
                        timeForResign = parent.resignTime;
                }

                return false;
            }

            public override void Update(float dt)
            {
                if (ShouldResign(dt))
                {
                    targetCharacter = null;
                    target = null;
                    movementController.Stop();
                    pathfinding.Stop();
                    return;
                }



                timeToNextPathfinding -= dt;
                if (timeToNextPathfinding > 0)
                    return;

                timeToNextPathfinding = parent.timeBetweenRetryingFindingPath;
                if (pathfinding.Target == pathfinding.GetTileId(target.transform.position))
                    return;

                pathfinding.MoveTo(target.transform.position, true);

            }

            public override string ToString()
            {
                return "ChaseAction;" +
                    "\ntimeForResign: " + timeForResign.ToString() +
                    "\ntimeToNextPathfinding: " + timeToNextPathfinding.ToString() +
                    "\ntarget: " + target.name +
                    "\nfinished: " + IsFinished();
            }
        }
    }
}