using RuinsRaiders;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static RuinsRaiders.PlayerData;

namespace RuinsRaiders.UI {
    public class UpgradeUI : MonoBehaviour
    {
        [SerializeField]
        private PlayerData data;

        [SerializeField]
        private UpgradeCostData costData;

        private CharacterType type;

        [SerializeField]
        private UpgradeBarsUI[] skills;

        [SerializeField]
        private UpgradeButtonUI[] buttons;


        public bool CanUpdateSkill(int skillNumber)
        {
            int upgradeNumber = data.GetCharacterData(type).skills[skillNumber];
            var cost = costData.GetCost(type, skillNumber, upgradeNumber);
            return data.GetGems(cost.Item2) >= cost.Item1;
        }

        public void UpdateSkill(int skillNumber)
        {
            if (!CanUpdateSkill(skillNumber))
                return;

            int upgradeNumber = data.GetCharacterData(type).skills[skillNumber];
            var cost = costData.GetCost(type, skillNumber, upgradeNumber);

            data.UpgradeSkill(type,skillNumber, cost.Item2,cost.Item1);
            skills[skillNumber].SetActiveBars(data.GetCharacterData(type).skills[skillNumber]);
        }

        private void Start()
        {
            for (int i = 0; i < skills.Length; i++)
                skills[i].SetActiveBars(data.GetCharacterData(type).skills[i]);

            for (int i = 0; i < buttons.Length; i++)
            {
                int id = i;
                buttons[id].OnPress.AddListener(() =>
                {
                    UpdateSkill(id);
                    UpdateButtons();
                });
            }


            UpdateButtons();

        }

        public void UpdateButtons()
        {
            for (int i = 0; i < buttons.Length; i++)
            {
                int upgradeNumber = data.GetCharacterData(type).skills[i];
                var cost = costData.GetCost(type, i, upgradeNumber);
                buttons[i].SetData(data.GetGems(cost.Item2) >= cost.Item1, cost.Item1);
            }

        }

    }
}