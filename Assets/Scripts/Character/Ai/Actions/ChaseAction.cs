using Assets.Scripts.Character.Ai;
using UnityEngine;

[CreateAssetMenu(fileName = "ChaseTargetAction", menuName = "RuinsRaiders/Ai/ChaseTargetAction", order = 1)]
public class ChaseAction : BasicAiActionData
{
    public float resignDistance = -1;
    public float timeBetweenRetryingFindingPath = 0.5f;

    public override BasicAiAction Create(ActivatorData trigger)
    {
        return new Action(this, trigger);
    }

    public class Action : BasicAiAction
    {
        internal float timeToNextPathfinding = 0;
        internal GameObject target;
        internal Character targetCharacter;
        internal Character character;
        internal PathfindingController pathfinding;
        internal ChaseAction parent;

        public Action(ChaseAction parent, ActivatorData trigger )
        {
            character = trigger.triggeredFor.GetComponent<Character>();
            pathfinding = trigger.triggeredFor.GetComponent<PathfindingController>();
            target = trigger.triggeredBy;
            targetCharacter = target.GetComponent<Character>();
            this.parent = parent;
        }

        internal virtual bool TryWalkingToTarget()
        {
            if (target == null)
                return false;
            if (!pathfinding.CanWalkOnPosition(target.transform.position))
                return false;

            return pathfinding.MoveTo(target.transform.position);
        }

        public override bool CanStop()
        {
            return IsFinished() || character.IsGrounded;
        }
        public override bool IsFinished()
        {
            return target == null;
        }


        public override void Update(float dt)
        {
            if (IsFinished())
                return;

            if (targetCharacter != null && targetCharacter.IsDead) {
                targetCharacter = null;
                target = null;
                return;
            }

            timeToNextPathfinding -= dt;
            if (timeToNextPathfinding > 0)
                return;

            timeToNextPathfinding = parent.timeBetweenRetryingFindingPath;
            if (pathfinding.Target == pathfinding.GetTileId(target.transform.position))
                return;

            TryWalkingToTarget();

        }
    }

}
