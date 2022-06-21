using UnityEngine;

public class SubScene : MonoBehaviour
{
    public int id = -1;
    public float width = 32;
    public float height = 18;

    public SubScene left;
    public SubScene right;
    public SubScene top;
    public SubScene bottom;

    public GameObject respawnPoint;

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1, 0, 0, 0.5f);
        Gizmos.DrawWireCube(transform.position, new Vector3(width, height, 1));
    }

    public bool Contains(Vector2 position)
    {
        var left = -width / 2 + transform.position.x;
        var right = width / 2 + transform.position.x;
        var top = height / 2 + transform.position.y;
        var bottom = -height / 2 + transform.position.y;
        return position.x >= left && position.x <= right && position.y >= bottom && position.y <= top;
    }

}