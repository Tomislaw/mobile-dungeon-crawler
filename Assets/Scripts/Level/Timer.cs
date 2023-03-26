using UnityEngine;
using UnityEngine.Events;

namespace RuinsRaiders
{
    public class Timer : MonoBehaviour
    {
        [SerializeField]
        private float rate = 10;

        [SerializeField]
        private UnityEvent OnTimer = new();

        private float _timeLeft;

        private void Start()
        {
            _timeLeft = rate;
        }

        private void FixedUpdate()
        {
            _timeLeft -= Time.fixedDeltaTime;
            if(_timeLeft < 0)
            {
                _timeLeft = rate;
                OnTimer.Invoke();
            }
        }

    }
}
