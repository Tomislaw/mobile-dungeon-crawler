using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace RuinsRaiders
{
    public class OxygenController : MonoBehaviour
    {
        public int oxygen = 4;
        public int maxOxygen = 4;

        public float headHeight = 0.8f;

        [SerializeField]
        private float oxygenTime;

        [SerializeField]
        private int damageWithoutOxygen = 1;

        private float _oxygenTimeLeft;
        private HealthController _healthController;
        private MovementController _movementController;

        public bool IsUnderwater
        {
            get
            {
                if (_movementController == null || !_movementController.IsSwimming)
                    return false;

                var headPosition = gameObject.transform.position + new Vector3(0, headHeight);

                foreach(var water in _movementController.waters)
                {
                    if (water.transform.position.y + 0.5 >= headPosition.y)
                        return true;
                }

                return false;
            }
        }

        private void Start()
        {
            _healthController = GetComponent<HealthController>();
            _movementController = GetComponent<MovementController>();
        }

        private void FixedUpdate()
        {
            if (IsUnderwater == false || _healthController == null || _healthController.IsDead)
            {
                oxygen = maxOxygen;
                _oxygenTimeLeft = oxygenTime;
                return;
            }


            _oxygenTimeLeft -= Time.fixedDeltaTime;

            if (_oxygenTimeLeft < 0)
            {
                _oxygenTimeLeft = oxygenTime;
                oxygen -= 1;
            }

            if(oxygen < 0)
            {
                oxygen = 0;
                _healthController.Damage(damageWithoutOxygen, null);
            }
        }
    }


}