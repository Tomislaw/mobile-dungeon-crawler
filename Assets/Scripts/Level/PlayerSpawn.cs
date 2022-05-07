using System;
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
    public Character spearmanPrefab;


    public void Spawn(CharacterType type)
    {
        Character player = null;
        switch (type)
        {
            case CharacterType.Spearman:
                player = Instantiate(spearmanPrefab);
                break;
            case CharacterType.Mage:
                player = Instantiate(magePrefab);
                break;
            case CharacterType.Knight:
                player = Instantiate(knightPrefab);
                break;
            case CharacterType.Bowman:
                player = Instantiate(archerPrefab);
                break;
        }
        player.transform.position = transform.position;
        transform.parent = player.gameObject.transform;
        player.GetComponent<HealthController>()?.OnDeath.AddListener(OnDeath);
    }

    private void OnDeath()
    {
        FindObjectOfType<LevelEvents>()?.LevelFailed();
    }

    [Serializable]
    public enum CharacterType
    {
        Knight, Bowman, Spearman, Mage
    }
}