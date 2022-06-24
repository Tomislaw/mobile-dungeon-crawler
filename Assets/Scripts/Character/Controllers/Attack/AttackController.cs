using UnityEngine;
using UnityEngine.Events;

namespace RuinsRaiders
{
    public class AttackController : MonoBehaviour
    {
        // attack and damage group
        public float attackSpeed = 0.2f;
        public float attackAnimationTime = 0.15f;
        public float overchargeTime = 1;
        public bool chargeAttack = false;

        public UnityEvent onAttack = new();
        public UnityEvent onChargedAttack = new();

        // private fields
        private float _timeToNextAttack = 0;
        private float _timeToAnimationAttackFinish = 0;
        private float _timeToOvercharge = 0;

        private Character _character;

        public bool CanAttack { get => _timeToNextAttack <= 0 && !_character.IsDead && !_character.holdUpdate; }
        public bool IsAttacking { get => _timeToAnimationAttackFinish > 0; }
        public bool IsOvercharged { get => overchargeTime > 0 && _timeToOvercharge <= 0; }

        public void OnEnable()
        {
            _timeToOvercharge = overchargeTime;
            _character = GetComponent<Character>();

        }
        public void FixedUpdate()
        {
            if (_timeToAnimationAttackFinish > 0) _timeToAnimationAttackFinish -= Time.deltaTime;
            if (_timeToNextAttack > 0) _timeToNextAttack -= Time.deltaTime;

            if (chargeAttack)
                _timeToOvercharge -= Time.fixedDeltaTime;
            else
                _timeToOvercharge = overchargeTime;

        }

        public virtual void Attack()
        {
            if (CanAttack)
            {
                _timeToAnimationAttackFinish = attackAnimationTime;
                _timeToNextAttack = attackSpeed;
                chargeAttack = false;

                if (IsOvercharged)
                    onChargedAttack.Invoke();
                else
                    onAttack.Invoke();
            }
        }
    }
}

