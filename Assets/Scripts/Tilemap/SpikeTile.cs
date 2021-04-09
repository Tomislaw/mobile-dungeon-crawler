using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeTile : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        var character = collision.gameObject.GetComponent<Character>();
        var player = collision.gameObject.GetComponent<PlayerController>();
        if (character.isTeleporting)
        {
            return;
        }
        if(player != null && !character.IsDead)
        {
            var respawn = FindRespawnPosition();
            if (respawn == null)
            {
                character.DamageController.Damage(9999, gameObject);
            }
            else
            {
                character.DamageController.Damage(2, gameObject);
                if (!character.IsDead)
                    StartCoroutine(character.Teleport(respawn.transform.position));
            }
            
        }
        else if (!character.IsDead)
        {
            character.DamageController.Damage(9999, gameObject);
        }
    }

    private GameObject FindRespawnPosition()
    {
        GameObject scene = gameObject;
        do
        {
            scene = scene.transform?.parent?.gameObject;
            var subscene  = scene.GetComponent<SubScene>();
            if (subscene == null)
                continue;
            return subscene.respawnPoint;

        }
        while (scene != null);

        return null;
    }
}