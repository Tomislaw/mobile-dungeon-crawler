using UnityEngine;


public class RespawnTile : MonoBehaviour
{
    private void Start()
    {
        SubScene scene = FindSubscene();
        if (scene)
        {
            scene.respawnPoint = gameObject;
        }
    }

    private SubScene FindSubscene()
    {
        var scenes = FindObjectsOfType<SubScene>();
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
    