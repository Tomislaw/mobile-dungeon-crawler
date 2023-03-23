using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace RuinsRaiders.GUI
{
    // Responsible for showing single heart in HeartUi and handling its flashin animations
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

        private SpriteRenderer _sprite;
        private Image _image;

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
            _sprite = GetComponent<SpriteRenderer>();
            _image = GetComponent<Image>();
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
                    if(_sprite)
                        _sprite.sprite = damageHalfRightSprite;
                    if (_image)
                        _image.sprite = damageHalfRightSprite;

                    yield return new WaitForSeconds(SwitchTime);

                    if (_sprite)
                        _sprite.sprite = halfFullSprite;
                    if (_image)
                        _image.sprite = halfFullSprite;
                }
                else if (health == 0)
                {
                    if (_sprite)
                        _sprite.sprite = damageFullSprite;
                    if (_image)
                        _image.sprite = damageFullSprite;

                    yield return new WaitForSeconds(SwitchTime);

                    if (_sprite)
                        _sprite.sprite = noneSprite;
                    if (_image)
                        _image.sprite = noneSprite;
                }
            }
            else if (current == 1)
            {
                if (health == 2)
                {
                    if (_sprite)
                        _sprite.sprite = damageHalfRightSprite;
                    if (_image)
                        _image.sprite = damageHalfRightSprite;

                    yield return new WaitForSeconds(SwitchTime);

                    if (_sprite)
                        _sprite.sprite = fullSprite;
                    if (_image)
                        _image.sprite = fullSprite;
                }
                else if (health == 0)
                {
                    if (_sprite)
                        _sprite.sprite = damageHalfLeftSprite;
                    if (_image)
                        _image.sprite = damageHalfLeftSprite;

                    yield return new WaitForSeconds(SwitchTime);

                    if (_sprite)
                        _sprite.sprite = noneSprite;
                    if (_image)
                        _image.sprite = noneSprite;

                }
            }
            else if (current == 0)
            {
                if (health == 2)
                {
                    if (_sprite)
                        _sprite.sprite = damageFullSprite;
                    if (_image)
                        _image.sprite = damageFullSprite;

                    yield return new WaitForSeconds(SwitchTime);

                    if (_sprite)
                        _sprite.sprite = fullSprite;
                    if (_image)
                        _image.sprite = fullSprite;
                }
                else if (health == 1)
                {
                    if (_sprite)
                        _sprite.sprite = damageHalfLeftSprite;
                    if (_image)
                        _image.sprite = damageHalfLeftSprite;

                    yield return new WaitForSeconds(SwitchTime);

                    if (_sprite)
                        _sprite.sprite = halfFullSprite;
                    if (_image)
                        _image.sprite = halfFullSprite;
                }
            }
        }
    }
}