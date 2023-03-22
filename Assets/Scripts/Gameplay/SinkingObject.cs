using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RuinsRaiders
{
    public class SinkingObject : MonoBehaviour
    {
        private const string IdleAnim = "Idle";
        private const string SinkAnim = "Sink";
        private const string RiseAnim = "Rise";

        public ColliderContainer colliderContainer;

        [SerializeField]
        private float timeToRefreshAfterStartingSinking;

        private float _timeToRefreshLeft;

        private bool _previousState;

        private Animator _animator;
        void Start()
        {
            _animator = GetComponent<Animator>();
            _previousState = false;
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            if(_timeToRefreshLeft > 0)
            {
                _timeToRefreshLeft -= Time.fixedDeltaTime;
                return;
            }

            bool sinking = colliderContainer.GetColliders().Count > 0;
            if (sinking != _previousState)
            {
                if (sinking)
                    _timeToRefreshLeft = timeToRefreshAfterStartingSinking;

                var currentAnim = _animator.GetCurrentAnimatorClipInfo(0)[0];
                var currentState = _animator.GetCurrentAnimatorStateInfo(0);
                switch (currentAnim.clip.name)
                {
                    case IdleAnim:
                        if (sinking)
                            _animator.Play(SinkAnim, 0, 0f);
                        break;
                    case SinkAnim:
                        if (!sinking)
                            _animator.Play(RiseAnim, 0, 1f - currentState.normalizedTime);
                        break;
                    case RiseAnim:
                        if (sinking)
                            _animator.Play(SinkAnim, 0, 1f - currentState.normalizedTime);
                        break;
                }
                _previousState = sinking;

            }
        }
    }
}