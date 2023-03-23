using System.Collections.Generic;
using UnityEngine;

namespace RuinsRaiders.GUI
{
    // Responsible for showinh hearts above player and enemies
    public class HealthUI : MonoBehaviour
    {

        public HealthController healthController;

        [SerializeField]
        protected float hearthWidth = 1;

        [SerializeField]
        protected HealthHeart prefab;

        [SerializeField]
        protected int maxHeartsInRow = 4;

        protected readonly List<HealthHeart> _hearts = new();

        protected int _previousHealth = 0;

        // Update is called once per frame
        private void OnEnable()
        {
            if (healthController != null)
                return;

            healthController = GetComponent<HealthController>();
            if (healthController == null && transform.parent!=null)
                healthController = transform.parent.GetComponent<HealthController>();
        }

        private void FixedUpdate()
        {
            if (healthController == null)
                return;

            if (healthController.IsDead)
            {
                if (_hearts.Count != 0)
                    Invalidate();
                return;
            }

            if (transform.parent.localScale.x < 0)
                transform.localScale = new Vector3(-1, 1, 1);
            else
                transform.localScale = new Vector3(1, 1, 1);

            var ah = GetMaxHealth();
            if ((GetMaxHealth() + 1) / 2 != _hearts.Count)
            {
                Invalidate();
                _previousHealth = 0;
            }

            if (GetHealth() != _previousHealth)
            {
                _previousHealth = GetHealth();
                var healthLeft = _previousHealth;
                for (int i = _hearts.Count - 1; i >= 0; i--)
                {
                    var heart = _hearts[i];
                    if (healthLeft >= 2)
                    {
                        heart.Health = 2;
                        healthLeft -= 2;
                    }
                    else if (healthLeft == 1)
                    {
                        heart.Health = 1;
                        healthLeft = 0;
                    }
                    else
                    {
                        heart.Health = 0;
                    }
                }
            }
        }

        protected virtual int GetHealth()
        {
            return healthController.health;
        }

        protected virtual int GetMaxHealth()
        {
            return healthController.maxHealth;
        }

        protected virtual int GetHeartsCount()
        {
            return healthController.IsDead ? 0 : (GetMaxHealth() + 1) / 2; ;
        }

        private void Invalidate()
        {
            foreach (var heart in _hearts)
                Destroy(heart.gameObject);

            _hearts.Clear();


            int heartsCount = GetHeartsCount();

            for (int i = 0; i < heartsCount; i++)
            {
                int hearthRow = i / maxHeartsInRow;
                float startingPos = i % maxHeartsInRow * hearthWidth;

                var p = Instantiate(prefab);
                p.name = "heart" + i;
                p.transform.SetParent(transform, false);
                p.transform.localPosition = new Vector2(startingPos, hearthRow * hearthWidth);
                _hearts.Add(p);
            }
            _hearts.Reverse();
        }
    }
}