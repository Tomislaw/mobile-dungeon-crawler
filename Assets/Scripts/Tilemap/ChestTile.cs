using UnityEngine;
using UnityEngine.Events;


namespace RuinsRaiders
{
    [DefaultExecutionOrder(100)]
    public class ChestTile : MonoBehaviour
    {
        private const string OpenAnimation = "Open";

        public UnityEvent onOpen;
        public bool isOpen;

        [SerializeField]
        private AdventureData.ChestData.Type type;

        private LevelEvents _levelEvents;
        private Animator _animator;

        private void Start()
        {
            if (_levelEvents == null)
                _levelEvents = FindObjectOfType<LevelEvents>();

            _animator = GetComponent<Animator>();

            if (isOpen)
            {
                _animator.Play(OpenAnimation);
                var collider = GetComponent<Collider2D>();
                collider.enabled = false;
            }

        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (_levelEvents == null)
                return;

            var character = collision.gameObject.GetComponent<PlayerController>();
            if (character)
            {
                _animator.Play("Open");
                onOpen.Invoke();
                var collider = GetComponent<Collider2D>();
                collider.enabled = false;
                isOpen = true;
            }
        }
    }
}