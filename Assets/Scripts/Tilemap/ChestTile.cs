using UnityEngine;
using UnityEngine.Events;


namespace RuinsRaiders
{
    [DefaultExecutionOrder(100)]
    [RequireComponent(typeof(Animator))]
    public class ChestTile : MonoBehaviour
    {
        private const string OpenAnimation = "Open";

        public UnityEvent onOpen;
        public bool isOpen;

        public AdventureData.ChestData chestData;

        private Animator _animator;

        public void OnEnable()
        {
            Refresh();
        }

        public void Refresh()
        {
            if (LevelEvents.Instance == null)
                return;
            chestData = LevelEvents.Instance.GetChestData(this);
            isOpen = chestData.acquired;
            _animator = GetComponent<Animator>();
            _animator.SetLayerWeight((int)chestData.type, 1);

            _animator.SetBool("IsOpen", isOpen);
            if (isOpen)
            {
                var collider = GetComponent<Collider2D>();
                collider.enabled = false;
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            var character = collision.gameObject.GetComponent<PlayerController>();
            if (character)
            {
                _animator.SetBool("IsOpen", true);
                onOpen.Invoke();
                var collider = GetComponent<Collider2D>();
                collider.enabled = false;
                isOpen = true;
            }
        }
    }
}