using UnityEngine;

namespace RuinsRaiders.UI
{
    // responsible for showing opening chest animation after level is finished
    // todo
    [RequireComponent(typeof(Animator))]
    public class ChestUI : MonoBehaviour
    {
        public ChestState state;
        public ChestType type;
        public bool isOpen;

        private Animator _animator;

        private void Start()
        {
            _animator = GetComponent<Animator>();
            _animator.SetBool("Open", isOpen);
            _animator.SetInteger("Style", (int)type);
            _animator.SetInteger("State", (int)state);
        }

        public void Update()
        {
            _animator.SetBool("Open", isOpen);
            _animator.SetInteger("Style", (int)type);
            _animator.SetInteger("State", (int)state);
        }

        public void OnOpen()
        {
        }

        public enum ChestType
        {
            Normal, Expensive
        }

        public enum ChestState
        {
            Empty, AlreadyOpen, Normal
        }
    }
}