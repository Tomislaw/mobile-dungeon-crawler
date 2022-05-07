using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class LevelEvents : MonoBehaviour
{
    public AdventureData adventure;
    public int level;

    public GameObject playerCharacter;

    public List<AdventureData.ChestData> chests = new List<AdventureData.ChestData>();

    public UnityEvent OnLevelFinished;
    public UnityEvent OnLevelFailed;
    public UnityEvent OnSubsceneChanged;

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

        Debug.Log("Level " + adventure.levels[level].scene + " failed");
    }

    public void LevelFinished()
    {
        if (OnLevelFinished != null)
            OnLevelFinished.Invoke();

        Debug.Log("Level " + adventure.levels[level].scene + " finished");
    }

    public void LevelRestarted()
    {
        Debug.Log("Level " + adventure.levels[level].scene + " restarted");
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
        if (level == adventure.levels.Count - 1)
            SceneManager.LoadScene("MainMenu");
        else
            SceneManager.LoadScene(adventure.levels[level + 1].scene);
    }
}