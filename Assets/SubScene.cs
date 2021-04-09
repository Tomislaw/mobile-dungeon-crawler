using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubScene : MonoBehaviour
{
    public float width = 32;
    public float height = 18;

    public SubScene left;
    public SubScene right;
    public SubScene top;
    public SubScene bottom;

    public GameObject respawnPoint;

    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1, 0, 0, 0.5f);
        Gizmos.DrawWireCube(transform.position, new Vector3(width, height, 1));
    }
}