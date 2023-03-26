using UnityEngine;

namespace RuinsRaiders
{
    public class DestroyTrigger : MonoBehaviour
    {

        private void OnCollisionEnter2D(Collision2D collision)
        {
            var health = collision.gameObject.GetComponent<HealthController>();
            if (health != null)
                health.Damage(int.MaxValue, null);

            Destroy(collision.gameObject);
        }
        private void OnTriggerEnter2D(Collider2D collision)
        {
            var health = collision.gameObject.GetComponent<HealthController>();
            if (health != null)
                health.Damage(int.MaxValue, null);

            Destroy(collision.gameObject);
        }

    }
}
