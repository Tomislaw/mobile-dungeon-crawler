using UnityEngine;
using UnityEngine.InputSystem;


namespace RuinsRaiders.UI
{
    // Responsibe for inner part of virtual joystick
    [ExecuteAlways]
    [RequireComponent(typeof(RectTransform))]
    public class JoystickHandle : MonoBehaviour
    {
        internal float _radius;

        internal Vector2 _center;
        internal Vector2 _value;
        internal Vector2 _lastPointerPosition;

        internal bool _pressed;

        internal Vector2 _localInitialPosition;
        internal Vector2 _worldInitialPosition;

        private int _pointerId = -2;

        public void Awake()
        {
            _localInitialPosition = transform.localPosition;
        }

        private void CheckForPress()
        {
            _localInitialPosition = transform.localPosition;
            _worldInitialPosition = transform.position;
            if (Touchscreen.current != null)
            {
                int id = 0;
                foreach (var touch in Touchscreen.current.touches)
                {
                    if (touch.phase.ReadValue() == UnityEngine.InputSystem.TouchPhase.Began)
                    {
                        var distance = Vector2.Distance(_worldInitialPosition, touch.position.ReadValue());
                        if (distance < _radius)
                        {
                            _lastPointerPosition = touch.position.ReadValue();
                            TouchStarted(touch.touchId.ReadValue());
                            return;
                        }

                    }
                    id++;
                }
            }

            if (Mouse.current.leftButton.isPressed && Mouse.current.leftButton.wasPressedThisFrame)
            {
                var distance = Vector2.Distance(_worldInitialPosition, Mouse.current.position.ReadValue());
                if (distance < _radius)
                {
                    TouchStarted(-1);
                    return;
                }
            }
        }

        public void CheckForUnpress()
        {
            if (_pointerId >= 0)
            {
                foreach (var touch in Touchscreen.current.touches)
                {
                    if (touch.touchId.ReadValue() == _pointerId)
                    {
                        switch (touch.phase.ReadValue())
                        {
                            case UnityEngine.InputSystem.TouchPhase.None:
                            case UnityEngine.InputSystem.TouchPhase.Ended:
                            case UnityEngine.InputSystem.TouchPhase.Canceled:
                                TouchFinished(_pointerId);
                                break;
                        };
                    }
                }
            }
            else if (_pointerId == -1)
            {
                if (!Mouse.current.leftButton.isPressed)
                    TouchFinished(-1);
            }
        }
        public void ResolveMove()
        {
            Vector2 point = new();
            if (_pointerId >= 0)
            {
                foreach (var touch in Touchscreen.current.touches)
                {
                    if (touch.touchId.ReadValue() != _pointerId)
                        continue;

                    point = touch.position.ReadValue();
                    break;
                }
            }
            else if (_pointerId == -1)
                point = Mouse.current.position.ReadValue();

            _lastPointerPosition = point;

            var normalized = (point - _worldInitialPosition).normalized;

            transform.position = point;
            var distance = Vector2.Distance(_localInitialPosition, transform.localPosition);
            if (distance > _radius)
                transform.localPosition = normalized * _radius;

            var previousValue = _value;
            _value = transform.localPosition / _radius;
            if (previousValue != _value)
                TouchMoved(_pointerId, _value);
        }

        public void Update()
        {
            if (!_pressed)
                CheckForPress();

            if (_pressed)
            {
                ResolveMove();
                CheckForUnpress();
            }
        }

        public void TouchStarted(int id)
        {
            Debug.Log(id + " - started");
            _pointerId = id;
            _pressed = true;
        }

        public void TouchFinished(int id)
        {
            Debug.Log(id + " - finished");
            _pointerId = -2;
            _pressed = false;
            _value = new Vector2();
            transform.localPosition = _localInitialPosition;
        }

        public void TouchMoved(int id, Vector2 value)
        {

        }
    }

    [ExecuteAlways]
    [RequireComponent(typeof(RectTransform))]
    public abstract class BasicJoystick : MonoBehaviour
    {
        public float Radius;
        public Vector2 Center;

        public Vector2 Value { get => joystick == null ? new Vector2() : joystick._value; }


        [SerializeField]
        protected JoystickHandle joystick;

        // Update is called once per frame
        protected virtual void Update()
        {
            if (joystick)
            {
                joystick._center = Center;
                joystick._radius = Radius;
            }
        }
    }
}