using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RuinsRaiders.UI
{
    public class DebugActionsUI : MonoBehaviour
    {
        public PlayerData playerData;


        public void AddMoney(int money)
        {
            playerData.gems += new PlayerData.Gems{redGems = money, blueGems = money, silverGems = money, greenGems = money};
            (playerData as SaveableData).Save();
        }

        public void ResetMoney()
        {
            playerData.gems = new PlayerData.Gems();
            (playerData as SaveableData).Save();
        }

        public void UnlockLevels()
        {
            var adventures = Resources.FindObjectsOfTypeAll<AdventureData>();
            foreach (var adventure in adventures)
            {
                adventure.enabled = true;
                foreach(var level in adventure.levels)
                    level.enabled = true;
            }
        }

        public void LockLevels()
        { 
            var adventures = Resources.FindObjectsOfTypeAll<AdventureData>();
            foreach (var adventure in adventures)
            {
                adventure.enabled = false;
                foreach (var level in adventure.levels)
                    level.enabled = false;

                if (adventure.name == "Mountians")
                {
                    adventure.enabled = true;
                    adventure.levels[0].enabled = true;
                }
            }
        }

        public void RemoveData()
        {
            SaveableData.DeleteAll();
        }

    }
}
