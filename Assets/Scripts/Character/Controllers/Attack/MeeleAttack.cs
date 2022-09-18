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

        public bool deflectProjectiles = false;

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

        private void DeflectIfPossibe(Projectile target)
        {
            // can only deflect enemy projectiles
            if (target.group == _healthController.group)
                return;

            target.group = _healthController.group;

            var rigidbody2d = target.GetComponent<Rigidbody2D>();
            if (rigidbody2d == null)
                return;

            rigidbody2d.velocity = -rigidbody2d.velocity;
        }

        private void DamageIfPossible(HealthController target)
        {
            // do not hit character from same group (avoid friendly fire)
            if (target.group == _healthController.group || _healthController.IsDead)
                return;

            // do not hit character at back
            if (_movementController.FaceLeft == true
                && target.transform.position.x > transform.position.x
                || _movementController.FaceLeft == false
                && target.transform.position.x < transform.position.x)
                return;

            target.Damage(attackDamage, gameObject);
            var hitBody = target.GetComponent<Rigidbody2D>();
            if (hitBody)
                hitBody.AddForce(new Vector2(Mathf.Sign(target.transform.position.x - transform.position.x) * knockback, knockback / 2));

        }

        private void FindAndDamage()
        {
            var alreadyDamaged = new List<GameObject>();
            var raycast = Physics2D.OverlapCircleAll(transform.position, distance);
            foreach (var cast in raycast)
            {

                // avoid multiple damage for characters with multiple colliders
                if (alreadyDamaged.Contains(cast.gameObject))
                    continue;

                var hitHc = cast.gameObject.GetComponent<HealthController>();
                if(hitHc)
                    DamageIfPossible(hitHc);

                var projectile = cast.gameObject.GetComponent<Projectile>();
                if (projectile)
                    DeflectIfPossibe(projectile);

                alreadyDamaged.Add(cast.gameObject);
            }
        }
    }
}