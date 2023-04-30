using System.Collections.Generic;
using UnityEngine;
using System.Linq;
namespace RuinsRaiders
{
    [DefaultExecutionOrder(-1000)]
    public class SubScene : MonoBehaviour
    {
        public int id = -1;
  
        public GameObject respawnPoint;

        public SubScene left;
        public SubScene right;
        public SubScene top;
        public SubScene bottom;

        public Vector2 size = new(32, 18);
        public Vector2 padding = new(1, 1);

        private List<(GameObject,bool)> characters;

        private void Awake()
        {
            characters = FindObjectsOfType<MovementController>()
                .Where(it => Contains(it.transform.position))
                .Select(it => (it.gameObject, it.gameObject.activeSelf))
                .ToList();

            foreach(var character in characters)
                character.Item1.SetActive(false);
        }

        public void CameraEneterd()
        {
            foreach (var character in characters)
                character.Item1.SetActive(character.Item2 | character.Item1.activeSelf);
            characters.Clear();
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = new Color(1, 0, 0, 0.5f);
            Gizmos.DrawWireCube(transform.position, new Vector3(size.x, size.y, 1));
            Gizmos.color = new Color(1, 1, 0, 0.5f);
            Gizmos.DrawWireCube(transform.position, new Vector3(size.x - padding.x, size.y - padding.y, 1));
        }

        public bool Contains(Vector2 position)
        {
            var left = -size.x / 2 + transform.position.x;
            var right = size.x / 2 + transform.position.x;
            var top = size.y / 2 + transform.position.y;
            var bottom = -size.y / 2 + transform.position.y;
            return position.x >= left && position.x <= right && position.y >= bottom && position.y <= top;
        }

    }
}