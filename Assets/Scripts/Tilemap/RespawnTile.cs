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
        Debug.Log("Found scenes:" + scenes.Length);
        foreach (var scene in scenes)
        {
            Debug.Log(scene.respawnPoint + " - " + scene.Contains(transform.position));
            if (scene.respawnPoint == null && scene.Contains(transform.position))
            {
                Debug.Log(scene.transform.position);
                return scene;
            }
                
        }

        return null;
    }
}
    