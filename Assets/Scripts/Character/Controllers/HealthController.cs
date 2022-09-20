using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace RuinsRaiders
{
    public class HealthController : MonoBehaviour
    {
        public bool IsDead { get => health <= 0; }

        public Group group;

        public UnityEvent onDeath = new();
        public OnDamageEvent onDamage = new();

        // health
        public int health = 4;
        public int maxHealth = 4;

        [SerializeField]
        private float ressurectTime = 1f;

        private Character _character;
        private AI.BasicAi _ai;

        private int _initialLayer = 0;

        public enum Group
        {
            Skeletons, Player, Goblins, Fish, None
        }

        private void Start()
        {
            _character = GetComponent<Character>();
            _ai = GetComponent<AI.BasicAi>();
            _initialLayer = gameObject.layer;
            if (health <= 0)
            {
                if (_character)
                    gameObject.layer = LayerMask.NameToLayer("Dead");

                if (_ai)
                    _ai.enabled = false;
            }
        }

        public void Damage(int damage, GameObject who)
        {
            if (_character != null && _character.holdUpdate == true)
                return;

            if (health <= 0 && damage > 0)
                return;

            if (onDamage != null)
                onDamage.Invoke(who);

            health -= damage;
            if (health <= 0)
                Death();
        }

        private void Death()
        {
            if (_character)
                gameObject.layer = LayerMask.NameToLayer("Dead");

            if (_ai)
            {
                _ai.enabled = false;
            }


            if (onDeath != null)
                onDeath.Invoke();
        }

        public void Resurrect()
        {
            StartCoroutine(ResurrectCoroutine());
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