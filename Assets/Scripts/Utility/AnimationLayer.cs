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

        private Animator _animator;
        void Start()
        {
            _animator = GetComponent<Animator>();
            _animator.SetLayerWeight(layerIndex, weight);
        }

    }
}