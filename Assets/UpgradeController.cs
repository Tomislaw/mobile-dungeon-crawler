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


        private ShieldedHealthController _shieldController;
        private HealthController _healthController;

        private RangedAttack _rangedAttack;
        private MeeleAttack _meeleAttack;
        private RaycastAttack _raycastAttack;


        void Start()
        {
            _shieldController = GetComponent<ShieldedHealthController>();
            _healthController = GetComponent<HealthController>();

            _rangedAttack = GetComponent<RangedAttack>();
            _raycastAttack = GetComponent<RaycastAttack>();
            _meeleAttack = GetComponent<MeeleAttack>();

            var data = playerData.GetCharacterData(characterType);

            if (_shieldController != null)
            {
                _shieldController.health += healthIncreasePerLevel * data.skills[0];
                _shieldController.maxHealth += healthIncreasePerLevel * data.skills[0];

                _shieldController.shield += shieldIncreasePerLevel * data.skills[0];
                _shieldController.maxShield += shieldIncreasePerLevel * data.skills[0];
            }
            if (_healthController != null)
            {
                _healthController.health += healthIncreasePerLevel * data.skills[0];
                _healthController.maxHealth += healthIncreasePerLevel * data.skills[0];
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
                //_rangedAttack.attackDamage += attackDamageIncreasePerLevel * data.skills[1];

                _rangedAttack.overchargeTime -= overchargeTimeDecreasePerLevel * data.skills[2];
            }
            if (_raycastAttack != null)
            {
                _raycastAttack.attackSpeed -= attackSpeedDecreasePerLevel * data.skills[1];
                _raycastAttack.damage += attackDamageIncreasePerLevel * data.skills[1];

                _raycastAttack.overchargeTime -= overchargeTimeDecreasePerLevel * data.skills[2];
                _raycastAttack.specialDamage += specialAttackDamageIncreasePerLevel * data.skills[1];
            }


        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}