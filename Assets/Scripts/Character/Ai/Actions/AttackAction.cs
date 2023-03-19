using UnityEngine;

namespace RuinsRaiders.AI
{
    [CreateAssetMenu(fileName = "AttackAction", menuName = "RuinsRaiders/Ai/AttackAction", order = 1)]
    public class AttackAction : BasicAiActionData
    {
        public float attackTime;
        public bool stopMoving;
        public bool canBeInterrupted;

        public override BasicAiAction Create(ActivatorData trigger)
        {
            return new Action(this, trigger);
        }

        public class Action : BasicAiAction
        {
            private float _attackTime;
            private bool _canBeInterrupted;
            private bool _stopMoving;

            private AttackController _attackController;
            private MovementController _movementController;

            private GameObject _target;
            public override bool CanStop()
            {
                return _canBeInterrupted;
            }

            public Action(AttackAction parent, ActivatorData trigger)
            {
                _attackTime = parent.attackTime;
                _canBeInterrupted = parent.canBeInterrupted;
                _stopMoving = parent.stopMoving;

                _attackController = trigger.triggeredFor.GetComponent<AttackController>();
                _movementController = trigger.triggeredFor.GetComponent<MovementController>();
                _target = trigger.triggeredBy;
            }

            public override bool IsFinished()
            {
                return _attackTime < 0 || _attackController == null;
            }

            public override void Update(float dt)
            {
                if (stopped)
                {
                    if (_attackController)
                        _attackController.chargeAttack = false;
                    return;
                }

                _attackTime -= dt;

                if (_attackController)
                    _attackController.chargeAttack = true;

                if(_movementController!=null)
                {
                    if (_target != null)
                        _movementController.FacePosition(_target.transform.position);

                    if(_stopMoving)
                        _movementController.Stop();
                }
                

                if (IsFinished())
                {
                    if (_attackController)
                        _attackController.Attack();
                }
            }
        }
    }
}
