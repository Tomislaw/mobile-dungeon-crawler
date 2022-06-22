using UnityEngine;
using UnityEngine.UI;

namespace RuinsRaiders.UI
{
    // Helper class for selecting different control type on Mobile
    public class SelectControlTypeUi : MonoBehaviour
    {
        [SerializeField]
        private OptionsData.TouchUiType type;
        [SerializeField]
        private OptionsData data;

        private Toggle toggle;

        void Start()
        {
            toggle = GetComponent<Toggle>();
            toggle.isOn = type == data.touchUiType;
            toggle.onValueChanged.AddListener(OnChange);
        }

        public void OnChange(bool change)
        {
            if (change)
                data.touchUiType = type;
        }
    }
}