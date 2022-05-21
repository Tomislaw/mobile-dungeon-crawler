using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class ItemsController : MonoBehaviour
{
    public GameObject keyPrefab;

    public float HoverHeight = 2;
    public float HoverVariationDistance = 0.5f;
    public float ItemSpeed = 10;
    public int NumberOfKeys = 0;

    private List<GameObject> keys = new List<GameObject>();

    public void AddKey()
    {
        var key = Instantiate(keyPrefab);
        key.transform.position = transform.position;
        keys.Add(key);
        NumberOfKeys++;
    }

    public void RemoveKey()
    {
        if (NumberOfKeys > 0)
        {
            var key = keys[NumberOfKeys - 1];
            keys.Remove(key);
            Destroy(key.gameObject);
        }
          
        NumberOfKeys--;
    }

    public void FixedUpdate()
    {
        if (NumberOfKeys > keys.Count && keys.Count < 9)
            while (NumberOfKeys != keys.Count && keys.Count < 9)
            {
                var key = Instantiate(keyPrefab);
                key.transform.position = transform.position;
                keys.Add(key);
            }

        if (NumberOfKeys != 0 && NumberOfKeys < keys.Count)
            while (NumberOfKeys != keys.Count && keys.Count > 0)
            {
                var key = keys[NumberOfKeys - 1];
                keys.Remove(key);
                Destroy(key.gameObject);
            }

        for (int i = 0; i < keys.Count; i++)
        {
            var offset = Mathf.Sin(((Time.fixedTime + i / 2f) % 1f) * Mathf.PI) * HoverVariationDistance;
            keys[i].transform.position = Vector3.Lerp(
                keys[i].transform.position,
                transform.position + new Vector3(-i - 1, HoverHeight + offset),
                ItemSpeed * Time.fixedDeltaTime);
        }
    }

}
