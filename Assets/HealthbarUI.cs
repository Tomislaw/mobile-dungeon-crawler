using System.Collections.Generic;
using UnityEngine;


namespace RuinsRaiders.GUI {
    public class HealthbarUI : MonoBehaviour
    {
        private static float FlashTime = 0.4f;

        public SpriteRenderer barPrefab;

        public HealthController healthController;
    
        public Sprite health;
        public Sprite shield;
        public Sprite armor;
        public Sprite hit;
        public Sprite empty;

        public float offset;

        public int maxInRow = 4;

        protected int _previousHealth = 0;
        protected float _flashTimeLeft = 0;

        protected readonly List<SpriteRenderer> _hearts = new();
        protected readonly List<SpriteRenderer> _armors = new();
        protected readonly List<SpriteRenderer> _shields = new();


        private int _shieldCount;
        private int _maxShieldCount;

        private int _armorCount;
        private int _maxArmorCount;

        private int _healthCount;
        private int _maxHealthCount;

        private int _barSize;

        void OnEnable()
        {
            Invalidate();
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            if (healthController == null)
                return;

            if (_maxHealthCount != healthController.maxHealth
                || _maxArmorCount != healthController.maxArmor
                || _maxShieldCount != healthController.maxShield)
                Invalidate();


            if (_healthCount != healthController.health)
            {
                SetHitSprite(_healthCount - healthController.health, healthController.health, _hearts);
                _healthCount = healthController.health;
            }
            if (_shieldCount != healthController.shield)
            {
                SetHitSprite(_shieldCount - healthController.shield, healthController.shield, _shields);
                _shieldCount = healthController.shield;
            }
            if (_armorCount != healthController.armor)
            {
                SetHitSprite(_armorCount - healthController.armor, healthController.armor, _armors);
                _armorCount = healthController.armor;
            }

            if (_flashTimeLeft > 0)
                _flashTimeLeft -= Time.fixedDeltaTime;


            if (_flashTimeLeft < 0)
            {
                _flashTimeLeft = 0;

                if (healthController.IsDead)
                {
                    _armors.ForEach(armor => armor.sprite = null);
                    _shields.ForEach(shield => shield.sprite = null);
                    _hearts.ForEach(health => health.sprite = null);
                }
                else
                {
                    SetSprite(_armorCount, _armors, armor);
                    SetSprite(_shieldCount, _shields, shield);
                    SetSprite(_healthCount, _hearts, health);
                }
            }

        }

        private void SetHitSprite(int damage, int left, List<SpriteRenderer> sprites)
        {
            _flashTimeLeft = FlashTime;
            for (int i = 1; i <= sprites.Count; i++)
            {
                 if ( i <= left + damage && i > left)
                    sprites[sprites.Count - i].sprite = hit;
            }
        }

        private void SetSprite(int left, List<SpriteRenderer> sprites, Sprite defaultSprite)
        {
            for (int i = 1; i <= sprites.Count; i++)
            {
                if (i <= left)
                    sprites[sprites.Count - i].sprite = defaultSprite;
                else
                    sprites[sprites.Count - i].sprite = empty;
            }
        }

        private void Invalidate()
        {
            if(healthController == null)
                return;

            foreach (var heart in _hearts)
                Destroy(heart.gameObject);

            foreach (var armor in _armors)
                Destroy(armor.gameObject);

            foreach (var shield in _shields)
                Destroy(shield.gameObject);

            _hearts.Clear();
            _shields.Clear();
            _armors.Clear();

            _shieldCount = healthController.shield;
            _maxShieldCount = healthController.maxShield;

            _armorCount = healthController.armor;
            _maxArmorCount = healthController.maxArmor;

            _healthCount = healthController.health;
            _maxHealthCount = healthController.maxHealth;

            _barSize = _maxShieldCount + _maxArmorCount + _maxHealthCount;


            for (int i = 0; i < _barSize; i++)
            {
                int row = i / maxInRow;
                int iconsInRow = Mathf.Min(maxInRow, _barSize - row * maxInRow);

                int column = i % maxInRow;
                float posX = (-iconsInRow / 2f + column + 0.5f) * offset;
                float posY = -row * offset;
                var prefab = Instantiate(barPrefab);
                prefab.transform.SetParent(transform, false);
                prefab.transform.localPosition = new Vector2(posX, posY);

                if (i < _maxArmorCount)
                {
                    prefab.name = "armor" + i;
                    prefab.sprite = armor;
                    _armors.Add(prefab);
                } else if (i < _maxArmorCount + _maxShieldCount)
                {
                    prefab.name = "shield" + i;
                    prefab.sprite = shield;
                    _shields.Add(prefab);
                }
                else if (i < _maxArmorCount + _maxShieldCount + _maxHealthCount)
                {
                    prefab.name = "health" + i;
                    prefab.sprite = health;
                    _hearts.Add(prefab);
                }

            }
        }

        private void OnDamage()
        {

        }
    }
}