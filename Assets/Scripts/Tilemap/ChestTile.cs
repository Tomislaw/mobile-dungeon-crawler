using UnityEngine;
using UnityEngine.Events;


namespace RuinsRaiders
{
    [DefaultExecutionOrder(100)]
    public class ChestTile : MonoBehaviour
    {
        public UnityEvent OnOpen;
        public bool IsOpen;

        [SerializeField]
        private AdventureData.ChestData.Type type;

        private LevelEvents levelEvents;
        private Animator animator;

        private void Start()
        {
            if (levelEvents == null)
                levelEvents = FindObjectOfType<LevelEvents>();

            animator = GetComponent<Animator>();

            if (IsOpen)
            {
                animator.Play("Open");
                var collider = GetComponent<Collider2D>();
                collider.enabled = false;
            }

        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (levelEvents == null)
                return;

            var character = collision.gameObject.GetComponent<PlayerController>();
            if (character)
            {
                animator.Play("Open");
                OnOpen.Invoke();
                var collider = GetComponent<Collider2D>();
                collider.enabled = false;
                IsOpen = true;
            }
        }
    }
}