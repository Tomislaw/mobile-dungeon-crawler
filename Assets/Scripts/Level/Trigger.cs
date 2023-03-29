using UnityEngine;
using UnityEngine.Events;

namespace RuinsRaiders
{
    public class Trigger : MonoBehaviour
    {
        public bool onlyPlayer = false;

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
            if(onlyPlayer && collision.gameObject.GetComponent<PlayerController>() == null)
                return;

            onCollisionEnter.Invoke();
        }

        private void OnCollisionExit2D(Collision2D collision)
        {
            if (onlyPlayer && collision.gameObject.GetComponent<PlayerController>() == null)
                return;

            onCollisionExit.Invoke();
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (onlyPlayer && collision.gameObject.GetComponent<PlayerController>() == null)
                return;

            onTriggerEnter.Invoke();
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (onlyPlayer && collision.gameObject.GetComponent<PlayerController>() == null)
                return;

            onTriggerExit.Invoke();
        }

        private void OnDestroy()
        {
            onKilled.Invoke();
        }
    }
}