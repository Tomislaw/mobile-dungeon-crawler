using UnityEngine;

namespace RuinsRaiders
{

    // Class used for holding different items by NPCs
    // For now it saying how many keys npc is having
    // Keys are used for opening doors by player
    public class InventoryController : MonoBehaviour
    {
        [SerializeField]
        private int NumberOfKeys = 1;
        [SerializeField]
        private AudioClip PickupSound;


        private AudioSource audioSource;
        void Start()
        {
            // When killed, add key to player
            var health = GetComponent<HealthController>();
            if (health != null)
                health.OnDeath.AddListener(AddItemsToPlayer);

            audioSource = GetComponent<AudioSource>();
        }

        public void AddItemsToPlayer()
        {
            var items = FindObjectOfType<ItemsController>();

            while (NumberOfKeys > 0)
            {
                NumberOfKeys--;
                items.AddKey();
            }

            if (audioSource)
                audioSource.PlayOneShot(PickupSound);

        }
    }
}