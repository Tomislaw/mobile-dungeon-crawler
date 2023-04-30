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

        public int slopeRollDamageToPlayer = 1;
        public int fallDamageToPlayer = 2;

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
                if(health.transform.position.y <= transform.position.y)
                    health.Damage(health.group == HealthController.Group.Player ? fallDamageToPlayer : fallDamage, null);
            }

            if(_healthController != null)
                _healthController.Damage(fallDamage, null);

            foreach (var t in touchersToBeRemoved)
                touchers.Remove(t);

            touchersToBeRemoved.Clear();
        }

        private void RollDamage()
        {
            if(_box._accumulatedRoll == 0)
                return;

            foreach (var health in touchers)
            {
                if (_box._accumulatedRoll < 0 && health.transform.position.x > transform.position.x)
                    continue;
                else if(health.transform.position.x < transform.position.x)
                    continue;

                health.Damage(health.group == HealthController.Group.Player ? slopeRollDamageToPlayer : slopeRollDamage, null);

                if (_healthController != null)
                    _healthController.Damage(  slopeRollDamage, null);
            }

            foreach (var t in touchersToBeRemoved)
                touchers.Remove(t);

            touchersToBeRemoved.Clear();
        }

    }
}