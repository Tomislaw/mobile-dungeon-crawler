using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace RuinsRaiders.UI {
    public class UpgradeButtonUI : MonoBehaviour
    {
        public UnityEvent OnPress;

        [SerializeField]
        private Color textEnabled;

        [SerializeField]
        private Color textDisabled;

        [SerializeField]
        private TMPro.TMP_Text text;

        [SerializeField]
        private Button buttton;

        public void SetData(bool enabled, int cost)
        {
            if (text)
            {
                text.text = cost.ToString();
                text.color = enabled ? textEnabled : textDisabled;
            }
           

            if(buttton)
                buttton.interactable = enabled;

        }

        private void Start()
        {
            if (buttton)
                buttton.onClick.AddListener(OnPress.Invoke);
        }

    }
}
