using UnityEngine;

namespace RuinsRaiders
{
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