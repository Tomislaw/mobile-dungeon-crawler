using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class HealthController : MonoBehaviour
{
    // health
    public int Health = 4;
    public int MaxHealth = 4;
    public float RessurectTime = 1f;

    private Character character;
    private BasicAi ai;
    public bool IsDead { get => Health <= 0; }
    public UnityEvent OnDeath = new UnityEvent();
    public OnDamageEvent OnDamage = new OnDamageEvent();

    public Group group;
    public enum Group
    {
        Skeletons, Player, Goblins, None
    }

    private void OnEnable()
    {
        character = GetComponent<Character>();
        ai = GetComponent<BasicAi>();
    }

    public void Damage(int damage, GameObject who)
    {
        if (character?.holdUpdate == true)
            return;

        if (Health <= 0 && damage > 0)
            return;

        if (OnDamage != null)
            OnDamage.Invoke(who);

        Health -= damage;
        if (Health <= 0)
        {
            if (character)
                character.gameObject.layer = LayerMask.NameToLayer("Dead");

            if (ai)
                ai.enabled = false;

            if (OnDeath != null)
                OnDeath.Invoke();
        }
    }

    public void Resurrect()
    {
        StartCoroutine(ResurrectCoroutine());
    }

    private IEnumerator ResurrectCoroutine()
    {
        character.holdUpdate = true;
        character.SetAnimation("Resurrect");
        yield return new WaitForSeconds(RessurectTime);
        character.SetAnimation("Idle");
        character.holdUpdate = false;
        var ai = GetComponent<BasicAi>();
        if (ai)
            ai.enabled = true;
    }
}

public class OnDamageEvent : UnityEvent<GameObject>{}


