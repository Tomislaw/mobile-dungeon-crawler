using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace RuinsRaiders.GUI
{
    // Responsible for showing single heart in HeartUi and handling its flashin animations
    public class OxygenBubble : MonoBehaviour
    {

        [SerializeField]
        private Sprite fullSprite;

        [SerializeField]
        private Sprite halfSprite;

        [SerializeField]
        private Sprite noneSprite;


        [HideInInspector]
        [SerializeField]
        private int _oxygen = 2;

        private SpriteRenderer _sprite;
        private Image _image;

        public int Oxygen
        {
            get => _oxygen;
            set
            {
                SetOxygen(value);
            }
        }

        private void Awake()
        {
            _sprite = GetComponent<SpriteRenderer>();
            _image = GetComponent<Image>();
        }

        private void SetOxygen(int value)
        {
            var oxygen = Mathf.Clamp(value, 0, 2);
            _oxygen = oxygen;
            switch (_oxygen)
            {
                case 0:
                    if(_sprite)
                        _sprite.sprite = noneSprite;
                    if (_image)
                        _image.sprite = noneSprite;
                    break;
                case 1:
                    if (_sprite)
                        _sprite.sprite = halfSprite;
                    if (_image)
                        _image.sprite = halfSprite;
                    break;
                default:
                    if (_sprite)
                        _sprite.sprite = fullSprite;
                    if (_image)
                        _image.sprite = fullSprite;
                    break;
            }
        }
    }
}