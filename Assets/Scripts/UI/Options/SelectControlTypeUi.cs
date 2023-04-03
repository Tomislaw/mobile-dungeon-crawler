using UnityEngine;
using UnityEngine.UI;

namespace RuinsRaiders.UI
{
    // Helper class for selecting different control type on Mobile
    [ExecuteAlways]
    public class SelectControlTypeUi : MonoBehaviour
    {
        [SerializeField]
        private OptionsData.TouchUiType type;

        [SerializeField]
        private OptionsData data;

        private Toggle _toggle;

        void OnEnable()
        {
            _toggle = GetComponent<Toggle>();
            _toggle.isOn = type == data.touchUiType;
            _toggle.onValueChanged.AddListener(OnChange);
        }

        public void OnChange(bool change)
        {
            if (change)
                data.touchUiType = type;
        }
    }
}