using System.Collections.Generic;
using UnityEngine;


namespace RuinsRaiders
{
    public class PlatformTile : MonoBehaviour
    {
        [SerializeField]
        private Collider2D platformCollider;

        private readonly HashSet<MovementController> _characters = new();

        private void Start()
        {
            if(platformCollider == null)
                platformCollider = GetComponent<Collider2D>();
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            var character = collision.gameObject.GetComponent<MovementController>();
            if (character == null)
                return;

            var colliders = character.GetComponentsInChildren<Collider2D>();

            if (character.canUsePlatform)
            {
                _characters.Add(character);
                foreach (var characterCollider in colliders)
                    Physics2D.IgnoreCollision(platformCollider, characterCollider, character.move.y < 0);
            }
            else
            {
                foreach (var characterCollider in colliders)
                    Physics2D.IgnoreCollision(platformCollider, characterCollider, true);
            }

        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            var character = collision.gameObject.GetComponent<MovementController>();
            if (character)
            {
                _characters.Remove(character);
            }
        }

        private void FixedUpdate()
        {
            foreach (var character in _characters)
            {
                var colliders = character.GetComponentsInChildren<Collider2D>();
                foreach (var characterCollider in colliders)
                    if (character.move.y < 0)
                        Physics2D.IgnoreCollision(platformCollider, characterCollider);
            }
        }
    }
}