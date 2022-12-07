using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RuinsRaiders
{
    public class ColliderContainer : MonoBehaviour
    {
        private HashSet<Collider2D> colliders = new HashSet<Collider2D>();
        public HashSet<Collider2D> GetColliders() { return colliders; }

        private Collider2D _collider;

        private void OnTriggerEnter2D(Collider2D other)
        {
            colliders.Add(other); //hashset automatically handles duplicates
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            colliders.Remove(other);
        }

        public Vector3 GetRandomPoint()
        {
            Bounds b = _collider.bounds;

            var target = new Vector3(
                Random.Range(b.min.x, b.max.x),
                Random.Range(b.min.y, b.max.y),
                Random.Range(b.min.z, b.max.z)
            );
            return b.ClosestPoint(target);
        }

        private void Start()
        {
            _collider = GetComponent<Collider2D>();
        }

    }
}