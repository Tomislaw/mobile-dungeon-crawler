using System.Collections.Generic;
using UnityEngine;

namespace RuinsRaiders.GUI
{
    // Responsible for showinh hearts above player and enemies
    public class HealthUI : MonoBehaviour
    {
        [SerializeField]
        private HealthHeart prefab;
        [SerializeField]
        private HealthController healthController;
        [SerializeField]
        private int maxHeartsInRow = 4;

        private readonly List<HealthHeart> _hearts = new();

        private int _previousHealth = 0;

        // Update is called once per frame
        private void OnEnable()
        {
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

            if ((healthController.haxHealth + 1) / 2 != _hearts.Count)
            {
                Invalidate();
                _previousHealth = 0;
            }

            if (healthController.health != _previousHealth)
            {
                _previousHealth = healthController.health;
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

        private void Invalidate()
        {
            foreach (var heart in _hearts)
                Destroy(heart.gameObject);

            _hearts.Clear();


            int heartsCount = healthController.IsDead ? 0 : (healthController.haxHealth + 1) / 2;

            float hearthWidth = 1;

            for (int i = 0; i < heartsCount; i++)
            {
                int hearthRow = i / maxHeartsInRow;
                int heartsInRow = Mathf.Min(maxHeartsInRow, heartsCount - hearthRow * maxHeartsInRow);
                float startingPos = (float)heartsInRow / 2 - hearthWidth / 2 - hearthWidth * i % maxHeartsInRow;

                var p = Instantiate(prefab);
                p.name = "heart" + i;
                p.transform.SetParent(transform, false);
                p.transform.localPosition = new Vector2(startingPos, hearthRow * hearthWidth);
                _hearts.Add(p);
            }
        }
    }
}