using Assets.Scripts.Character.Ai;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "ChaseTargetAction", menuName = "RuinsRaiders/Ai/ChaseTargetAction", order = 1)]
public class ChaseAction : BasicAiActionData
{
    public float resignDistance = -1;
    public float resignTime = 1;
    public float timeBetweenRetryingFindingPath = 0.5f;

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

        private CancellationTokenSource token = new CancellationTokenSource();
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

        //internal virtual IEnumerator<bool?> TryWalkingToTarget(CancellationToken ct = default)
        //{
        //    movementController.Stop();

        //    if (target == null)
        //    {
        //        yield return false;
        //        yield break;
        //    }

        //    IEnumerator<bool?> pathfindingProgress;
        //    if (pathfinding.CanWalkOnPosition(target.transform.position))
        //    {
        //        pathfindingProgress = pathfinding.MoveToAsync(target.transform.position, token.Token);
        //        while (pathfindingProgress.MoveNext())
        //            if (!pathfindingProgress.Current.HasValue)
        //                yield return null;
        //            else
        //            {
        //                if (pathfindingProgress.Current.Value == true)
        //                {
        //                    yield return true;
        //                    yield break;
        //                }
        //            }
        //    }

        //    var tile = pathfinding.GetTileId(target.transform.position);
        //    for (int x = -1; x <= 1; x++) {
        //        for (int y = 1; y >= -6; y--)
        //        {
        //            if (x == 0 && y == 0)
        //                continue;

        //            if (!pathfinding.CanWalkOnTile(tile + new Vector2Int(x, y)))
        //                continue;

        //            pathfindingProgress = pathfinding.MoveToAsync(target.transform.position, token.Token);
        //            while (pathfindingProgress.MoveNext())
        //                if (!pathfindingProgress.Current.HasValue)
        //                    yield return null;
        //                else
        //                {
        //                    if (pathfindingProgress.Current.Value == true)
        //                    {
        //                        yield return true;
        //                        yield break;
        //                    }
        //                }

        //        }
        //    }
        //    yield return false;
        //    yield break;
        //}

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
            token.Cancel();
            base.Stop();
        }

        protected bool shouldResign(float dt)
        {
            if (IsFinished())
                return true;

            if (stopped)
                return true;

            if (targetCharacter != null && targetCharacter.IsDead
                || targetCharacter == null || target == null)
                return true;

            if(parent.resignDistance > 0)
                if(Vector2.Distance(target.transform.position, character.transform.position) > parent.resignDistance)
                    return true;

            if (parent.resignTime > 0)
            {
                bool seeingTarget = true;
                foreach (var item in Physics2D.LinecastAll(target.transform.position + new Vector3(0,0.5f), character.transform.position))
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
            if (shouldResign(dt))
            {
                targetCharacter = null;
                target = null;
                movementController.Stop();
                pathfinding.StopMoving();
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
    }

}
