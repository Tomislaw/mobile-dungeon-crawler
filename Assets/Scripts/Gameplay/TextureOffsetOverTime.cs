using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureOffsetOverTime : MonoBehaviour
{
    public Vector2 offsetPerSecond;
    private new Renderer renderer;

    void Start()
    {
        renderer = GetComponent<Renderer>();
    }

    void Update()
    {
        if (renderer != null || renderer.material != null) { 
            var offset = renderer.material.mainTextureOffset + offsetPerSecond * Time.deltaTime;
            offset.x %= 1f;
            offset.y %= 1f;
            renderer.material.mainTextureOffset = offset;
        }
    }
}
