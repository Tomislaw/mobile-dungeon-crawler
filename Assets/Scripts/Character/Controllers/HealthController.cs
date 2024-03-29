﻿using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace RuinsRaiders
{
    public class HealthController : MonoBehaviour
    {
        private static float FlashTime = 0.2f;

        public bool IsDead { get => health <= 0; }

        public Group group;

        public UnityEvent onDeath = new();
        public OnDamageEvent onDamage = new();

        [SerializeField]
        protected float immunityAfterHitTime = 0f;
        protected float _immunityAfterHitTimeLeft = 0f;
        protected int _immunityArmor = 0;

        [SerializeField]
        protected float ressurectTime = 1f;

        // health
        [Header("Health")]
        public int health = 4;
        public int maxHealth = 4;

        [Header("Shield")]
        public int shield = 0;
        public int maxShield = 0;

        public float shieldRegenerationTime = 10f;
        private float _shieldRegenerationTimeLeft;

        [Header("Armor")]
        public int armor = 0;
        public int maxArmor = 0;




        protected Character _character;
        protected AI.BasicAi _ai;
        protected Renderer _renderer;

        protected int _initialLayer = 0;

        public enum Group
        {
            Skeletons, Player, Goblins, Fish, None, Animals, Nomads, Global
        }

        protected void Start()
        {
            _character = GetComponent<Character>();
            _ai = GetComponent<AI.BasicAi>();
            _initialLayer = gameObject.layer;
            _renderer = GetComponent<Renderer>();

            if (health <= 0)
            {
                if (_character)
                    gameObject.layer = LayerMask.NameToLayer("Dead");

                if (_ai)
                    _ai.enabled = false;
            }
        }

        protected void FixedUpdate()
        {
            armor = maxArmor;


            if (shield < maxShield)
            {
                _shieldRegenerationTimeLeft -= Time.fixedDeltaTime;

                if (_shieldRegenerationTimeLeft <= 0)
                {
                    _shieldRegenerationTimeLeft = shieldRegenerationTime;
                    shield++;
                }
            }

            if (_immunityArmor > 0)
            {
                _immunityAfterHitTimeLeft -= Time.fixedDeltaTime;
                if (_immunityAfterHitTimeLeft < 0)
                    _immunityArmor = 0;
            }
        }

        public virtual void DamageIgnoreAll(int damage, GameObject who)
        {
            if (_character != null && _character.holdUpdate == true)
                return;

            if (health <= 0 && damage > 0)
                return;

            if (onDamage != null)
                onDamage.Invoke(who);

            if (_renderer != null)
                StartCoroutine(FlashColor());

            health -= damage;

            if (health <= 0)
                Death();
        }

        public virtual void Damage(int damage, GameObject who)
        {
            if (_character != null && _character.holdUpdate == true)
                return;

            if (health <= 0 && damage >= 0)
                return;

            if (onDamage != null)
                onDamage.Invoke(who);

            if (_renderer != null)
                StartCoroutine(FlashColor());

            if (damage < 0)
            {
                if (IsDead)
                {
                    Resurrect();
                    health = Mathf.Min(damage, maxHealth);
                } else
                {
                    health = Mathf.Min(health + damage, maxHealth);
                }
                return;
            }

            damage -= _immunityArmor;
            if(damage <= 0)
                return;

            _immunityArmor += damage;
            _immunityAfterHitTimeLeft = immunityAfterHitTime;

            armor -= damage;

            if (armor >= 0)
                return;

            shield += armor;
            armor = 0;

            if (shield >= 0)
                return;

            health += shield;
            shield = 0;


            if (health <= 0)
                Death();
        }

        protected void Death()
        {
              gameObject.layer = LayerMask.NameToLayer("Dead");

            if (_ai)
                _ai.enabled = false;

            if (onDeath != null)
                onDeath.Invoke();
        }

        public void Resurrect()
        {
            StartCoroutine(ResurrectCoroutine());
        }

        private IEnumerator FlashColor()
        {
            _renderer.material.SetFloat("Flash", 1);
            yield return new WaitForSeconds(FlashTime);
            _renderer.material.SetFloat("Flash", 0);
        }

        private IEnumerator ResurrectCoroutine()
        {
            if (health > 0)
                yield break;

            _character.holdUpdate = true;
            _character.SetAnimation("Resurrect");
            yield return new WaitForSeconds(ressurectTime);
            _character.SetAnimation("Idle");
            _character.holdUpdate = false;
            var ai = GetComponent<AI.BasicAi>();
            if (ai)
                ai.enabled = true;
            gameObject.layer = _initialLayer;
            health = maxHealth;

        }
    }

    public class OnDamageEvent : UnityEvent<GameObject> { }


}