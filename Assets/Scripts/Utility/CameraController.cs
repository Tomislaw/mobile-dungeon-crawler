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
        private Vector2 cameraSizeInUnits = new(16, 9);

        [SerializeField]
        private AnimationCurve tweenAnimation;

        [SerializeField]
        private float tweenTime = 1;

        private SubScene _subScene;

        // bounds of current SubScene
        private float _left = 0;
        private float _right = 0;
        private float _top = 0;
        private float _bottom = 0;

        private float _paddingX = 0;
        private float _paddingY = 0;

        private float _tweanTimeLeft = 0;
        private Vector3 _tweanStartingPos;

        private void Start()
        {
            if (controlledCamera == null)
                controlledCamera = GetComponent<Camera>();

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

            if (_tweanTimeLeft > 0)
                _tweanTimeLeft -= Time.deltaTime;
        }

        private void SetSubScene(SubScene scene)
        {
            if (_subScene == scene)
                return;

            scene.CameraEneterd();

            _left = -scene.size.x / 2 + scene.transform.position.x;
            _right = scene.size.x / 2 + scene.transform.position.x;
            _top = scene.size.y / 2 + scene.transform.position.y;
            _bottom = -scene.size.y / 2 + scene.transform.position.y;

            _paddingX = scene.padding.x;
            _paddingY = scene.padding.y;

            if (_subScene != null)
            {
                _tweanStartingPos = transform.position;
                _tweanTimeLeft = tweenTime;
            }
            _subScene = scene;

        }

        private Vector3 CalculatePosition()
        {
            var startingPosition = objectToFollow != null ?
                new Vector3(objectToFollow.transform.position.x, objectToFollow.transform.position.y, transform.position.z)
                : transform.position;

            if (controlledCamera && _subScene)
            {
                float w = cameraSizeInUnits.x;
                float h = cameraSizeInUnits.y;

                if (startingPosition.x - w < _left)
                    startingPosition = new Vector3(_left + w, startingPosition.y, startingPosition.z);
                if (startingPosition.x + w > _right)
                    startingPosition = new Vector3(_right - w, startingPosition.y, startingPosition.z);
                if (startingPosition.y - h < _bottom)
                    startingPosition = new Vector3(startingPosition.x, _bottom + h, startingPosition.z);
                if (startingPosition.y + h > _top)
                    startingPosition = new Vector3(startingPosition.x, _top - h, startingPosition.z);

            }

            if (_tweanTimeLeft >= 0)
                startingPosition = Vector3.Lerp(_tweanStartingPos, startingPosition, 1f - _tweanTimeLeft / tweenTime);

            return startingPosition;
        }


        private void UpdateCameraPosition()
        {
            if (objectToFollow == null)
                return;

            if (_subScene == null)
            {
                var scenes = FindObjectsOfType<SubScene>();
                if (scenes.Length == 0)
                    return;
                var scene = scenes.OrderBy(it =>
                    it.Contains(objectToFollow.transform.position) ? 0 :
                    Vector3.Distance(it.transform.position, objectToFollow.transform.position)
                ).First();
                SetSubScene(scene);
            }

            if (_subScene)
            {
                if (_subScene.left && (_left > objectToFollow.transform.position.x - _paddingX / 2f))
                    SetSubScene(_subScene.left);
                else if (_subScene.right && (_right < objectToFollow.transform.position.x + _paddingX / 2f))
                    SetSubScene(_subScene.right);
                else if (_subScene.bottom && (_bottom > objectToFollow.transform.position.y - _paddingY / 2f))
                    SetSubScene(_subScene.bottom);
                else if (_subScene.top && (_top < objectToFollow.transform.position.y + _paddingY / 2f))
                    SetSubScene(_subScene.top);
            }

            transform.position = CalculatePosition();


        }
    }
}