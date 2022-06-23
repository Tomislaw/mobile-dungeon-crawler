using UnityEngine;

namespace RuinsRaiders
{
    public class LadderTile : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D collision)
        {
            var character = collision.gameObject.GetComponent<MovementController>();
            if (character && character.canUseLadder)
            {
                character.ladders.Add(this);
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            var character = collision.gameObject.GetComponent<MovementController>();
            if (character && character.canUseLadder)
            {
                character.ladders.Remove(this);
            }
        }
    }
}