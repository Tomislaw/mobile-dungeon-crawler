using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RuinsRaiders
{
    public class MeeleAttackTrigger : MonoBehaviour
    {
        private List<GameObject> hits = new List<GameObject>();

        private int _damage = 0;
        private float _knockback = 0;
        private bool _deflectProjectiles;
        private HealthController.Group _group;

        private Collider2D _collider;

        public void StartAttack(int damage, float knockback, bool deflectProjectiles, HealthController.Group group)
        {
            _collider.enabled = true;
            hits.Clear();
            _damage = damage;
            _deflectProjectiles = deflectProjectiles;
            _group = group;
            _knockback = knockback;
        }

        public void StopAttack()
        {
            _collider.enabled = false;
        }

        public bool IsAttacking()
        {
            return _collider.enabled;
        }


        void Start()
        {
            _collider = GetComponent<Collider2D>();
            StopAttack();
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (hits.Contains(collision.gameObject))
                return;

            if (_deflectProjectiles && DeflectIfPossibe(collision.gameObject))
            {
                hits.Add(collision.gameObject);
                return;
            }

            DamageIfPossible(collision.gameObject);
            hits.Add(collision.gameObject);
        }

        private void DamageIfPossible(GameObject hit)
        {
            var target = hit.GetComponent<HealthController>();

            if (target == null || target.group == _group)
                return;

            target.Damage(_damage, gameObject);
            var hitBody = target.GetComponent<Rigidbody2D>();
            if (hitBody)
                hitBody.AddForce(new Vector2(Mathf.Sign(target.transform.position.x - transform.position.x) * _knockback, _knockback / 2));

        }

        private bool DeflectIfPossibe(GameObject projectile)
        {

            Projectile target = projectile.GetComponent<Projectile>();

            if (target == null || target.group == _group)
                return false;

            var rigidbody2d = target.GetComponent<Rigidbody2D>();
            if (rigidbody2d == null)
                return false;

            target.group = _group;
            target.ResetLifeTime();

            projectile.layer = gameObject.layer;

            rigidbody2d.velocity = -rigidbody2d.velocity;
            return true;
        }

    }
}
