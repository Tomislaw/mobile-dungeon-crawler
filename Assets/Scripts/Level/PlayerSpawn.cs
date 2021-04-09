using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class PlayerSpawn : MonoBehaviour
{
    public PlayerData data;

    public Character knightPrefab;
    public Character archerPrefab;
    public Character magePrefab;
    public Character spermanPrefab;

    public LevelEvents levelEvents;

    private void Start()
    {
    }

    // Update is called once per frame
    private void OnValidate()
    {
        if (levelEvents == null)
            levelEvents = Object.FindObjectOfType<LevelEvents>();
    }

    public void SpawnCharacter()
    {
    }
}