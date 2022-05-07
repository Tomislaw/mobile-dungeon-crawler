
using Assets.Scripts.Character.Ai;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "AreaTriggerActivator", menuName = "RuinsRaiders/Ai/AreaTriggerActivator", order = 1)]
public class AreaTriggerActivator : BasicAiActivatorData
{

    public Vector2 TriggerArea;
    public bool Raycast;
    public float triggerDelay;
    public List<HealthController.Group> Activators;

    public override BasicAiActivator Create(GameObject gameObject)
    {
        return new Activator(this,gameObject);
    }
    public class Activator : BasicAiActivator
    {
        public GameObject Target;
        private AreaTriggerActivator data;
        private float timeToNextTrigger;

        public Activator(AreaTriggerActivator data, GameObject target)
        {
            this.data = data;
            timeToNextTrigger = 0;
            Target = target;

        }
        public override ActivatorData? Triggered()
        {
            timeToNextTrigger -= Time.fixedDeltaTime;
            if (timeToNextTrigger > 0)
                return null;
            timeToNextTrigger = data.triggerDelay;

            var raycast = Physics2D.OverlapBoxAll(Target.transform.position,data.TriggerArea*2, 0);
            foreach (var go in raycast)
            {
                var target = go.gameObject.GetComponent<HealthController>();

                if (target != null && !target.IsDead && data.Activators.Contains(target.group))
                {
                    if (this.data.Raycast)
                        foreach (var item in Physics2D.LinecastAll(Target.transform.position + new Vector3(0,0.95f,0), 
                            go.gameObject.transform.position))
                        {
                            if (item.collider is TilemapCollider2D)
                                return null;
                        }

                    ActivatorData data;
                    data.triggeredBy = go.gameObject;
                    data.triggeredFor = Target;

                    return data;
                }
            }
            return null;
        }
    }
}

