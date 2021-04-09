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

    public PlayerSpawn spawner;

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

    public void RestartLevel()
    {
        Debug.Log("Level " + adventure.levels[level].scene + " restarted");
        Scene scene = SceneManager.GetActiveScene(); SceneManager.LoadScene(scene.name);
    }

    public void Menu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void StartNextLevel()
    {
        if (level == adventure.levels.Count - 1)
            SceneManager.LoadScene("MainMenu");
        else
            SceneManager.LoadScene(adventure.levels[level + 1].scene);
    }

}