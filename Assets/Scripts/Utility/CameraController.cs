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
        private Camera controlledCamera;

        [SerializeField]
        private SubScene subScene;

        [SerializeField]
        private Vector2 cameraSizeInUnits = new(16, 9);

        // bounds of current SubScene
        private float _left = 0;
        private float _right = 0;
        private float _top = 0;
        private float _bottom = 0;

        private void Start()
        {
            if (controlledCamera == null)
                controlledCamera = GetComponent<Camera>();

            SetSubScene(subScene);
            UpdateCameraPosition();
        }

        private void OnValidate()
        {
            if (controlledCamera == null)
                controlledCamera = GetComponent<Camera>();
        }

        private void LateUpdate()
        {
            UpdateCameraPosition();
        }

        public void SetSubScene(SubScene scene)
        {
            if (!scene)
                return;

            _left = -scene.size.x / 2 + scene.transform.position.x;
            _right = scene.size.x / 2 + scene.transform.position.x;
            _top = scene.size.y / 2 + scene.transform.position.y;
            _bottom = -scene.size.y / 2 + scene.transform.position.y;

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
                if (subScene.left && _left > objectToFollow.transform.position.x)
                    SetSubScene(subScene.left);
                else if (subScene.right && _right < objectToFollow.transform.position.x)
                    SetSubScene(subScene.right);
                else if (subScene.bottom && _bottom > objectToFollow.transform.position.y)
                    SetSubScene(subScene.bottom);
                else if (subScene.top && _top < objectToFollow.transform.position.y)
                    SetSubScene(subScene.top);
            }

            if (controlledCamera && subScene)
            {
                float w = cameraSizeInUnits.x;
                float h = cameraSizeInUnits.y;

                if (transform.position.x - w < _left)
                    transform.position = new Vector3(_left + w, transform.position.y, transform.position.z);
                if (transform.position.x + w > _right)
                    transform.position = new Vector3(_right - w, transform.position.y, transform.position.z);
                if (transform.position.y - h < _bottom)
                    transform.position = new Vector3(transform.position.x, _bottom + h, transform.position.z);
                if (transform.position.y + h > _top)
                    transform.position = new Vector3(transform.position.x, _top - h, transform.position.z);

            }
        }
    }
}