using System;
using UnityEngine;


namespace RuinsRaiders
{
    [CreateAssetMenu(fileName = "PlayerData", menuName = "RuinsRaiders/PlayerData", order = 1)]
    public class PlayerData : ScriptableObject
    {
        public Gems gems;

        public Character knight;
        public Character mage;
        public Character spearman;
        public Character archer;

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
            public int[] skills;

        }

        public ref Character GetCharacterData(CharacterType type)
        {
            switch (type)
            {
                case CharacterType.Knight:
                    return ref knight;
                case CharacterType.Mage:
                    return ref mage;
                case CharacterType.Archer:
                    return ref archer;
                default:
                    return ref spearman;
            }
        }

        public int GetGems(GemsType type)
        {
            switch (type)
            {
                case GemsType.Red:
                    return gems.redGems;
                case GemsType.Blue:
                    return gems.blueGems;
                case GemsType.Silver:
                    return gems.silverGems;
                case GemsType.Green:
                    return gems.greenGems;
                default:
                    return 0;
            }
        }

        public void UpgradeSkill(CharacterType character, int skill, GemsType type, int cost)
        {

            switch (type)
            {
                case GemsType.Red:
                    gems.redGems -= cost;
                    break;
                case GemsType.Blue:
                    gems.blueGems -= cost;
                    break;
                case GemsType.Silver:
                    gems.silverGems -= cost;
                    break;
                case GemsType.Green:
                    gems.greenGems -= cost;
                    break;
                default:
                    return;
            }

            GetCharacterData(character).skills[skill] += 1;
        }

        public enum CharacterType
        {
            Knight, Mage, Archer, Spearman
        }

        public enum GemsType
        {
            Red, Blue, Green, Silver
        }
    }
}