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
        private bool IsLocked;

        [SerializeField]
        private Sprite locked;
        [SerializeField]
        private Sprite hovered;
        [SerializeField]
        private Sprite normal;

        [SerializeField]
        private TMPro.TMP_Text text;

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
            if (IsLocked)
            {
                if (image)
                    image.sprite = locked;
                if (text)
                    text.gameObject.SetActive(false);
                if (button)
                    button.interactable = false;
            }
            else
            {
                if (image)
                    image.sprite = hovered ? this.hovered : this.normal;
                if (text)
                    text.gameObject.SetActive(true);
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
            IsLocked = !levelData.enabled;

            if (text)
                text.text = (level + 1).ToString();

            SetHovered(false);
        }

        public void StartLevel()
        {
            EventManager.TriggerEvent("Level Start");
            StartCoroutine("DelayedStart");
        }

        // Start is delayed to show some cool transition effects
        private IEnumerator DelayedStart()
        {
            yield return new WaitForSeconds(startDelay);
            SceneManager.LoadScene(data.levels[level].scene);
        }
    }
}