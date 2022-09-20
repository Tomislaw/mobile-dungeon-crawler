using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


namespace RuinsRaiders.AI
{
    [CreateAssetMenu(fileName = "AreaTriggerActivator", menuName = "RuinsRaiders/Ai/AreaTriggerActivator", order = 1)]
    public class AreaTriggerActivator : BasicAiActivatorData
    {
        [SerializeField]
        private Vector2 triggerArea;

        [SerializeField]
        private bool onlyTargetInSight;

        [SerializeField]
        private bool onlyTargetInWater;

        [SerializeField]
        private List<HealthController.Group> activators = new();

        [SerializeField]
        private float triggerDelay;

        public override BasicAiActivator Create(GameObject gameObject)
        {
            return new Activator(this, gameObject);
        }
        public class Activator : BasicAiActivator
        {
            public GameObject target;
            private readonly AreaTriggerActivator _data;
            private float _timeToNextTrigger;

            public Activator(AreaTriggerActivator data, GameObject target)
            {
                _data = data;
                _timeToNextTrigger = 0;
                this.target = target;

            }
            public override ActivatorData? Triggered()
            {
                _timeToNextTrigger -= Time.fixedDeltaTime;
                if (_timeToNextTrigger > 0)
                    return null;
                _timeToNextTrigger = _data.triggerDelay;

                var raycast = Physics2D.OverlapBoxAll(target.transform.position, _data.triggerArea * 2, 0);
                foreach (var go in raycast)
                {
                    // continue if target is dead or not in valid group
                    var target = go.gameObject.GetComponent<HealthController>();
                    if (target == null || target.IsDead || !_data.activators.Contains(target.group))
                        continue;

                    // continue if target is not swimming when looking only for characters in water
                    var movementController = target.GetComponent<MovementController>();
                    if (_data.onlyTargetInWater && movementController != null && !movementController.IsSwimming)
                        continue;

                    // if blocked by tilemap collider then it is not visible when looking for targets in sight
                    if (_data.onlyTargetInSight)
                        foreach (var item in Physics2D.LinecastAll(this.target.transform.position + new Vector3(0, 0.95f, 0),
                            go.gameObject.transform.position + new Vector3(0, 0.5f, 0)))
                        {
                            if (item.collider is TilemapCollider2D or CompositeCollider2D)
                                return null;
                        }

                    ActivatorData data;
                    data.triggeredBy = go.gameObject;
                    data.triggeredFor = this.target;

                    return data;
                }

                return null;
            }
        }
    }
}
