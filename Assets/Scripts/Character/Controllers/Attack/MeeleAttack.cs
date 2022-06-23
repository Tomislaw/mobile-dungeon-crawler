using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RuinsRaiders
{
    public class MeeleAttack : AttackController
    {

        public int attackDamage;
        public float distance;
        public float knockback;

        private HealthController _healthController;
        private MovementController _movementController;
        private void Awake()
        {
            _healthController = GetComponent<HealthController>();
            _movementController = GetComponent<MovementController>();
        }

        public override void Attack()
        {
            if (CanAttack)
            {
                FindAndDamage();
                base.Attack();
            }

        }

        private void FindAndDamage()
        {
            var alreadyDamaged = new List<HealthController>();
            var raycast = Physics2D.OverlapCircleAll(transform.position, distance);
            foreach (var cast in raycast)
            {
                var hitHc = cast.gameObject.GetComponent<HealthController>();

                // do not hit character from same group (avoid friendly fire)
                if (hitHc == null || hitHc.group == _healthController.group || _healthController.IsDead)
                    continue;

                if (alreadyDamaged.Contains(hitHc))
                    continue;

                // do not hit character at back
                if (_movementController.faceLeft == true
                    && hitHc.transform.position.x > transform.position.x
                    || _movementController.faceLeft == false
                    && hitHc.transform.position.x < transform.position.x)
                    continue;

                hitHc.Damage(attackDamage, gameObject);
                var hitBody = hitHc.GetComponent<Rigidbody2D>();
                if (hitBody)
                    hitBody.AddForce(new Vector2(Mathf.Sign(hitHc.transform.position.x - transform.position.x) * knockback, knockback / 2));

                // avoid multiple damage for characters with multi colliders
                alreadyDamaged.Add(hitHc);
            }
        }
    }
}