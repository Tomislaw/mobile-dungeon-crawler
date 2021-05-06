using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CharacterSelectUI : MonoBehaviour
{
    public PlayerData playerData;
    public PlayerData.CharacterType characterType;

    public UnityEvent IsLocked;
    public UnityEvent IsUnlocked;
    void OnEnable()
    {
        if (IsCharacterUnlocked)
            IsUnlocked.Invoke();
        else
            IsLocked.Invoke();

    }

    public bool IsCharacterUnlocked { 
        get {
            switch (characterType)
            {
                case PlayerData.CharacterType.Knight:
                    return playerData.knight.unlocked;
                case PlayerData.CharacterType.Mage:
                    return playerData.mage.unlocked;
                case PlayerData.CharacterType.Archer:
                    return playerData.archer.unlocked;
                case PlayerData.CharacterType.Spearman:
                    return playerData.spearman.unlocked;
                default:
                    return false;
            };
        } 
    }
}