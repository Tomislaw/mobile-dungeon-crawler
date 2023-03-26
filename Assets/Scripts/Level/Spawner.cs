using System.Collections.Generic;
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

        [SerializeField]
        private int maxReplicas = -1;

        private List<GameObject> _spawnedObjects = new();

        public void Spawn()
        {
            if(maxReplicas >= 0)
            {
                _spawnedObjects.RemoveAll(it =>
                {
                    if(it==null)
                        return true;

                    var healthController = it.GetComponent<HealthController>();
                    return healthController != null && healthController.IsDead;
                }
                );

                if (_spawnedObjects.Count >= maxReplicas)
                    return;
            }

            var spawn = Instantiate(prefab, transform.position, Quaternion.identity);
            _spawnedObjects.Add(spawn);

            if (preserveScale)
                spawn.transform.localScale = transform.localScale;
            onSpawn.Invoke();
        }

    }
}