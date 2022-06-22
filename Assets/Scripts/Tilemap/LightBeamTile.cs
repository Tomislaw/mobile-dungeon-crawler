using UnityEngine;

namespace RuinsRaiders
{
    public class LightBeamTile : MonoBehaviour
    {
        [SerializeField]
        private LayerMask playerLayer;

        private LevelEvents levelEvents;

        private void Start()
        {
            if (levelEvents == null)
                levelEvents = FindObjectOfType<LevelEvents>();
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (levelEvents == null)
                return;

            var character = collision.gameObject.GetComponent<Character>();
            if (character == null)
                return;

            if (playerLayer == (playerLayer | (1 << collision.gameObject.layer)))
            {
                levelEvents.LevelFinished();
                StartCoroutine(character.HideCoroutine());
            }
        }
    }
}