using UnityEngine;

namespace RuinsRaiders
{

    // Class used for holding different items by NPCs
    // For now it saying how many keys npc is having
    // Keys are used for opening doors by player
    public class InventoryController : MonoBehaviour
    {
        [SerializeField]
        private int numberOfKeys = 1;
        [SerializeField]
        private AudioClip pickupSound;

        private AudioSource _audioSource;

        void Start()
        {
            // When killed, add key to player
            var health = GetComponent<HealthController>();
            if (health != null)
                health.onDeath.AddListener(AddItemsToPlayer);

            _audioSource = GetComponent<AudioSource>();
        }

        public void AddItemsToPlayer()
        {
            var items = FindObjectOfType<ItemsController>();

            while (numberOfKeys > 0)
            {
                numberOfKeys--;
                items.AddKey();
            }

            if (_audioSource)
                _audioSource.PlayOneShot(pickupSound);

        }
    }
}