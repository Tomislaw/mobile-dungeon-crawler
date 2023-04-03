using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace RuinsRaiders.UI
{
    // Helper class for Level selection UI
    [ExecuteAlways]
    public class LevelUI : MonoBehaviour
    {
        [SerializeField]
        private bool isLocked;


        [SerializeField]
        private Sprite lockedSprite;

        [SerializeField]
        private Sprite hoveredSprite;

        [SerializeField]
        private Sprite normalSprite;

        [SerializeField]
        private Sprite noChestSprite;

        [SerializeField]
        private Sprite normalChestSprite;

        [SerializeField]
        private Sprite exquisiteChestSprite;

        [SerializeField]
        private Image[] chests;

        [SerializeField]
        private TMPro.TMP_Text tmpText;


        [SerializeField]
        private Image image;

        [SerializeField]
        private Button button;

        [SerializeField]
        private AdventureData data;

        [SerializeField]
        private int level;

        [SerializeField]
        private float startDelay = 0.5f;

        public void OnEnable()
        {
            Invalidate();
        }

        public void SetHovered(bool hovered)
        {
            if (isLocked)
            {
                if (image)
                    image.sprite = lockedSprite;
                if (tmpText)
                    tmpText.gameObject.SetActive(false);
                if (button)
                    button.interactable = false;
            }
            else
            {
                if (image)
                    image.sprite = hovered ? this.hoveredSprite : this.normalSprite;
                if (tmpText)
                    tmpText.gameObject.SetActive(true);
                if (button)
                    button.interactable = true;
            }
        }

        private void OnValidate()
        {
            Invalidate();
        }

        public void Invalidate()
        {
            if (data == null || data.levels.Count <= level || level < 0)
                return;

            var levelData = data.levels[level];
            isLocked = !levelData.enabled;

            if (chests.Length != levelData.chests.Count)
                Debug.LogWarningFormat("Invalid number of chests in ui for {0} in {1}", name, transform.parent.name);

            for (int i = 0; i < chests.Length && i < levelData.chests.Count; i++)
            {
                if (!levelData.chests[i].acquired)
                    chests[i].sprite = noChestSprite;
                else
                    switch (levelData.chests[i].type)
                    {
                        case AdventureData.ChestData.Type.Normal:
                            chests[i].sprite = normalChestSprite;
                            break;
                        case AdventureData.ChestData.Type.Equisite:
                            chests[i].sprite = exquisiteChestSprite;
                            break;
                    }
            }

            if (tmpText)
                tmpText.text = (level + 1).ToString();

            SetHovered(false);
        }

        public void StartLevel()
        {
            EventManager.TriggerEvent("Level Start");
            EventManager.TriggerEvent("Next Scene");
            StartCoroutine(DelayedStart());
        }

        // Start is delayed to show some cool transition effects
        private IEnumerator DelayedStart()
        {
            yield return new WaitForSeconds(startDelay);
            SceneManager.LoadScene(data.levels[level].scene);
        }
    }
}