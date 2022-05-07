using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformTile : MonoBehaviour
{
    public Collider2D collider;

    private HashSet<MovementController> characters = new HashSet<MovementController>();

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var character = collision.gameObject.GetComponent<MovementController>();
        if (character)
        {
            characters.Add(character);
            var colliders = character.GetComponentsInChildren<Collider2D>();
            foreach (var character_collider in colliders)
                Physics2D.IgnoreCollision(collider, character_collider, character.move.y < 0);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        var character = collision.gameObject.GetComponent<MovementController>();
        if (character)
        {
            characters.Remove(character);
        }
    }

    private void FixedUpdate()
    {
        foreach (var character in characters)
        {
            var colliders = character.GetComponentsInChildren<Collider2D>();
            foreach (var character_collider in colliders)
                if (character.move.y < 0)
                    Physics2D.IgnoreCollision(collider, character_collider);
        }
    }
}