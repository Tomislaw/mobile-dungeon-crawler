using UnityEngine;

[DefaultExecutionOrder(100)]
[RequireComponent(typeof(SpriteRenderer))]
public class ParallaxBackground : MonoBehaviour
{
    public SpriteRenderer sprite;
    public Vector2 movementFactor;

    public bool scaleToFitScreen = true;

    void Start()
    {
        if(sprite == null)
            sprite = GetComponent<SpriteRenderer>();

    }
    void LateUpdate()
    {
        Vector2 cameraPos = new Vector2();


        var camera =  Camera.main;

        if (camera != null) {
            cameraPos = camera.transform.position;

            if (scaleToFitScreen && sprite != null)
            {
                Vector2 cameraSize = new Vector2();
                cameraSize.y = 2f * camera.orthographicSize;
                cameraSize.x = cameraSize.y * camera.aspect;
                cameraSize.x += 1;
                cameraSize.y += 1;

                sprite.size = cameraSize;

            }
        }


        transform.position = new Vector3(cameraPos.x + transform.position.x * movementFactor.x,
                                         cameraPos.y + transform.position.y * movementFactor.y, 
                                         transform.position.z);
    }
}
