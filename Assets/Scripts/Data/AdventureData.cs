using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(fileName = "AdventureData", menuName = "RuinsRaiders/AdventureData", order = 1)]
public class AdventureData : ScriptableObject
{
    public List<Level> levels = new List<Level>();

    [Serializable]
    public class Level
    {
        public bool enabled;
        public bool finished;
        public List<ChestData> chests = new List<ChestData>();
        public PlayerData.Gems firstClearReward;

        public string scene;
    }

    public void Invalidate()
    {
        Level previous = null;
        foreach (var level in levels)
        {
            if (previous == null)
            {
                previous = level;
                break;
            }

            level.enabled = previous.finished;

            previous = level;
        }
    }

    [Serializable]
    public struct ChestData
    {
        public bool acquired;
        public int type;
    }
}