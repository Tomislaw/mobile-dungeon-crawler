using UnityEngine;

namespace RuinsRaiders
{
    public class RespawnTile : MonoBehaviour
    {
        private void Start()
        {
            SubScene scene = transform.FindSubscene();
            if (scene)
            {
                scene.respawnPoint = gameObject;
            }
        }
    }
}