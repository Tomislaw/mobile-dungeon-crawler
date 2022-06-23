using UnityEngine;

namespace RuinsRaiders.AI
{
    [CreateAssetMenu(fileName = "OnDamageActivator", menuName = "RuinsRaiders/Ai/OnDamageActivator", order = 1)]
    public class OnDamageActivator : BasicAiActivatorData
    {
        public override BasicAiActivator Create(GameObject gameObject)
        {
            return new Activator(gameObject);
        }
        public class Activator : BasicAiActivator
        {
            private readonly GameObject target = null;
            private GameObject damagedBy = null;

            public Activator(GameObject target)
            {
                this.target = target;
                target.GetComponent<HealthController>().onDamage.AddListener(Damaged);
            }

            private void Damaged(GameObject by)
            {
                var projectile = by.GetComponent<Projectile>();
                if (projectile && projectile.launcher != null)
                    damagedBy = projectile.launcher.gameObject;
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
}
