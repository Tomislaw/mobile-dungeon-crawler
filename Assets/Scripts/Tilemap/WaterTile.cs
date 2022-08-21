using UnityEngine;

namespace RuinsRaiders
{
    public class WaterTile : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D collision)
        {
            var character = collision.gameObject.GetComponent<OxygenController>();
            if (character)
            {
                character.waterTiles.Add(this);
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            var character = collision.gameObject.GetComponent<OxygenController>();
            if (character)
            {
                character.waterTiles.Remove(this);
            }
        }
    }
}