using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Spawner : MonoBehaviour
{
    public GameObject Prefab;
    public UnityEvent OnSpawn;

    public void Spawn()
    {
        var player = Instantiate(Prefab, transform.position, Quaternion.identity);
        OnSpawn.Invoke();
    }

}