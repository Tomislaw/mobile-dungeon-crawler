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
        public int[] skillCosts;
        public List<CharacterUpgrades> upgrades = new();

        public (int, GemsType) GetCost(CharacterType character, int skill, int upgrade)
        {
            var characterUpgrades = upgrades.FirstOrDefault(it => it.character == character);

            if (characterUpgrades.skillsCostCurrency.Length <= skill || skillCosts.Length <= upgrade)
                return (int.MaxValue, GemsType.Silver);


            return (skillCosts[upgrade], characterUpgrades.skillsCostCurrency[skill]);
        }

        [Serializable]
        public struct CharacterUpgrades
        {
            public GemsType[] skillsCostCurrency;
            public CharacterType character;
        }
    }
}
