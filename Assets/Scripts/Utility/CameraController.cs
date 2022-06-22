using System.Linq;
using UnityEngine;

namespace RuinsRaiders
{
    // Camera controller bounded by SubScene
    public class CameraController : MonoBehaviour
    {
        [SerializeField]
        private GameObject objectToFollow;
        [SerializeField]
        private Camera camera;
        [SerializeField]
        private SubScene subScene;
        [SerializeField]
        private Vector2 cameraSizeInUnits = new Vector2(16, 9);

        // bounds of current SubScene
        private float left = 0;
        private float right = 0;
        private float top = 0;
        private float bottom = 0;

        private void Start()
        {
            if (camera == null)
                camera = GetComponent<Camera>();

            SetSubScene(subScene);
            UpdateCameraPosition();
        }

        private void OnValidate()
        {
            if (camera == null)
                camera = GetComponent<Camera>();
        }

        private void LateUpdate()
        {
            UpdateCameraPosition();
        }

        public void SetSubScene(SubScene scene)
        {
            if (!scene)
                return;

            left = -scene.width / 2 + scene.transform.position.x;
            right = scene.width / 2 + scene.transform.position.x;
            top = scene.height / 2 + scene.transform.position.y;
            bottom = -scene.height / 2 + scene.transform.position.y;

            subScene = scene;
        }

        private void UpdateCameraPosition()
        {
            if (objectToFollow == null)
                return;

            transform.position = new Vector3(
                objectToFollow.transform.position.x,
                objectToFollow.transform.position.y,
                transform.position.z);

            if (subScene == null)
            {
                var scenes = FindObjectsOfType<SubScene>();
                if (scenes.Length == 0)
                    return;
                subScene = scenes.OrderBy(it => Vector3.Distance(it.transform.position, transform.position)).First();
            }

            if (subScene)
            {
                if (subScene.left && left > objectToFollow.transform.position.x)
                    SetSubScene(subScene.left);
                else if (subScene.right && right < objectToFollow.transform.position.x)
                    SetSubScene(subScene.right);
                else if (subScene.bottom && bottom > objectToFollow.transform.position.y)
                    SetSubScene(subScene.bottom);
                else if (subScene.top && top < objectToFollow.transform.position.y)
                    SetSubScene(subScene.top);
            }

            if (camera && subScene)
            {
                float w = cameraSizeInUnits.x;
                float h = cameraSizeInUnits.y;

                if (transform.position.x - w < left)
                    transform.position = new Vector3(left + w, transform.position.y, transform.position.z);
                if (transform.position.x + w > right)
                    transform.position = new Vector3(right - w, transform.position.y, transform.position.z);
                if (transform.position.y - h < bottom)
                    transform.position = new Vector3(transform.position.x, bottom + h, transform.position.z);
                if (transform.position.y + h > top)
                    transform.position = new Vector3(transform.position.x, top - h, transform.position.z);

            }
        }
    }
}