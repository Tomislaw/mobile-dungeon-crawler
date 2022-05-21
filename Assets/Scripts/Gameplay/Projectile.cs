using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class Projectile : MonoBehaviour
{
    public int Damage = 0;

    public bool Rotating = false;

    public float DestroyTime = 1;
    public float LifeTime = 2;

    public float ExplosionRadius = 1;
    public float ExplosionForce = 1;

    public List<HealthController.Group> HitFilter;

    public Vector2 InitialVelocity;
    public Vector2 Deceleration;
    public float Gravity = 0;

    public UnityEvent OnSpawn;
    public UnityEvent OnHit;

    public Character Launcher = null;


    private Rigidbody2D rigidbody;
    private Collider2D collider;

    private bool _hit = false;

    void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        rigidbody.velocity = InitialVelocity;
        collider = GetComponent<Collider2D>();
        OnSpawn.Invoke();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        LifeTime -= Time.fixedDeltaTime;
        if (LifeTime <= 0)
        {
            if (_hit == false)
            {
                _hit = true;
                OnProjectileDeath();
                Stop();
            }

            LifeTime = 0;
            DestroyTime -= Time.fixedDeltaTime;
            if (DestroyTime <= 0)
                Destroy(gameObject);

            return;
        }

        rigidbody.velocity -= new Vector2(0, Gravity) * Time.fixedDeltaTime;
        var deceleration = Deceleration * Time.fixedDeltaTime;
        if (Mathf.Abs(rigidbody.velocity.x) <= deceleration.x)
            rigidbody.velocity = new Vector2(0, rigidbody.velocity.y);
        else
            rigidbody.velocity -= Mathf.Sign(rigidbody.velocity.x) * new Vector2(deceleration.x, 0);

        if (Mathf.Abs(rigidbody.velocity.y) <= deceleration.y)
            rigidbody.velocity = new Vector2(rigidbody.velocity.x, 0);
        else
            rigidbody.velocity -= Mathf.Sign(rigidbody.velocity.y) * new Vector2(0, deceleration.y);

        if (Rotating)
        {
            float angle = Mathf.Atan2(rigidbody.velocity.y, rigidbody.velocity.x) * Mathf.Rad2Deg;
            transform.eulerAngles = new Vector3(0, 0, angle - angle % 15);
        }
    }

    private void ResolveHit(HealthController target)
    {

        if (!HitFilter.Contains(target.group))
            return;
        else
        {
            target.Damage(Damage, gameObject);
            if (ExplosionForce > 0)
            {
                var rg = target.GetComponent<Rigidbody2D>();
                if (rg)
                {
                    var force = transform.position - new Vector3(rg.worldCenterOfMass.x, rg.worldCenterOfMass.y, 0);
                    force.Normalize();
                    rg.AddForce(force * -ExplosionForce);
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

        var hitItems = ExplosionRadius > 0
            ? Physics2D.OverlapCircleAll(transform.position, ExplosionRadius).Select(it => it.gameObject).Distinct()
            : new List<GameObject>() { collision.gameObject };

        ResolveColliders(hitItems);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {

        if (_hit)
            return;

        transform.position = collision.contacts.First().point;

        var hitItems = ExplosionRadius > 0
            ? Physics2D.OverlapCircleAll(transform.position, ExplosionRadius).Select(it => it.gameObject).Distinct()
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
                LifeTime = 0;
                continue;
            }
            if (target.IsDead)
                continue;

            LifeTime = 0;
            ResolveHit(target);
        }


        if (LifeTime <= 0)
        {
            Stop();
            OnProjectileDeath();
        }
    }

    private void Stop()
    {
        collider.enabled = false;
        rigidbody.isKinematic = false;
        rigidbody.simulated = false;
        _hit = true;
    }

    private void OnProjectileDeath()
    {
        OnHit.Invoke();
    }
}
