using UnityEngine;
using UnityEngine.UI;

namespace RuinsRaiders.UI
{
    public class ControlSpacingScaler : MonoBehaviour
    {
        public OptionsData options;

        public Type type = Type.None;
        public Slider slider;

        private RectTransform rectTransform;
        private Vector2 startingSize;
        private Vector2 startingPosition;
        private Vector3 startingScale;

        void OnEnable()
        {
            slider = GetComponent<Slider>();
            if (slider != null)
            {
                if (type.HasFlag(Type.OffsetX))
                    slider.value = options.touchUiSpacingX;
                if (type.HasFlag(Type.OffsetY))
                    slider.value = options.touchUiSpacingY;
                if (type.HasFlag(Type.Scale))
                    slider.value = options.touchUiScale;

            } else
            {
                rectTransform = gameObject.GetComponent<RectTransform>();
                startingSize = rectTransform.sizeDelta;
                startingPosition = rectTransform.anchoredPosition;
                startingScale = rectTransform.localScale;
            }
        }
        private void OnDisable()
        {
            if (slider == null)
            {
                if (type.HasFlag(Type.OffsetX))
                    rectTransform.sizeDelta = startingSize;
                if (type.HasFlag(Type.OffsetY))
                    rectTransform.anchoredPosition = startingPosition;
                if (type.HasFlag(Type.Scale))
                    rectTransform.localScale = startingScale;
            }
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            if (slider != null)
            {
                if (type.HasFlag(Type.OffsetX))
                    options.touchUiSpacingX = slider.value;
                if (type.HasFlag(Type.OffsetY))
                    options.touchUiSpacingY = slider.value;
                if (type.HasFlag(Type.Scale))
                    options.touchUiScale = slider.value;
            }
            else
            {
                if (type.HasFlag(Type.OffsetX))
                {
                    var newSize = startingSize - new Vector2(options.touchUiSpacingX * rectTransform.rect.width / 2f, 0);
                    if (rectTransform.sizeDelta != newSize)
                        rectTransform.sizeDelta = newSize;
                }
                if (type.HasFlag(Type.OffsetY))
                {
                    var newPosition = startingPosition + new Vector2(0, options.touchUiSpacingY * rectTransform.rect.height / 2f);
                    if (rectTransform.anchoredPosition != newPosition)
                        rectTransform.anchoredPosition = newPosition;
                }
                if (type.HasFlag(Type.Scale))
                {
                    var newScale = startingScale * options.touchUiScale;
                    if (rectTransform.localScale != newScale)
                        rectTransform.localScale = newScale;
                }
            }
        }

        [System.Flags]
        public enum Type
        {
            None = 0, OffsetX = 1, OffsetY = 2, Scale = 4
        }
    }
}