using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RuinsRaiders
{
    public class MeeleAttack : AttackController
    {

        public MeeleAttackTrigger attackTrigger;

        public int attackDamage;
        public float attackTime = 0.1f;

        public float knockback;
        public bool deflectProjectiles = false;

        private HealthController _healthController;
        public float _attackTimeLeft;

        private void Awake()
        {
            _healthController = GetComponent<HealthController>();
            if(attackTrigger == null)
                attackTrigger = GetComponentInChildren<MeeleAttackTrigger>();
        }

        public override void Attack()
        {
            if (CanAttack)
            {
                attackTrigger.StartAttack(attackDamage, knockback, deflectProjectiles, _healthController.group);
                _attackTimeLeft = attackTime;
                base.Attack();
            }
        }

        private void FixedUpdate()
        {
            if (_healthController.IsDead)
                _attackTimeLeft = 0;

            if (_attackTimeLeft > 0)
                _attackTimeLeft -= Time.fixedDeltaTime;

            if (_attackTimeLeft <= 0 && attackTrigger.IsAttacking())
            {
                attackTrigger.StopAttack();
                _attackTimeLeft = 0;
            }

            base.FixedUpdate();
        }

    }
}