using System;
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

        public int damage = 0;

        [SerializeField]
        public HealthController.Group group;

        [NonSerialized]
        public List<HealthController> hitTargets = new List<HealthController>();

        [Header("Flags")]
        [SerializeField]
        private bool rotating = false;
        [SerializeField]
        private bool destroyOnHit = true;

        [Header("Timers")]
        [SerializeField]
        private float destroyTime = 1;
        [SerializeField]
        private float lifeTime = 2;

        [Header("Forces")]
        [SerializeField]
        private float explosionRadius = 1;
        [SerializeField]
        private float explosionForce = 1;




        [SerializeField]
        private Vector2 deceleration;
        [SerializeField]
        private float gravity = 0;


        private Rigidbody2D _rigidbody;
        private Collider2D _collider;

        private bool _hit = false;
        private float _lifeTimeLeft = 2;

        void Awake()
        {
            _rigidbody = GetComponent<Rigidbody2D>();
            _collider = GetComponent<Collider2D>();
            _lifeTimeLeft = lifeTime;
        }

        private void Start()
        {
            _rigidbody.velocity = initialVelocity;
            onSpawn.Invoke();
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            _lifeTimeLeft -= Time.fixedDeltaTime;
            if (_lifeTimeLeft <= 0)
            {
                if (_hit == false)
                {
                    _hit = true;
                    OnProjectileDeath();
                    Stop();
                }

                _lifeTimeLeft = 0;
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


        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (_hit)
                return;

            if (collision.isTrigger)
                return;

            var healthController = collision.GetComponent<HealthController>();
            if (healthController == null)
                _lifeTimeLeft = 0;
            else if (group == healthController.group)
                return;

            if (destroyOnHit)
            {
                _lifeTimeLeft = 0;
                transform.position = collision.ClosestPoint(transform.position);
            }

            ResolveHit(collision.gameObject);
        }


        private void ResolveHit(GameObject hit)
        {

            var target = hit.GetComponent<HealthController>();

            if (target == null)
                _lifeTimeLeft = 0;
            if (target == null || hitTargets.Contains(target) || target.IsDead)
                return;

            hitTargets.Add(target);

            if (group == target.group)
                return;

            target.Damage(damage, gameObject);


            if (explosionForce <= 0)
                return;
            var rg = target.GetComponent<Rigidbody2D>();
            if (rg)
            {

                var force = _lifeTimeLeft <= 0 ? new Vector3(rg.worldCenterOfMass.x, rg.worldCenterOfMass.y, 0) - transform.position
                                               : new Vector3(_rigidbody.velocity.x, _rigidbody.velocity.y + _rigidbody.velocity.x/10f, 0);
                force.Normalize();
                rg.AddForce(force * explosionForce);
            }

        }

        private void Stop()
        {
            _collider.enabled = false;
            _rigidbody.isKinematic = false;
            _rigidbody.simulated = false;
            _hit = true;
        }

        public void ResetLifeTime()
        {
            if (_lifeTimeLeft > 0)
                _lifeTimeLeft = lifeTime;
        }

        private void OnProjectileDeath()
        {
            onHit.Invoke();
            if (explosionRadius > 0)
            {
                var hitItems = Physics2D.OverlapCircleAll(transform.position, explosionRadius).Select(it => it.gameObject).Distinct();
                foreach (var item in hitItems)
                    ResolveHit(item);
            }
        }
    }
}