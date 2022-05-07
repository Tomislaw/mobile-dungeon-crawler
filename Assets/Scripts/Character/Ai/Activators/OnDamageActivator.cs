
using Assets.Scripts.Character.Ai;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "OnDamageActivator", menuName = "RuinsRaiders/Ai/OnDamageActivator", order = 1)]
public class OnDamageActivator : BasicAiActivatorData
{

    public override BasicAiActivator Create(GameObject gameObject)
    {
        return new Activator(this, gameObject);
    }
    public class Activator : BasicAiActivator
    {
        private GameObject target = null;
        private GameObject damagedBy = null;
        private OnDamageActivator data;

        public Activator(OnDamageActivator data, GameObject target)
        {
            this.data = data;
            this.target = target;
            target.GetComponent<HealthController>().OnDamage.AddListener(Damaged);
        }

        private void Damaged(GameObject by)
        {
            var projectile = by.GetComponent<Projectile>();
            if (projectile)
                damagedBy = projectile.Launcher.gameObject;
            else
                damagedBy = by;
        }
        public override ActivatorData? Triggered()
        {
            if (damagedBy == null)
                return null;

            ActivatorData data;
            data.triggeredBy = damagedBy;
            data.triggeredFor = target;
            damagedBy = null;
            return data;
        }
    }
}

