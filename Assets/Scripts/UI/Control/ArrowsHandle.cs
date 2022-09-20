using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace RuinsRaiders.UI
{
    // Responsible for custom Mobile joystick type based on arrows
    public class ArrowsHandle : MonoBehaviour
    {
        public Vector2 Value { get; internal set; }
        public bool Pressed { get; private set; }

        [SerializeField]
        private ArrowsHandleButton upButton;
        [SerializeField]
        protected ArrowsHandleButton downButton;
        [SerializeField]
        private ArrowsHandleButton leftButton;
        [SerializeField]
        private ArrowsHandleButton rightButton;

        [SerializeField]
        private ArrowsHandleButton leftUpButton;
        [SerializeField]
        private ArrowsHandleButton leftDownButton;
        [SerializeField]
        private ArrowsHandleButton rightUpButton;
        [SerializeField]
        private ArrowsHandleButton rightDownButton;

        [SerializeField]
        private float startOffsetVertical;
        [SerializeField]
        private float midOffsetVertical;
        [SerializeField]
        private float midSizeVertical;
        [SerializeField]
        private float endOffsetVertical;
        [SerializeField]
        private float endSizeVertical;
        [SerializeField]
        private float startOffsetHorizontal;
        [SerializeField]
        private float midOffsetHorizontal;
        [SerializeField]
        private float midSizeHorizontal;
        [SerializeField]
        private float endOffsetHorizontal;
        [SerializeField]
        private float endSizeHorizontal;

        private int _pointerId = -1;

        private RectTransform _rect;

        void Start()
        {
            _rect = GetComponent<RectTransform>();
        }


        private void PressingPoint(Vector2 point)
        {
            point = transform.InverseTransformVector(point);
            point -= _rect.anchoredPosition;

            Value = new Vector2();
            ResolvePress(Poly.ContainsPoint(LeftHull(), point), leftButton, new Vector2(-1, Value.y));
            ResolvePress(Poly.ContainsPoint(RightHull(), point), rightButton, new Vector2(1, Value.y));

            ResolvePress(Poly.ContainsPoint(TopHull(), point), upButton, new Vector2(Value.x, 1));
            ResolvePress(Poly.ContainsPoint(BottomHull(), point), downButton, new Vector2(Value.x, -1));

            ResolvePress(Poly.ContainsPoint(LeftTopHull(), point), leftUpButton, new Vector2(-1, 1));
            ResolvePress(Poly.ContainsPoint(RightTopHull(), point), rightUpButton, new Vector2(1, 1));

            ResolvePress(Poly.ContainsPoint(LeftBottomHull(), point), leftDownButton, new Vector2(-1, -1));
            ResolvePress(Poly.ContainsPoint(RightBottomHull(), point), rightDownButton, new Vector2(1, -1));
        }

        private void ResolvePress(bool pressed, ArrowsHandleButton button, Vector2 valuePressed = new Vector2())
        {
            if (pressed)
                Value = valuePressed;
            if (button)
                button.SetSelected(pressed);
        }

        void OnDrawGizmosSelected()
        {
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.color = Color.red;
            DrawHull(LeftHull());
            DrawHull(RightHull());

            Gizmos.color = Color.blue;
            DrawHull(BottomHull());
            DrawHull(TopHull());

            Gizmos.color = Color.green;
            DrawHull(LeftTopHull());
            DrawHull(RightTopHull());
            DrawHull(LeftBottomHull());
            DrawHull(RightBottomHull());

            Gizmos.color = Color.white;
            DrawHull(TotalHull());
        }

        private void DrawHull(IEnumerable<Vector2> vertex)
        {
            bool started = false;
            Vector2 previous = new();
            foreach (var v in vertex)
            {
                if (started)
                {
                    Gizmos.DrawLine(previous, v);
                }
                started = true;
                previous = v;
            }
        }

        private Vector2[] LeftHull()
        {
            return new[] {
            new Vector2(-startOffsetHorizontal, 0),
            new Vector2(-midOffsetHorizontal, midSizeHorizontal / 2),
            new Vector2(-endOffsetHorizontal, endSizeHorizontal / 2),
            new Vector2(-endOffsetHorizontal, -endSizeHorizontal / 2),
            new Vector2(-midOffsetHorizontal, -midSizeHorizontal / 2),
            new Vector2(-startOffsetHorizontal, 0)
        };
        }

        private Vector2[] RightHull()
        {
            return new[] {
            new Vector2(startOffsetHorizontal, 0),
            new Vector2(midOffsetHorizontal, midSizeHorizontal / 2),
            new Vector2(endOffsetHorizontal, endSizeHorizontal / 2),
            new Vector2(endOffsetHorizontal, -endSizeHorizontal / 2),
            new Vector2(midOffsetHorizontal, -midSizeHorizontal / 2),
            new Vector2(startOffsetHorizontal, 0)
        };
        }

        private Vector2[] TopHull()
        {
            return new[] {
            new Vector2(0, startOffsetVertical),
            new Vector2(midSizeVertical / 2, midOffsetVertical),
            new Vector2(endSizeVertical / 2, endOffsetVertical),
            new Vector2(-endSizeVertical / 2, endOffsetVertical),
            new Vector2(-midSizeVertical / 2, midOffsetVertical),
            new Vector2(0, startOffsetVertical)
        };
        }

        private Vector2[] LeftTopHull()
        {
            return new[] {
            new Vector2(-midSizeVertical / 2, midSizeHorizontal / 2),
            new Vector2(-endSizeVertical / 2, endOffsetVertical),
            new Vector2(-endOffsetHorizontal, endOffsetVertical),
            new Vector2(-endOffsetHorizontal, endSizeHorizontal / 2),
            new Vector2(-midSizeVertical / 2, midSizeHorizontal / 2),
        };
        }

        private Vector2[] RightTopHull()
        {
            return new[] {
            new Vector2(midSizeVertical / 2, midSizeHorizontal / 2),
            new Vector2(endSizeVertical / 2, endOffsetVertical),
            new Vector2(endOffsetHorizontal, endOffsetVertical),
            new Vector2(endOffsetHorizontal, endSizeHorizontal / 2),
            new Vector2(midSizeVertical / 2, midSizeHorizontal / 2),
        };
        }

        private Vector2[] LeftBottomHull()
        {
            return new[] {
            new Vector2(-midSizeVertical / 2, -midSizeHorizontal / 2),
            new Vector2(-endSizeVertical / 2, -endOffsetVertical),
            new Vector2(-endOffsetHorizontal, -endOffsetVertical),
            new Vector2(-endOffsetHorizontal, -endSizeHorizontal / 2),
            new Vector2(-midSizeVertical / 2, -midSizeHorizontal / 2),
        };
        }

        private Vector2[] RightBottomHull()
        {
            return new[] {
            new Vector2(midSizeVertical / 2, -midSizeHorizontal / 2),
            new Vector2(endSizeVertical / 2, -endOffsetVertical),
            new Vector2(endOffsetHorizontal, -endOffsetVertical),
            new Vector2(endOffsetHorizontal, -endSizeHorizontal / 2),
            new Vector2(midSizeVertical / 2, -midSizeHorizontal / 2),
        };
        }

        private Vector2[] BottomHull()
        {
            return new[] {
            new Vector2(0, -startOffsetVertical),
            new Vector2(midSizeVertical / 2, -midOffsetVertical),
            new Vector2(endSizeVertical / 2, -endOffsetVertical),
            new Vector2(-endSizeVertical / 2, -endOffsetVertical),
            new Vector2(-midSizeVertical / 2, -midOffsetVertical),
            new Vector2(0, -startOffsetVertical)
        };
        }

        private Vector2[] TotalHull()
        {
            return new[] {
            new Vector2(-endOffsetHorizontal, -endOffsetHorizontal),
            new Vector2(-endOffsetHorizontal, endOffsetHorizontal),
            new Vector2(endOffsetHorizontal, endOffsetHorizontal),
            new Vector2(endOffsetHorizontal, -endOffsetHorizontal),
            new Vector2(-endOffsetHorizontal, -endOffsetHorizontal)
        };
        }


        private void CheckForPress()
        {
            if (Touchscreen.current != null)
            {
                int id = 0;
                foreach (var touch in Touchscreen.current.touches)
                {
                    if (touch.phase.ReadValue() == UnityEngine.InputSystem.TouchPhase.Began)
                    {
                        if (Contains(touch.position.ReadValue()))
                        {
                            TouchStarted(touch.touchId.ReadValue());
                            return;
                        }

                    }
                    id++;
                }
            }
            if (Mouse.current.leftButton.isPressed && Mouse.current.leftButton.wasPressedThisFrame)
            {
                if (Contains(Mouse.current.position.ReadValue()))
                {
                    TouchStarted(-1);
                    return;
                }
            }
        }

        private bool Contains(Vector2 position)
        {
            position = transform.InverseTransformVector(position);
            position -= _rect.anchoredPosition;
            return Poly.ContainsPoint(TotalHull(), position);
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

            var previousValue = Value;
            PressingPoint(point);
            if (previousValue != Value)
                TouchMoved(_pointerId, Value);
        }

        public void Update()
        {
            if (!Pressed)
                CheckForPress();

            if (Pressed)
            {
                ResolveMove();
                CheckForUnpress();
            }
        }

        public void TouchStarted(int id)
        {
            _pointerId = id;
            Pressed = true;
        }

        public void TouchFinished(int id)
        {
            _pointerId = -2;
            Pressed = false;
            Value = new Vector2();

            ResolvePress(false, leftButton);
            ResolvePress(false, rightButton);
            ResolvePress(false, upButton);
            ResolvePress(false, downButton);
            ResolvePress(false, leftDownButton);
            ResolvePress(false, leftUpButton);
            ResolvePress(false, rightUpButton);
            ResolvePress(false, rightDownButton);
        }

        public void TouchMoved(int id, Vector2 value)
        {

        }

    }
}
