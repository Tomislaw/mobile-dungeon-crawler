using UnityEngine;

namespace RuinsRaiders
{

    // Class used for holding different items by NPCs

    public class InventoryController : MonoBehaviour
    {
        public int numberOfKeys = 0;
        public PlayerData.Gems gems;

        public GameObject keyPrefab;
        public PickupItem gemPrefab;

        public AudioClip pickupSound;

        private AudioSource _audioSource;

        private Rigidbody2D _body2d;

        void Start()
        {
            // When killed, add key to player
            var health = GetComponent<HealthController>();
            if (health != null)
                health.onDeath.AddListener(DropItems);

            _audioSource = GetComponent<AudioSource>();
            _body2d = GetComponent<Rigidbody2D>();
        }

        public void DropItems()
        {
            AddKeysToPlayer();
            DropGems();
        }

        private void AddKeysToPlayer()
        {
            var items = FindObjectOfType<ItemsController>();

            while (numberOfKeys > 0)
            {
                numberOfKeys--;
                GameObject key = Instantiate(keyPrefab);
                key.transform.position = transform.position;
                items.AddKey(key);
            }

            if (_audioSource)
                _audioSource.PlayOneShot(pickupSound);

        }

        private void DropGems()
        {
            while (gems.greenGems > 0)
            {
                gems.greenGems--;
                DropGem(PlayerData.GemsType.Green);
            }
            while (gems.blueGems > 0)
            {
                gems.blueGems--;
                DropGem(PlayerData.GemsType.Blue);
            }
            while (gems.silverGems > 0)
            {
                gems.silverGems--;
                DropGem(PlayerData.GemsType.Silver);
            }
            while (gems.redGems > 0)
            {
                gems.redGems--;
                DropGem(PlayerData.GemsType.Red);
            }

        }

        private void DropGem(PlayerData.GemsType type)
        {
            gemPrefab.type = type;
            var gem = Instantiate(gemPrefab);
            gem.transform.position = transform.position;

            if (_body2d!= null && gem.Rigidbody2D!= null)
                gem.Rigidbody2D.velocity += _body2d.velocity;
        }
    }
}