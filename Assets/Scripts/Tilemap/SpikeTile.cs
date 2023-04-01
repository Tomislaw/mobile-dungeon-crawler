using System.Linq;
using UnityEngine;

namespace RuinsRaiders
{
    public class SpikeTile : MonoBehaviour
    {
        [SerializeField]
        private int damageToPlayer = 2;

        [SerializeField]
        private int damageToNpc = 999;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            var healthController = collision.gameObject.GetComponent<HealthController>();
            if (healthController == null || healthController.IsDead)
                return;

            var player = collision.gameObject.GetComponent<PlayerController>();
            var character = collision.gameObject.GetComponent<Character>();

            if (character != null && character.holdUpdate == true)
                return;

            if (player != null)
            {

                var respawn = transform.FindRespawnPosition();
                if (respawn == null)
                {
                    healthController.DamageIgnoreShield(damageToNpc, gameObject);
                }
                else
                {
                    var movementController = collision.gameObject.GetComponent<MovementController>();
                    healthController.DamageIgnoreShield(damageToPlayer, gameObject);
                    if (character != null && !character.IsDead)
                        movementController.Teleport(respawn.transform.position);
                }

            }
            else
            {
                healthController.DamageIgnoreShield(damageToNpc, gameObject);
            }
        }

    }
}