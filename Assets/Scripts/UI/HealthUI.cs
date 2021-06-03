using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthUI : MonoBehaviour
{
    public HealthHeart prefab;

    private List<HealthHeart> hearts = new List<HealthHeart>();

    public Character character;

    private int previousHealth = 0;

    public int maxHeartsInRow = 4;

    // Update is called once per frame
    private void FixedUpdate()
    {
        if (character.IsDead)
        {
            if (hearts.Count != 0)
                Invalidate();
            return;
        }

        if (transform.parent.localScale.x < 0)
            transform.localScale = new Vector3(-1, 1, 1);
        else
            transform.localScale = new Vector3(1, 1, 1);

        if ((character.MaxHealth + 1) / 2 != hearts.Count)
        {
            Invalidate();
            previousHealth = 0;
        }

        if (character.Health != previousHealth)
        {
            previousHealth = character.Health;
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

        
        int heartsCount = character.IsDead ? 0 : (character.MaxHealth + 1) / 2;

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