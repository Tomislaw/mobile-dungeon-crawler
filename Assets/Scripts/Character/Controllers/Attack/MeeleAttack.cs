using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeeleAttack : AttackController
{

    public int AttackDamage;
    public float Distance;
    public float Knockback;

    private HealthController healthController;
    private MovementController movementController;
    private void Awake()
    {
        healthController = GetComponent<HealthController>();
        movementController = GetComponent<MovementController>();
    }

    public override void Attack()
    {
        if (CanAttack)
        {
            FindAndDamage(AttackDamage);
            base.Attack();
        }
   
    }

    private void FindAndDamage(int damage)
    {
        var alreadyDamaged = new List<HealthController>();
        var raycast = Physics2D.OverlapCircleAll(transform.position, Distance);
        foreach (var cast in raycast)
        {
            var hitHc = cast.gameObject.GetComponent<HealthController>();

            // do not hit character from same group (avoid friendly fire)
            if (hitHc == null || hitHc.group == healthController.group || healthController.IsDead)
                continue;

            if (alreadyDamaged.Contains(hitHc))
                continue;

            // do not hit character at back
            if (movementController.FaceLeft == true 
                && hitHc.transform.position.x > transform.position.x
                || movementController.FaceLeft == false
                && hitHc.transform.position.x < transform.position.x)
                continue;

            hitHc.Damage(AttackDamage, gameObject);
            var hitBody = hitHc.GetComponent<Rigidbody2D>();
            if(hitBody)
                hitBody.AddForce(new Vector2(Mathf.Sign(hitHc.transform.position.x - transform.position.x) * Knockback, Knockback / 2));

            // avoid multiple damage for characters with multi colliders
            alreadyDamaged.Add(hitHc);
        }
    }
}