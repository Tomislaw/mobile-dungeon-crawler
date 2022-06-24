using System;
using UnityEngine;


namespace RuinsRaiders
{
    [CreateAssetMenu(fileName = "PlayerData", menuName = "RuinsRaiders/PlayerData", order = 1)]
    public class PlayerData : ScriptableObject
    {
        [Serializable]
        public struct Gems
        {
            public int redGems;
            public int blueGems;
            public int greenGems;
            public int silverGems;
        }

        [Serializable]
        public struct Character
        {
            public bool unlocked;
            public int healthUpgrades;
            public int attackUpgrades;
            public int staminaUpgrades;
            public int specialUpgrades;
        }

        public Gems gems;

        public Character knight;
        public Character mage;
        public Character spearman;
        public Character archer;

        public CharacterType selectedCharacter = 0;

        public enum CharacterType
        {
            Knight, Mage, Archer, Spearman
        }
    }
}