using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace RuinsRaiders
{
    [CreateAssetMenu(fileName = "AdventureData", menuName = "RuinsRaiders/AdventureData", order = 1)]
    public class AdventureData : ScriptableObject
    {
        public List<Level> levels = new();

        public Level GetCurrentLevel()
        {
            return levels.Find(it => it.scene.Equals(SceneManager.GetActiveScene().name));
        }

        public Level GetNextLevel()
        {
            for (int i = 0; i < levels.Count; i++)
            {
                if (levels[i].scene != (SceneManager.GetActiveScene().name))
                    continue;

                if (i >= levels.Count - 1)
                    return null;

                return levels[i + 1];
            }
            return null;
        }

        public void FinishedCurrentLevel()
        {
            for (int i = 0; i < levels.Count; i++)
            {
                if (levels[i].scene != (SceneManager.GetActiveScene().name))
                    continue;


                levels[i].finished = true;
                if (i < levels.Count - 1)
                    levels[i].enabled = true;
            }
        }

        [Serializable]
        public class Level
        {
            public bool enabled;
            public bool finished;
            public List<ChestData> chests = new();
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
            public Type type;

            public enum Type
            {
                Normal, Exotic
            }
        }
    }
}