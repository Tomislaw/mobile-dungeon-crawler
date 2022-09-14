using UnityEngine;

namespace RuinsRaiders
{
    public class WaterTile : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D collision)
        {
            var character = collision.gameObject.GetComponent<MovementController>();
            if (character)
            {
                character.waters.Add(this);
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            var character = collision.gameObject.GetComponent<MovementController>();
            if (character)
            {
                character.waters.Remove(this);
            }
        }
    }
}