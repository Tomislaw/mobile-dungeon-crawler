using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RuinsRaiders {
    public class UpgradeController : MonoBehaviour
    {
        public PlayerData playerData;
        public PlayerData.CharacterType characterType;

        [Header("Skill 1")]
        public int shieldIncreasePerLevel;
        public int healthIncreasePerLevel;

        [Header("Skill 2")]
        public int attackDamageIncreasePerLevel;
        public float attackSpeedDecreasePerLevel;

        [Header("Skill 3")]
        public int specialAttackDamageIncreasePerLevel;
        public int specialAttackRangeIncreasePerLevel;
        public float overchargeTimeDecreasePerLevel;

        private HealthController _healthController;

        private RangedAttack _rangedAttack;
        private MeeleAttack _meeleAttack;
        private RaycastAttack _raycastAttack;
        private Shockwave _shockwave;


        void Start()
        {
            _healthController = GetComponent<HealthController>();

            _rangedAttack = GetComponentInChildren<RangedAttack>();
            _raycastAttack = GetComponentInChildren<RaycastAttack>();
            _meeleAttack = GetComponentInChildren<MeeleAttack>();
            _shockwave = GetComponentInChildren<Shockwave>();

            var data = playerData.GetCharacterData(characterType);


            if (_healthController != null)
            {
                _healthController.health += healthIncreasePerLevel * data.skills[0];
                _healthController.maxHealth += healthIncreasePerLevel * data.skills[0];

                _healthController.shield += shieldIncreasePerLevel * data.skills[0];
                _healthController.maxShield += shieldIncreasePerLevel * data.skills[0];
            }
            if(_meeleAttack != null)
            {
                _meeleAttack.attackSpeed -= attackSpeedDecreasePerLevel * data.skills[1];
                _meeleAttack.attackDamage += attackDamageIncreasePerLevel * data.skills[1];

                _meeleAttack.overchargeTime -= overchargeTimeDecreasePerLevel * data.skills[2];
            }
            if (_rangedAttack != null)
            {
                _rangedAttack.attackSpeed -= attackSpeedDecreasePerLevel * data.skills[1];
                _rangedAttack.attackDamage += attackDamageIncreasePerLevel * data.skills[1];

                _rangedAttack.overchargeTime -= overchargeTimeDecreasePerLevel * data.skills[2];
                _rangedAttack.specialAttackDamage += specialAttackDamageIncreasePerLevel * data.skills[2];
            }
            if (_raycastAttack != null)
            {
                _raycastAttack.attackSpeed -= attackSpeedDecreasePerLevel * data.skills[1];
                _raycastAttack.attackDamage += attackDamageIncreasePerLevel * data.skills[1];

                _raycastAttack.overchargeTime -= overchargeTimeDecreasePerLevel * data.skills[2];
                _raycastAttack.specialAttackDamage += specialAttackDamageIncreasePerLevel * data.skills[2];
            }
            if (_shockwave != null)
            {
                _shockwave.shockDamage += specialAttackDamageIncreasePerLevel * data.skills[2];
                _shockwave.shockWidth += specialAttackRangeIncreasePerLevel * data.skills[2];
            }
        }

    }
}