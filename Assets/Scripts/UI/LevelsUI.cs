using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace RuinsRaiders.UI
{

    [ExecuteAlways]
    public class LevelsUI : MonoBehaviour
    {
        [SerializeField]
        private AdventureData data;

        [SerializeField]
        private GameObject whenLocked;

        [SerializeField]
        private GameObject whenUnlocked;

        void OnEnable()
        {
            if(data != null)
            {
                if (whenUnlocked)
                    whenUnlocked.SetActive(data.enabled);

                if (whenLocked)
                    whenLocked.SetActive(!data.enabled);
            }
        }
    }
}