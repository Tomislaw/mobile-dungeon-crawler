using System.Collections.Generic;
using UnityEngine;

namespace RuinsRaiders
{
    public class ItemsController : MonoBehaviour
    {
        public float itemSpeed = 5;
        public int itemsInRow = 5;

        [Header("Key")]
        public float keyHoverHeight = 2;
        public float keyHoverVariationDistance = 0.5f;
        private int _numberOfKeys = 0;
        private readonly List<GameObject> _keys = new();
        public bool HaveKeys { get => _numberOfKeys > 0; }

        [Header("Gem")]
        public float gemHoverHeight = 1;
        public float gemHoverVariationDistance = 1f;
        public float gemLifetime = 1f;
        public PlayerData.Gems gems;
        private readonly List<MutablePair<GameObject, float>> _gems = new();

        public void AddKey(GameObject key)
        {
            _keys.Add(key);
            _numberOfKeys++;
        }

        public void AddGems(GameObject gem, PlayerData.Gems amount)
        {
            gems += amount;
            _gems.Add(new MutablePair<GameObject, float>(gem, gemLifetime));
        }

        public void RemoveKey()
        {
            if (_numberOfKeys > 0)
            {
                var key = _keys[_numberOfKeys - 1];
                _keys.Remove(key);
                Destroy(key);
            }

            _numberOfKeys--;
        }

        public void FixedUpdate()
        {
            for (int i = 0; i < _keys.Count; i++)
            {
                var row = i / itemsInRow;
                var rowItems = row == _keys.Count / itemsInRow ? _keys.Count % itemsInRow : itemsInRow;

                var offset = Mathf.Sin(((Time.fixedTime + i / 2f) % 1f) * Mathf.PI) * keyHoverVariationDistance;
                var yPos = keyHoverHeight + offset + row;
                var xPos = i% itemsInRow - rowItems / 2f + 0.5f;

                _keys[i].transform.position = Vector3.Lerp(
                    _keys[i].transform.position,
                    transform.position + new Vector3(xPos, yPos),
                    itemSpeed * Time.fixedDeltaTime);
            }

            for (int i = 0; i < _gems.Count;)
            {
                if (_gems[i].Second <= 0)
                {
                    Destroy(_gems[i].First);
                    _gems.RemoveAt(i);
                    continue;
                }
                var row = i / itemsInRow;
                var rowItems = row == _gems.Count / itemsInRow ? _gems.Count % itemsInRow : itemsInRow;

                var factor = (_gems[i].Second / gemLifetime);
                var yPos = gemHoverHeight + factor * (gemHoverVariationDistance + row);
                var xPos = Mathf.Lerp(0f, i % itemsInRow - rowItems / 2f + 0.5f, factor);

                _gems[i].First.transform.position = Vector3.Lerp(
                    _gems[i].First.transform.position,
                    transform.position + new Vector3(xPos, yPos),
                    itemSpeed * Time.fixedDeltaTime);
                _gems[i].Second -= Time.fixedDeltaTime;
                i++;
            }
        }

    }
}