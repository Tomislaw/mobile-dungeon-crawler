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
                foreach (var level in adventure.levels) { 
                    level.enabled = false;
                    level.finished = false;
                }
                if (adventure.name == "Mountians")
                {
                    adventure.enabled = true;
                    adventure.levels[0].enabled = true;
                }
            }
        }

        public void RemoveData()
        {
            LockLevels();
            ResetMoney();

            var adventures = Resources.FindObjectsOfTypeAll<AdventureData>();
            foreach (var adventure in adventures)
                foreach (var level in adventure.levels)
                    for (int i = 0; i < level.chests.Count; i++)
                        level.chests[i] = new AdventureData.ChestData { acquired = false, type = level.chests[i].type };

            playerData.knight = new PlayerData.Character { skills = new int[3], unlocked = true};
            playerData.archer = new PlayerData.Character { skills = new int[3], unlocked = true };
            playerData.mage = new PlayerData.Character { skills = new int[3] };
            playerData.spearman = new PlayerData.Character { skills = new int[3] };

            SaveableData.DeleteAll();
        }

    }
}
