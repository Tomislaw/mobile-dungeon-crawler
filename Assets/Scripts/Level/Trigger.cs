using UnityEngine;
using UnityEngine.Events;

namespace RuinsRaiders
{
    public class Trigger : MonoBehaviour
    {
        public UnityEvent onStart;
        public UnityEvent onCollisionEnter;
        public UnityEvent onCollisionExit;
        public UnityEvent onTriggerEnter;
        public UnityEvent onTriggerExit;
        public UnityEvent onKilled;

        void Start()
        {
            onStart.Invoke();
        }

        // Update is called once per frame
        private void OnCollisionEnter2D(Collision2D collision)
        {
            onCollisionEnter.Invoke();
        }

        private void OnCollisionExit2D(Collision2D collision)
        {
            onCollisionExit.Invoke();
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            onTriggerEnter.Invoke();
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            onTriggerExit.Invoke();
        }

        private void OnDestroy()
        {
            onKilled.Invoke();
        }
    }
}