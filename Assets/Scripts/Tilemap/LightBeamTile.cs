using UnityEngine;

namespace RuinsRaiders
{
    public class LightBeamTile : MonoBehaviour
    {
        [SerializeField]
        private LayerMask playerLayer;

        private LevelEvents _levelEvents;

        private void Start()
        {
            if (_levelEvents == null)
                _levelEvents = FindObjectOfType<LevelEvents>();
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (_levelEvents == null)
                return;

            var character = collision.gameObject.GetComponent<Character>();
            if (character == null)
                return;

            if (playerLayer == (playerLayer | (1 << collision.gameObject.layer)))
            {
                _levelEvents.LevelFinished();
                StartCoroutine(character.HideCoroutine());
            }
        }
    }
}