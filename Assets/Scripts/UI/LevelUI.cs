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

        public void Start()
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

            if (tmpText)
                tmpText.text = (level + 1).ToString();

            SetHovered(false);
        }

        public void StartLevel()
        {
            EventManager.TriggerEvent("Level Start");
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