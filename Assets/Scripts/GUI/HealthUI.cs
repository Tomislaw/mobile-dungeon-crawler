using System.Collections.Generic;
using UnityEngine;

namespace RuinsRaiders.GUI
{
    // Responsible for showinh hearts above player and enemies
    public class HealthUI : MonoBehaviour
    {
        [SerializeField]
        public HealthHeart prefab;
        [SerializeField]
        public HealthController healthController;
        [SerializeField]
        public int maxHeartsInRow = 4;

        private List<HealthHeart> hearts = new List<HealthHeart>();

        private int previousHealth = 0;

        // Update is called once per frame
        private void OnEnable()
        {
            healthController = GetComponent<HealthController>();
            if (healthController == null)
                healthController = transform.parent?.GetComponent<HealthController>();
        }

        private void FixedUpdate()
        {
            if (healthController == null)
                return;

            if (healthController.IsDead)
            {
                if (hearts.Count != 0)
                    Invalidate();
                return;
            }

            if (transform.parent.localScale.x < 0)
                transform.localScale = new Vector3(-1, 1, 1);
            else
                transform.localScale = new Vector3(1, 1, 1);

            if ((healthController.MaxHealth + 1) / 2 != hearts.Count)
            {
                Invalidate();
                previousHealth = 0;
            }

            if (healthController.Health != previousHealth)
            {
                previousHealth = healthController.Health;
                var healthLeft = previousHealth;
                for (int i = hearts.Count - 1; i >= 0; i--)
                {
                    var heart = hearts[i];
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
            foreach (var heart in hearts)
                Destroy(heart.gameObject);

            hearts.Clear();


            int heartsCount = healthController.IsDead ? 0 : (healthController.MaxHealth + 1) / 2;

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
                hearts.Add(p);
            }
        }
    }
}