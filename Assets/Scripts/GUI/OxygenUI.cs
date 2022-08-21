﻿using System.Collections.Generic;
using UnityEngine;

namespace RuinsRaiders.GUI
{
    // Responsible for showinh hearts above player and enemies
    public class OxygenUI : MonoBehaviour
    {
        [SerializeField]
        private OxygenBubble prefab;
        [SerializeField]
        private OxygenController oxygenController;
        [SerializeField]
        private HealthController healthController;
        [SerializeField]
        private int maxBubblesInRow = 4;

        private readonly List<OxygenBubble> _bubbles = new();

        private int _previousOxygen = 0;

        // Update is called once per frame
        private void OnEnable()
        {
            if (oxygenController != null)
                return;

            oxygenController = GetComponent<OxygenController>();
            if (oxygenController == null && transform.parent != null)
                oxygenController = transform.parent.GetComponent<OxygenController>();

            if (healthController != null)
                return;
            healthController = GetComponent<HealthController>();
            if (healthController == null && transform.parent != null)
                healthController = transform.parent.GetComponent<HealthController>();
        }

        private void FixedUpdate()
        {
            if (oxygenController == null)
                return;

            if (!oxygenController.inWater || healthController == null || healthController.IsDead)
            {
                if (_bubbles.Count != 0)
                    Invalidate();
                return;
            }

            if (transform.parent.localScale.x < 0)
                transform.localScale = new Vector3(-1, 1, 1);
            else
                transform.localScale = new Vector3(1, 1, 1);

            if ((oxygenController.maxOxygen + 1) / 2 != _bubbles.Count)
            {
                Invalidate();
                _previousOxygen = 0;
            }

            if (oxygenController.oxygen != _previousOxygen)
            {
                _previousOxygen = oxygenController.oxygen;
                var oxygenLeft = _previousOxygen;
                for (int i = _bubbles.Count - 1; i >= 0; i--)
                {
                    var bubble = _bubbles[i];
                    if (oxygenLeft >= 2)
                    {
                        bubble.Oxygen = 2;
                        oxygenLeft -= 2;
                    }
                    else if (oxygenLeft == 1)
                    {
                        bubble.Oxygen = 1;
                        oxygenLeft = 0;
                    }
                    else
                    {
                        bubble.Oxygen = 0;
                    }
                }
            }
        }

        private void Invalidate()
        {
            foreach (var bubble in _bubbles)
                Destroy(bubble.gameObject);

            _bubbles.Clear();

            int bubbleCount = !oxygenController.inWater || healthController.IsDead ? 0 : (oxygenController.maxOxygen + 1) / 2;

            float bubbleWidth = 1;

            for (int i = 0; i < bubbleCount; i++)
            {
                int bubbleRow = i / maxBubblesInRow;
                int bubblesInRow = Mathf.Min(maxBubblesInRow, bubbleCount - bubbleRow * maxBubblesInRow);
                float startingPos = (float)bubblesInRow / 2 - bubbleWidth / 2 - bubbleWidth * i % maxBubblesInRow;

                var p = Instantiate(prefab);
                p.name = "bubble" + i;
                p.transform.SetParent(transform, false);
                p.transform.localPosition = new Vector2(startingPos, bubbleRow * bubbleWidth);
                _bubbles.Add(p);
            }
        }
    }
}