using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SpikeTile : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        var healthController = collision.gameObject.GetComponent<HealthController>();
        if (healthController == null || healthController.IsDead)
            return;

        var player = collision.gameObject.GetComponent<PlayerController>();
        var character = collision.gameObject.GetComponent<Character>();

        if (character?.holdUpdate == true)
            return;

        if(player != null)
        {
            var respawn = FindRespawnPosition();
            if (respawn == null)
            {
                healthController.Damage(9999, gameObject);
            }
            else
            {
                var movementController = collision.gameObject.GetComponent<MovementController>();
                healthController.Damage(2, gameObject);
                if (!character.IsDead)
                    movementController.Teleport(respawn.transform.position);
            }
            
        }
        else
        {
            healthController.Damage(9999, gameObject);
        }
    }

    private GameObject FindRespawnPosition()
    {
        var scenes = FindObjectsOfType<SubScene>().OrderBy(it => Vector3.Distance(it.transform.position, transform.position));
        if(scenes.Count() > 0)
            return scenes.First().respawnPoint;
        return FindObjectsOfType<RespawnTile>().OrderBy(it => Vector3.Distance(it.transform.position, transform.position)).First().gameObject;
    }
}