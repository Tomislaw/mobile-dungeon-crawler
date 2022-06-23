using UnityEngine;
using UnityEngine.Events;

namespace RuinsRaiders
{
    public class Spawner : MonoBehaviour
    {
        public UnityEvent onSpawn;

        [SerializeField]
        private GameObject prefab;

        [SerializeField]
        private bool preserveScale;

        public void Spawn()
        {
            var spawn = Instantiate(prefab, transform.position, Quaternion.identity);
            if (preserveScale)
                spawn.transform.localScale = transform.localScale;
            onSpawn.Invoke();
        }

    }
}