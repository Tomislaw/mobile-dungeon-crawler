using System.Linq;
using UnityEngine;

namespace RuinsRaiders
{
    public static class TileHelper
    {
        public static GameObject FindRespawnPosition(this Transform transform)
        {
            var scenes = Object.FindObjectsOfType<SubScene>().OrderBy(it => Vector3.Distance(it.transform.position, transform.position));
            if (scenes.Count() > 0)
                return scenes.First().respawnPoint;
            return Object.FindObjectsOfType<RespawnTile>().OrderBy(it => Vector3.Distance(it.transform.position, transform.position)).First().gameObject;
        }

        public static SubScene FindSubscene(this Transform transform)
        {
            var scenes = Object.FindObjectsOfType<SubScene>();
            foreach (var scene in scenes)
            {
                if (scene.respawnPoint == null && scene.Contains(transform.position))
                {
                    return scene;
                }
            }
            return null;
        }
    }
}
