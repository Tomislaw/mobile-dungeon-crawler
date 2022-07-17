using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static RuinsRaiders.PlayerData;

namespace RuinsRaiders
{
    [CreateAssetMenu(fileName = "UpgradeCostrData", menuName = "RuinsRaiders/UpgradeCostrData", order = 1)]
    public class UpgradeCostData : ScriptableObject
    {

        public List<CharacterUpgrades> upgrades = new();

        public (int, GemsType) GetCost(CharacterType character, int skill, int upgrade)
        {
            var characterUpgrades = upgrades.FirstOrDefault(it => it.character == character);

            if (characterUpgrades.skills.Length <= skill || characterUpgrades.skills[skill].cost.Length <= upgrade)
                return (int.MaxValue, GemsType.Silver);


            return (characterUpgrades.skills[skill].cost[upgrade], characterUpgrades.skills[skill].type);
        }

        [Serializable]
        public struct SkillUpgradeCost
        {
            public int[] cost;
            public GemsType type;
        }

        [Serializable]
        public struct CharacterUpgrades
        {
            public SkillUpgradeCost[] skills;
            public CharacterType character;
        }
    }
}
