using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Spawner : MonoBehaviour
{
    public GameObject Prefab;
    public UnityEvent OnSpawn;

    public bool preserveScale;

    public void Spawn()
    {
        var spawn = Instantiate(Prefab, transform.position, Quaternion.identity);
        if(preserveScale)
            spawn.transform.localScale = transform.localScale;
        OnSpawn.Invoke();
    }

}