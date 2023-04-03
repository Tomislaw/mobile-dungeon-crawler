using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace RuinsRaiders {
    public class DebugPrefViewerUI : MonoBehaviour
    {
        public Button button;
        public TMPro.TMP_Text text;

        private List<GameObject> prefButtons = new();

        void OnEnable()
        {
            button.gameObject.SetActive(false);

            var prefs = SaveableData.GetAll();
    
        foreach (var button in prefButtons)
            {
                Destroy(button.gameObject);
            }
            prefButtons.Clear();

            foreach (var key in prefs)
            {
                var newButton = Instantiate(button);
                newButton.GetComponentInChildren<TMPro.TMP_Text>().text = key.GetFileName();
                newButton.onClick.AddListener(() => OnSelectedPref(key));
                newButton.gameObject.SetActive(true);
                newButton.transform.SetParent(button.transform.parent, false);
                newButton.transform.localScale = Vector3.one;
                prefButtons.Add(newButton.gameObject);
            }
        }

        public void OnSelectedPref(SaveableData data)
        {
            text.text = data.AsSerializedString();
        }
    }
}