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

        [SerializeField]
        private float delayBetwenLoadingScenes = 0.5f;

        public static LevelEvents Instance { get => FindObjectOfType<LevelEvents>(); }

        private List<ChestTile> _chests = new();
        private bool _levelEnded = false;
        private PlayerSpawn _spawner;
        private Coroutine _sceneCoroutine = null;

        public void Awake()
        {
            ValidityChecks();
            _spawner = FindObjectOfType<PlayerSpawn>();
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



        public AdventureData.ChestData GetChestData(ChestTile chest)
        {
            if (adventure == null)
                return AdventureData.ChestData.Empty;

            var adventureChestData = adventure.GetCurrentLevel().chests;

            if (!_chests.Contains(chest))
            {
                _chests.Add(chest);
                _chests.Sort(delegate (ChestTile c1, ChestTile c2) { return c1.transform.position.x.CompareTo(c1.transform.position.x); });

                foreach (var chestTile in _chests)
                    if (chestTile != chest)
                        chestTile.Refresh();
            }


            if (adventureChestData.Count < _chests.Count)
                Debug.LogErrorFormat("Scene have more than {0} chests!", adventureChestData.Count);

            for (int i = 0; i < _chests.Count; i++)
            {
                if (i >= adventureChestData.Count)
                    return AdventureData.ChestData.Empty;

                if (_chests[i] != chest)
                    continue;

                return adventureChestData[i];
            }

            return AdventureData.ChestData.Empty;
        }

        public void FailLevel()
        {
            if (_levelEnded)
                return;
            _levelEnded = true;

            if (onLevelFailed != null)
                onLevelFailed.Invoke();

            EventManager.TriggerEvent("Level Failed");
        }

        public void FinishLevel()
        {
            if (_levelEnded)
                return;
            _levelEnded = true;

            if (onLevelFinished != null)
                onLevelFinished.Invoke();
            adventure.FinishedCurrentLevel();

            EventManager.TriggerEvent("Level Finished");
        }

        public List<(ChestTile, bool)> GetCollectedChests()
        {
            var adventureChests = adventure.GetCurrentLevel().chests;
            List<(ChestTile, bool)> collectedChests = new();

            for (int i = 0; i < _chests.Count && i < adventureChests.Count; i++)
            {
                bool giveReward = _chests[i].isOpen && !adventureChests[i].acquired;
                collectedChests.Add((_chests[i], giveReward));

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
            (adventure as SaveableData).Save();
            (playerData as SaveableData).Save();
        }


        public void RestartLevel()
        {
            EventManager.TriggerEvent("Level Restarted");
            Scene scene = SceneManager.GetActiveScene();
            LoadScene(scene.name);
        }

        public void Menu()
        {
            EventManager.TriggerEvent("Menu");
            LoadScene("MainMenu");
        }

        public void SelectHero(int id)
        {
            _spawner.Spawn((PlayerSpawn.CharacterType)id);
        }

        public void StartNextLevel()
        {
            var level = adventure.GetNextLevel();
            if (level == null)
                LoadScene("MainMenu");
            else
                LoadScene(level.scene);
        }

        private void LoadScene(string scene)
        {
            if(_sceneCoroutine == null)
                StartCoroutine(LoadSceneCoroutine(scene));
        }

        private IEnumerator LoadSceneCoroutine(string scene)
        {
            EventManager.TriggerEvent("Next Scene");
            yield return new WaitForSeconds(delayBetwenLoadingScenes);
            SceneManager.LoadScene(scene);
            _sceneCoroutine = null;
        }
    }
}