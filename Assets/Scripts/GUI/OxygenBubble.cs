using System.Collections;
using UnityEngine;

namespace RuinsRaiders.GUI
{
    // Responsible for showing single heart in HeartUi and handling its flashin animations
    [RequireComponent(typeof(SpriteRenderer))]
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

        private SpriteRenderer sprite;

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
            sprite = GetComponent<SpriteRenderer>();
        }

        private void SetOxygen(int value)
        {
            var oxygen = Mathf.Clamp(value, 0, 2);
            _oxygen = oxygen;
            switch (_oxygen)
            {
                case 0:
                    sprite.sprite = noneSprite;
                    break;
                case 1:
                    sprite.sprite = halfSprite;
                    break;
                default:
                    sprite.sprite = fullSprite;
                    break;
            }
        }
    }
}