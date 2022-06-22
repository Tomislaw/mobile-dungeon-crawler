using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace RuinsRaiders
{
    // responsible for propagating global level events and handling level actions
    public class LevelEvents : MonoBehaviour
    {
        public UnityEvent OnLevelFinished;
        public UnityEvent OnLevelFailed;
        public UnityEvent OnSubsceneChanged;

        [SerializeField]
        private AdventureData adventure;

        [SerializeField]
        private GameObject playerCharacter;

        [SerializeField]
        private List<AdventureData.ChestData> chests = new List<AdventureData.ChestData>();

        private PlayerSpawn spawner;

        public void OnEnable()
        {
            EventManager.StartListening("LevelFailed", LevelFailed);
            EventManager.StartListening("LevelFinished", LevelFinished);
            EventManager.StartListening("LevelRestarted", LevelRestarted);
            spawner = FindObjectOfType<PlayerSpawn>();
        }

        public void OnDisable()
        {
            EventManager.StopListening("LevelFailed", LevelFailed);
            EventManager.StopListening("LevelFinished", LevelFinished);
            EventManager.StopListening("LevelRestarted", LevelRestarted);
        }

        public void LevelFailed()
        {
            if (OnLevelFailed != null)
                OnLevelFailed.Invoke();

            Debug.Log("Level failed");
        }

        public void LevelFinished()
        {
            if (OnLevelFinished != null)
                OnLevelFinished.Invoke();

            adventure.FinishedCurrentLevel();
            Debug.Log("Level finished");
        }

        public void LevelRestarted()
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