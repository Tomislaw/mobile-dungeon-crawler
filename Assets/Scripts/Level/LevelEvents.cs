using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace RuinsRaiders
{
    // responsible for propagating global level events and handling level actions
    [ExecuteAlways]
    public class LevelEvents : MonoBehaviour
    {
        public UnityEvent onLevelFinished;
        public UnityEvent onLevelFailed;
        public UnityEvent onSubsceneChanged;

        [SerializeField]
        private AdventureData adventure;

        [SerializeField]
        private PlayerData playerData;

        [SerializeField]
        private RewardsData rewardsData;

        [SerializeField]
        private GameObject playerCharacter;

        private PlayerSpawn spawner;

        public static LevelEvents Instance { get => FindObjectOfType<LevelEvents>(); }

        private List<ChestTile> chests = new ();

        private bool levelEnded = false;

        public void Awake()
        {
            ValidityChecks();

            EventManager.StartListening("LevelFailed", FailLevel);
            EventManager.StartListening("LevelFinished", FinishLevel);
            EventManager.StartListening("LevelRestarted", RestartLevel);
            EventManager.StartListening("Menu", Menu);
            spawner = FindObjectOfType<PlayerSpawn>();
        }

        private void ValidityChecks()
        {
            if (SceneManager.GetActiveScene().name.Contains("Test"))
                return; 
            if (adventure == null)
                Debug.LogErrorFormat("Object {0} is missing adventureData!", name);
            if (playerData == null)
                Debug.LogErrorFormat("Object {0} is missing playerData!", name);
            if (rewardsData == null)
                Debug.LogErrorFormat("Object {0} is missing rewardsData!", name);
            if (adventure != null)
            {
                var list = adventure.levels.FindAll(it => it.scene.Equals(SceneManager.GetActiveScene().name));
                if (list.Count != 1)
                    Debug.LogErrorFormat("Object {0} have invalid adventureData!", name);
            }
        }

        public void OnDisable()
        {
            EventManager.StopListening("LevelFailed", FailLevel);
            EventManager.StopListening("LevelFinished", FinishLevel);
            EventManager.StopListening("LevelRestarted", RestartLevel);
            EventManager.StopListening("Menu", Menu);
        }


        public AdventureData.ChestData GetChestData(ChestTile chest)
        {
            if(adventure == null)
                return AdventureData.ChestData.Empty;

            var adventureChestData = adventure.GetCurrentLevel().chests;

            if (!chests.Contains(chest))
            {
                chests.Add(chest);
                chests.Sort(delegate (ChestTile c1, ChestTile c2) { return c1.transform.position.x.CompareTo(c1.transform.position.x); });

                foreach (var chestTile in chests)
                    if (chestTile != chest)
                        chestTile.Refresh();
            }


            if (adventureChestData.Count < chests.Count)
                Debug.LogErrorFormat("Scene have more than {0} chests!", adventureChestData.Count);

            for (int i = 0; i < chests.Count; i++)
            {
                if (i >= adventureChestData.Count)
                    return AdventureData.ChestData.Empty;

                if (chests[i] != chest)
                    continue;

                return adventureChestData[i];
            }

            return AdventureData.ChestData.Empty;
        }

        public void FailLevel()
        {
            if (levelEnded)
                return;
            levelEnded = true;

            if (onLevelFailed != null)
                onLevelFailed.Invoke();

            Debug.Log("Level failed");
        }

        public void FinishLevel()
        {
            if (levelEnded)
                return;
            levelEnded = true;

            if (onLevelFinished != null)
                onLevelFinished.Invoke();
            adventure.FinishedCurrentLevel();

            Debug.Log("Level finished");
        }

        public List<(ChestTile, bool)> GetCollectedChests()
        {
            var adventureChests = adventure.GetCurrentLevel().chests;
            List<(ChestTile, bool)> collectedChests = new();

            for (int i = 0; i < chests.Count && i < adventureChests.Count; i++)
            {
                bool giveReward = chests[i].isOpen && !adventureChests[i].acquired;
                collectedChests.Add((chests[i], giveReward));

            }
            return collectedChests;
        }

        public void GiveLevelRewards()
        {
            var collectedChests = GetCollectedChests();
            for (int i = 0; i < collectedChests.Count; i++)
            {

                if (collectedChests[i].Item2 == false)
                    continue;

                var collectedChest = collectedChests[i].Item1;
                switch (collectedChest.chestData.type)
                {
                    case AdventureData.ChestData.Type.Normal:
                        playerData.gems += rewardsData.normalChestReward;
                        break;
                    case AdventureData.ChestData.Type.Equisite:
                        playerData.gems += rewardsData.exquisiteChestReward;
                        break;
                }
                adventure.GetCurrentLevel().chests[i] = new() { acquired = true, type = collectedChest.chestData.type };
            }
        }


        public void RestartLevel()
        {
            Debug.Log("Level restarted");
            Scene scene = SceneManager.GetActiveScene(); SceneManager.LoadScene(scene.name);
        }

        public void Menu()
        {
            SceneManager.LoadScene("MainMenu");
        }

        public void SelectHero(int id)
        {
            spawner.Spawn((PlayerSpawn.CharacterType)id);
        }

        public void StartNextLevel()
        {
            var level = adventure.GetNextLevel();
            if (level == null)
                SceneManager.LoadScene("MainMenu");
            else
                SceneManager.LoadScene(level.scene);
        }
    }
}