using UnityEngine;

namespace RuinsRaiders
{
    // Helper function for setting animation layer weight via MonoBehavior
    [RequireComponent(typeof(Animator))]
    public class AnimationLayer : MonoBehaviour
    {
        [SerializeField]
        private float weight;
        [SerializeField]
        private int layerIndex;

        private Animator animator;
        void Start()
        {
            animator = GetComponent<Animator>();
            animator.SetLayerWeight(layerIndex, weight);
        }

    }
}