using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;


namespace RuinsRaiders
{
    public class Projectile : MonoBehaviour
    {
        public Character launcher = null;

        public UnityEvent onSpawn;
        public UnityEvent onHit;

        public Vector2 initialVelocity;

        [SerializeField]
        private int damage = 0;

        [SerializeField]
        private bool rotating = false;

        [SerializeField]
        private float destroyTime = 1;
        [SerializeField]
        private float lifeTime = 2;

        [SerializeField]
        private float explosionRadius = 1;
        [SerializeField]
        private float explosionForce = 1;

        [SerializeField]
        private List<HealthController.Group> HitFilter = new();


        [SerializeField]
        private Vector2 deceleration;
        [SerializeField]
        private float gravity = 0;


        private Rigidbody2D _rigidbody;
        private Collider2D _collider;

        private bool _hit = false;

        void Start()
        {
            _rigidbody = GetComponent<Rigidbody2D>();
            _rigidbody.velocity = initialVelocity;
            _collider = GetComponent<Collider2D>();
            onSpawn.Invoke();
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            lifeTime -= Time.fixedDeltaTime;
            if (lifeTime <= 0)
            {
                if (_hit == false)
                {
                    _hit = true;
                    OnProjectileDeath();
                    Stop();
                }

                lifeTime = 0;
                destroyTime -= Time.fixedDeltaTime;
                if (destroyTime <= 0)
                    Destroy(gameObject);

                return;
            }

            _rigidbody.velocity -= new Vector2(0, gravity) * Time.fixedDeltaTime;
            var deceleration = this.deceleration * Time.fixedDeltaTime;
            if (Mathf.Abs(_rigidbody.velocity.x) <= deceleration.x)
                _rigidbody.velocity = new Vector2(0, _rigidbody.velocity.y);
            else
                _rigidbody.velocity -= Mathf.Sign(_rigidbody.velocity.x) * new Vector2(deceleration.x, 0);

            if (Mathf.Abs(_rigidbody.velocity.y) <= deceleration.y)
                _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, 0);
            else
                _rigidbody.velocity -= Mathf.Sign(_rigidbody.velocity.y) * new Vector2(0, deceleration.y);

            if (rotating)
            {
                float angle = Mathf.Atan2(_rigidbody.velocity.y, _rigidbody.velocity.x) * Mathf.Rad2Deg;
                transform.eulerAngles = new Vector3(0, 0, angle - angle % 15);
            }
        }

        private void ResolveHit(HealthController target)
        {

            if (!HitFilter.Contains(target.group))
                return;
            else
            {
                target.Damage(damage, gameObject);
                if (explosionForce > 0)
                {
                    var rg = target.GetComponent<Rigidbody2D>();
                    if (rg)
                    {
                        var force = transform.position - new Vector3(rg.worldCenterOfMass.x, rg.worldCenterOfMass.y, 0);
                        force.Normalize();
                        rg.AddForce(force * -explosionForce);
                    }

                }
            }

        }
        private void OnTriggerEnter2D(Collider2D collision)
        {

            if (_hit)
                return;

            if (collision.isTrigger)
                return;

            var hitItems = explosionRadius > 0
                ? Physics2D.OverlapCircleAll(transform.position, explosionRadius).Select(it => it.gameObject).Distinct()
                : new List<GameObject>() { collision.gameObject };

            ResolveColliders(hitItems);
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {

            if (_hit)
                return;

            transform.position = collision.contacts.First().point;

            var hitItems = explosionRadius > 0
                ? Physics2D.OverlapCircleAll(transform.position, explosionRadius).Select(it => it.gameObject).Distinct()
                : new List<GameObject>() { collision.gameObject };

            ResolveColliders(hitItems);
        }

        private void ResolveColliders(IEnumerable<GameObject> hitItems)
        {
            foreach (var hit in hitItems)
            {
                var target = hit.GetComponent<HealthController>();
                if (target == null)
                {
                    lifeTime = 0;
                    continue;
                }
                if (target.IsDead)
                    continue;

                lifeTime = 0;
                ResolveHit(target);
            }


            if (lifeTime <= 0)
            {
                Stop();
                OnProjectileDeath();
            }
        }

        private void Stop()
        {
            _collider.enabled = false;
            _rigidbody.isKinematic = false;
            _rigidbody.simulated = false;
            _hit = true;
        }

        private void OnProjectileDeath()
        {
            onHit.Invoke();
        }
    }
}