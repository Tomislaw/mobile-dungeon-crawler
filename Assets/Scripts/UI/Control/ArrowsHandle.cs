using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace RuinsRaiders.UI
{
    // Responsible for custom Mobile joystick type based on arrows
    public class ArrowsHandle : MonoBehaviour
    {
        [SerializeField]
        private ArrowsHandleButton Up;
        [SerializeField]
        protected ArrowsHandleButton Down;
        [SerializeField]
        private ArrowsHandleButton Left;
        [SerializeField]
        private ArrowsHandleButton Right;

        [SerializeField]
        private ArrowsHandleButton LeftUp;
        [SerializeField]
        private ArrowsHandleButton LeftDown;
        [SerializeField]
        private ArrowsHandleButton RightUp;
        [SerializeField]
        private ArrowsHandleButton RightDown;

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

        private float startOffsetHorizontal;
        [SerializeField]
        private float midOffsetHorizontal;
        [SerializeField]
        private float midSizeHorizontal;
        [SerializeField]
        private float endOffsetHorizontal;
        [SerializeField]
        private float endSizeHorizontal;

        private int pointerId = -1;

        private RectTransform rect;

        public Vector2 Value { get; internal set; }

        public bool Pressed { get; private set; }

        void Start()
        {
            rect = GetComponent<RectTransform>();
        }


        private void PressingPoint(Vector2 point)
        {
            point = transform.InverseTransformVector(point);
            point -= rect.anchoredPosition;

            Value = new Vector2();
            ResolvePress(Poly.ContainsPoint(LeftHull(), point), Left, new Vector2(-1, Value.y));
            ResolvePress(Poly.ContainsPoint(RightHull(), point), Right, new Vector2(1, Value.y));

            ResolvePress(Poly.ContainsPoint(TopHull(), point), Up, new Vector2(Value.x, 1));
            ResolvePress(Poly.ContainsPoint(BottomHull(), point), Down, new Vector2(Value.x, -1));

            ResolvePress(Poly.ContainsPoint(LeftTopHull(), point), LeftUp, new Vector2(-1, 1));
            ResolvePress(Poly.ContainsPoint(RightTopHull(), point), RightUp, new Vector2(1, 1));

            ResolvePress(Poly.ContainsPoint(LeftBottomHull(), point), LeftDown, new Vector2(-1, -1));
            ResolvePress(Poly.ContainsPoint(RightBottomHull(), point), RightDown, new Vector2(1, -1));
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
            Vector2 previous = new Vector2();
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
            position -= rect.anchoredPosition;
            return Poly.ContainsPoint(TotalHull(), position);
        }

        public void CheckForUnpress()
        {
            if (pointerId >= 0)
            {
                foreach (var touch in Touchscreen.current.touches)
                {
                    if (touch.touchId.ReadValue() == pointerId)
                    {
                        switch (touch.phase.ReadValue())
                        {
                            case UnityEngine.InputSystem.TouchPhase.None:
                            case UnityEngine.InputSystem.TouchPhase.Ended:
                            case UnityEngine.InputSystem.TouchPhase.Canceled:
                                TouchFinished(pointerId);
                                break;
                        };
                    }
                }
            }
            else if (pointerId == -1)
            {
                if (!Mouse.current.leftButton.isPressed)
                    TouchFinished(-1);
            }
        }
        public void ResolveMove()
        {
            Vector2 point = new Vector2();
            if (pointerId >= 0)
            {
                foreach (var touch in Touchscreen.current.touches)
                {
                    if (touch.touchId.ReadValue() != pointerId)
                        continue;

                    point = touch.position.ReadValue();
                    break;
                }
            }
            else if (pointerId == -1)
                point = Mouse.current.position.ReadValue();

            var previousValue = Value;
            PressingPoint(point);
            if (previousValue != Value)
                TouchMoved(pointerId, Value);
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
            Debug.Log(id + " - started");
            pointerId = id;
            Pressed = true;
        }

        public void TouchFinished(int id)
        {
            Debug.Log(id + " - finished");
            pointerId = -2;
            Pressed = false;
            Value = new Vector2();

            ResolvePress(false, Left);
            ResolvePress(false, Right);
            ResolvePress(false, Up);
            ResolvePress(false, Down);
            ResolvePress(false, LeftDown);
            ResolvePress(false, LeftUp);
            ResolvePress(false, RightUp);
            ResolvePress(false, RightDown);
        }

        public void TouchMoved(int id, Vector2 value)
        {

        }

    }
}
