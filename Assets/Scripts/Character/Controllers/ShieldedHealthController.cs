using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RuinsRaiders
{
    public class ShieldedHealthController : HealthController
    {
        public int shield = 1;
        public int maxShield = 1;

        public float regenerationTime = 10f;

        private float _regenerationTimeLeft;

        new protected void Start()
        {
            base.Start();
            _regenerationTimeLeft = regenerationTime;

        }

        // Update is called once per frame
        void FixedUpdate()
        {
            if (shield >= maxShield)
                return;

            _regenerationTimeLeft -= Time.fixedDeltaTime;

            if(_regenerationTimeLeft <= 0)
            {
                _regenerationTimeLeft = regenerationTime;
                shield++;
            }
        }

        public override void Damage(int damage, GameObject who)
        {
            if (_character != null && _character.holdUpdate == true)
                return;

            int damageLeft = Mathf.Max(0, damage - shield);
            shield -= damage;

            if (damageLeft > 0)
            {
                base.Damage(damageLeft, who);
                shield = 0;
            }

        }
    }
}