
using Assets.Scripts.Character.Ai;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AreaTriggerActivator", menuName = "RuinsRaiders/Ai/AreaTriggerActivator", order = 1)]
public class AreaTriggerActivator : BasicAiActivatorData
{

    public Vector2 TriggerArea;
    public bool Raycast;
    public float triggerDelay;
    public List<Character.Group> Activators;

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
                var ch = go.gameObject.GetComponent<Character>();

                if (ch != null && !ch.IsDead && data.Activators.Contains(ch.group))
                {
                    ActivatorData data;
                    data.triggeredBy = go.gameObject;
                    data.triggeredFor = Target;

                    Debug.Log("Area trigger occured for "+ Target.name + " by " + go.gameObject.name);

                    return data;
                }
            }
            return null;
        }
    }
}

