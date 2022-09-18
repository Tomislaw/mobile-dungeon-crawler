using UnityEngine;

namespace RuinsRaiders.AI
{
    [CreateAssetMenu(fileName = "ChaseAndAttackTargetAction", menuName = "RuinsRaiders/Ai/ChaseAndAttackTargetAction", order = 1)]
    public class ChaseAndAttackAction : ChaseAction
    {
        [SerializeField]
        private Vector2 distanceForAttack;

        [SerializeField]
        private float attackTime;
        [SerializeField]
        private float timeBetweenAttack;

        public override BasicAiAction Create(ActivatorData trigger)
        {
            return new Action(this, trigger);
        }

        new public class Action : ChaseAction.Action
        {
            readonly new ChaseAndAttackAction parent;
            private float _timeToAttack;
            private float _attackCooldown;
            private bool _attacking = false;

            readonly private AttackController _attackController;

            public Action(ChaseAndAttackAction parent, ActivatorData trigger) : base(parent, trigger)
            {
                _attackController = trigger.triggeredFor.GetComponent<AttackController>();
                this.parent = parent;
            }

            public override void Update(float dt)
            {
                if (ShouldResign(dt))
                {
                    targetCharacter = null;
                    target = null;
                    _attackController.chargeAttack = false;
                    movementController.Stop();
                    pathfinding.Stop();
                    return;
                }

                if (_attackCooldown > 0)
                {
                    _attackCooldown -= dt;
                    return;
                }


                if (_attacking)
                {
                    if (_timeToAttack == parent.attackTime)
                    {
                        movementController.Stop();
                        pathfinding.Stop();
                        movementController.FacePosition(target.transform.position);
                    }
                    if (_timeToAttack < 0)
                    {
                        _attackController.Attack();
                        _attackController.chargeAttack = false;
                        _attacking = false;
                        _attackCooldown = parent.timeBetweenAttack;
                    }
                    else
                    {
                        _attackController.chargeAttack = true;
                        _timeToAttack -= dt;
                    }
                    return;
                }
                var distance = character.transform.position - target.transform.position;
                distance = new Vector2(Mathf.Abs(distance.x), Mathf.Abs(distance.y));

                if (parent.distanceForAttack.x > distance.x 
                    && parent.distanceForAttack.y > distance.y
                    && (movementController.IsGrounded || movementController.IsSwimming || movementController.flying || !movementController.canUsePlatform)
                    )
                {
                    _attacking = true;
                    _timeToAttack = parent.attackTime;
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
}