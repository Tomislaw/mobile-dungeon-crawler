using UnityEngine;

namespace RuinsRaiders.UI
{
    // responsible for showing opening chest animation after level is finished
    // todo
    [RequireComponent(typeof(Animator))]
    public class ChestUI : MonoBehaviour
    {
        public enum ChestType
        {
            Normal, Expensive
        }

        public enum ChestState
        {
            Empty, AlreadyOpen, Normal
        }

        public ChestState State;
        public ChestType Type;
        public bool IsOpen;

        private Animator animator;

        private void Start()
        {
            animator = GetComponent<Animator>();
            animator.SetBool("Open", IsOpen);
            animator.SetInteger("Style", (int)Type);
            animator.SetInteger("State", (int)State);
        }

        public void Update()
        {
            animator.SetBool("Open", IsOpen);
            animator.SetInteger("Style", (int)Type);
            animator.SetInteger("State", (int)State);
        }

        public void OnOpen()
        {
        }
    }
}