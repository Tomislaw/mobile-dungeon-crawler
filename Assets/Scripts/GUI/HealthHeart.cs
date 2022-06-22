using System.Collections;
using UnityEngine;

namespace RuinsRaiders.GUI
{
    // Responsible for showing single heart in HeartUi and handling its flashin animations
    [RequireComponent(typeof(SpriteRenderer))]
    public class HealthHeart : MonoBehaviour
    {
        public float SwitchTime = 0.5f;

        [SerializeField]
        private Sprite full;
        [SerializeField]
        private Sprite halfFull;
        [SerializeField]
        private Sprite none;
        [SerializeField]
        private Sprite damageFull;
        [SerializeField]
        private Sprite damageHalfLeft;
        [SerializeField]
        private Sprite damageHalfRight;


        [HideInInspector]
        [SerializeField]
        private int _health = 2;

        private SpriteRenderer sprite;


        public int Health
        {
            get => _health;
            set
            {
                StartCoroutine("SetHealth", value);
            }
        }

        private void Awake()
        {
            sprite = GetComponent<SpriteRenderer>();
        }

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
}