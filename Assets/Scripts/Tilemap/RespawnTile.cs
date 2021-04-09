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
        GameObject scene = gameObject;
        do
        {
            scene = scene.transform?.parent?.gameObject;
            var subscene = scene.GetComponent<SubScene>();
            if (subscene == null)
                continue;
            return subscene;

        }
        while (scene != null);

        return null;
    }
}
    