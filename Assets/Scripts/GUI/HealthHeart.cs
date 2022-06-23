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
        private Sprite fullSprite;

        [SerializeField]
        private Sprite halfFullSprite;

        [SerializeField]
        private Sprite noneSprite;

        [SerializeField]
        private Sprite damageFullSprite;

        [SerializeField]
        private Sprite damageHalfLeftSprite;

        [SerializeField]
        private Sprite damageHalfRightSprite;


        [HideInInspector]
        [SerializeField]
        private int _health = 2;

        private SpriteRenderer sprite;

        public int Health
        {
            get => _health;
            set
            {
                StartCoroutine(SetHealth(value));
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
                    sprite.sprite = damageHalfRightSprite;
                    yield return new WaitForSeconds(SwitchTime);
                    sprite.sprite = halfFullSprite;
                }
                else if (health == 0)
                {
                    sprite.sprite = damageFullSprite;
                    yield return new WaitForSeconds(SwitchTime);
                    sprite.sprite = noneSprite;
                }
            }
            else if (current == 1)
            {
                if (health == 2)
                {
                    sprite.sprite = damageHalfRightSprite;
                    yield return new WaitForSeconds(SwitchTime);
                    sprite.sprite = fullSprite;
                }
                else if (health == 0)
                {
                    sprite.sprite = damageHalfLeftSprite;
                    yield return new WaitForSeconds(SwitchTime);
                    sprite.sprite = noneSprite;
                }
            }
            else if (current == 0)
            {
                if (health == 2)
                {
                    sprite.sprite = damageFullSprite;
                    yield return new WaitForSeconds(SwitchTime);
                    sprite.sprite = fullSprite;
                }
                else if (health == 1)
                {
                    sprite.sprite = damageHalfLeftSprite;
                    yield return new WaitForSeconds(SwitchTime);
                    sprite.sprite = halfFullSprite;
                }
            }
        }
    }
}