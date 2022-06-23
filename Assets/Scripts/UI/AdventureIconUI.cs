using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RuinsRaiders.UI
{
    // responsible for single Campaign block on level selection screen
    public class AdventureIconUI : MonoBehaviour
    {
        [SerializeField]
        private GameObject showOnSelect;
        [SerializeField]
        private bool selected;

        [SerializeField]
        [HideInInspector]
        private bool hovered;

        [SerializeField]
        private Sprite normalSprite;
        [SerializeField]
        private Sprite hoveredSprite;

        [SerializeField]
        private List<Image> images = new();


        private AdventureIconUI[] _allIcons;

        private void Start()
        {
            _allIcons = FindObjectsOfType<AdventureIconUI>(true);
            Invalidate();
        }

        public void SetSelected(bool selected)
        {
            if (showOnSelect)
                showOnSelect.SetActive(selected);

            if (this.selected == selected)
                return;

            this.selected = selected;
            if (selected)
                foreach (var icon in _allIcons)
                {
                    if (icon.gameObject == gameObject)
                        continue;

                    icon.SetSelected(false);
                }

            Invalidate();
        }

        public void Invalidate()
        {
            foreach (var image in images)
            {
                image.sprite = this.selected || this.hovered ? hoveredSprite : normalSprite;
            }
        }

        public void SetHovered(bool hovered)
        {
            this.hovered = hovered;
            Invalidate();
        }

    }
}