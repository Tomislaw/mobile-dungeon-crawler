using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RuinsRaiders
{
    [RequireComponent(typeof(RollableBox))]
    public class RollableBoxDamageController : MonoBehaviour
    {
        public int slopeRollDamage = 2;
        public int fallDamage = 4;

        public MeeleAttackTrigger trigger;
        private RollableBox _box;

        private HashSet<HealthController> touchers = new();
        private HashSet<HealthController> touchersToBeRemoved = new();
        private HealthController _healthController;

        private void OnCollisionEnter2D(Collision2D collision)
        {
            var health = collision.gameObject.GetComponent<HealthController>();
            if (health != null)
            {
                touchers.Add(health);
            }

        }

        private void OnCollisionExit2D(Collision2D collision)
        {
            var health = collision.gameObject.GetComponent<HealthController>();
            if (health != null)
            {
                touchersToBeRemoved.Add(health);
            }
        }

        void Start()
        {
            _box = gameObject.GetComponent<RollableBox>();
            _box.OnSlopeRoll.AddListener(RollDamage);
            _box.OnFall.AddListener(FallDamage);
            _healthController = GetComponent<HealthController>();
        }

        private void FallDamage()
        {
            foreach(var health in touchers)
            {
                health.Damage(fallDamage, null);
            }

            if(_healthController != null)
                _healthController.Damage(fallDamage, null);
        }

        private void RollDamage()
        {
            foreach (var health in touchers)
            {
                health.Damage(slopeRollDamage, null);
                if (_healthController != null)
                    _healthController.Damage(slopeRollDamage, null);
            }
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            if(_box.IsFalling! && !_box.IsRolling && touchersToBeRemoved.Count > 0)
            {
                foreach(var t in touchersToBeRemoved)
                    touchers.Remove(t);

                touchersToBeRemoved.Clear();
            }
        }
    }
}