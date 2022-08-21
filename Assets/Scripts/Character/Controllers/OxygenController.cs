using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace RuinsRaiders
{
    public class OxygenController : MonoBehaviour
    {

        public HashSet<WaterTile> waterTiles = new();

        public UnityEvent onInWater = new();
        public UnityEvent onOutWater = new();

        public bool inWater { get => waterTiles.Count > 0; }
        public int oxygen = 4;
        public int maxOxygen = 4;

        [SerializeField]
        private float oxygenTime;

        [SerializeField]
        private int damageWithoutOxygen = 1;

        private float _oxygenTimeLeft;
        private HealthController _healthController;

        private void Start()
        {
            _healthController = GetComponent<HealthController>();
        }

        private void FixedUpdate()
        {
            if (inWater == false || _healthController == null || _healthController.IsDead)
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