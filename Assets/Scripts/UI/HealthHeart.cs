using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class HealthHeart : MonoBehaviour
{
    public Sprite full;
    public Sprite halfFull;
    public Sprite none;
    public Sprite damageFull;
    public Sprite damageHalfLeft;
    public Sprite damageHalfRight;

    private SpriteRenderer sprite;

    private void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();
    }

    public float SwitchTime = 0.5f;

    [HideInInspector]
    [SerializeField]
    private int _health = 2;

    public int Health
    {
        get => _health;
        set
        {
            if (coroutine != null)
                StopCoroutine(coroutine);

            coroutine = StartCoroutine("SetHealth", value);
        }
    }

    private Coroutine coroutine;

    private IEnumerator SetHealth(int value)
    {
        var health = Mathf.Clamp(value, 0, 2);
        var current = Health;
        _health = health;
        if (current == 2)
        {
            if (health == 1)
            {
                sprite.sprite = damageHalfRight;
                yield return new WaitForSeconds(SwitchTime);
                sprite.sprite = halfFull;
            }
            else if (health == 0)
            {
                sprite.sprite = damageFull;
                yield return new WaitForSeconds(SwitchTime);
                sprite.sprite = none;
            }
        }
        else if (current == 1)
        {
            if (health == 2)
            {
                sprite.sprite = damageHalfRight;
                yield return new WaitForSeconds(SwitchTime);
                sprite.sprite = full;
            }
            else if (health == 0)
            {
                sprite.sprite = damageHalfLeft;
                yield return new WaitForSeconds(SwitchTime);
                sprite.sprite = none;
            }
        }
        else if (current == 0)
        {
            if (health == 2)
            {
                sprite.sprite = damageFull;
                yield return new WaitForSeconds(SwitchTime);
                sprite.sprite = full;
            }
            else if (health == 1)
            {
                sprite.sprite = damageHalfLeft;
                yield return new WaitForSeconds(SwitchTime);
                sprite.sprite = halfFull;
            }
        }
    }
}