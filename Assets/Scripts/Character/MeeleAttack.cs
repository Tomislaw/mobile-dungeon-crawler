using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeeleAttack : MonoBehaviour, OnAttack
{
    public int AttackDamage;
    public float Distance;
    public float Knockback;

    private Character character;

    bool OnAttack.ChargeAttack { get => false; set { } }

    private void Awake()
    {
        character = GetComponent<Character>();
        if (character)
            character.AttackController = this;
    }

    public void Attack()
    {
        FindAndDamage(AttackDamage);
    }

    private void FindAndDamage(int damage)
    {
        var raycast = Physics2D.OverlapCircleAll(transform.position, Distance);
        foreach (var cast in raycast)
        {
            var ch = cast.gameObject.GetComponent<Character>();
            if (ch == null || ch.group == character.group || ch.IsDead)
                continue;

            if (character.FaceLeft && ch.transform.position.x > transform.position.x
                || !character.FaceLeft && ch.transform.position.x < transform.position.x)
                continue;

            ch.DamageController.Damage(AttackDamage, gameObject);
            var rg = ch.GetComponent<Rigidbody2D>();
            if (rg)
                rg.AddForce(new Vector2(Mathf.Sign(ch.transform.position.x - transform.position.x) * Knockback, Knockback / 2));
        }
    }
}