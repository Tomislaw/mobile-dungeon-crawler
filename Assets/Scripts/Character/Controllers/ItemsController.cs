using System.Collections.Generic;
using UnityEngine;

namespace RuinsRaiders
{
    public class ItemsController : MonoBehaviour
    {
        public GameObject keyPrefab;

        public float hoverHeight = 2;
        public float hoverVariationDistance = 0.5f;
        public float itemSpeed = 10;
        public int numberOfKeys = 0;

        private readonly List<GameObject> _keys = new();

        public void AddKey()
        {
            var key = Instantiate(keyPrefab);
            key.transform.position = transform.position;
            _keys.Add(key);
            numberOfKeys++;
        }

        public void RemoveKey()
        {
            if (numberOfKeys > 0)
            {
                var key = _keys[numberOfKeys - 1];
                _keys.Remove(key);
                Destroy(key);
            }

            numberOfKeys--;
        }

        public void FixedUpdate()
        {
            if (numberOfKeys > _keys.Count && _keys.Count < 9)
                while (numberOfKeys != _keys.Count && _keys.Count < 9)
                {
                    var key = Instantiate(keyPrefab);
                    key.transform.position = transform.position;
                    _keys.Add(key);
                }

            if (numberOfKeys != 0 && numberOfKeys < _keys.Count)
                while (numberOfKeys != _keys.Count && _keys.Count > 0)
                {
                    var key = _keys[numberOfKeys - 1];
                    _keys.Remove(key);
                    Destroy(key);
                }

            for (int i = 0; i < _keys.Count; i++)
            {
                var offset = Mathf.Sin(((Time.fixedTime + i / 2f) % 1f) * Mathf.PI) * hoverVariationDistance;
                _keys[i].transform.position = Vector3.Lerp(
                    _keys[i].transform.position,
                    transform.position + new Vector3(-i - 1, hoverHeight + offset),
                    itemSpeed * Time.fixedDeltaTime);
            }
        }

    }
}