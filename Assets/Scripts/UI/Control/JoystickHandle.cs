using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

[ExecuteAlways]
[RequireComponent(typeof(RectTransform))]
public class JoystickHandle : MonoBehaviour
{
    internal float Radius;

    internal Vector2 Center;
    internal Vector2 Value;
    internal bool Pressed;

    private Vector2 LocalInitialPosition;
    private Vector2 WorldInitialPosition;

    private int pointerId = -2;

    public void Awake()
    {
        LocalInitialPosition = transform.localPosition;
    }

    private void CheckForPress()
    {
        LocalInitialPosition = transform.localPosition;
        WorldInitialPosition = transform.position;
        if (Touchscreen.current != null)
        {
            int id = 0;
            foreach (var touch in Touchscreen.current.touches)
            {
                if (touch.phase.ReadValue() == UnityEngine.InputSystem.TouchPhase.Began)
                {
                    var distance = Vector2.Distance(WorldInitialPosition, touch.position.ReadValue());
                    if (distance < Radius)
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
            var distance = Vector2.Distance(WorldInitialPosition, Mouse.current.position.ReadValue());
            if (distance < Radius)
            {
                TouchStarted(-1);
                return;
            }
        }
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

        var normalized = (point - WorldInitialPosition).normalized;

        transform.position = point;
        var distance = Vector2.Distance(LocalInitialPosition, transform.localPosition);
        if (distance > Radius)
            transform.localPosition = normalized * Radius;

        var previousValue = Value;
        Value = transform.localPosition / Radius;
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
        transform.localPosition = LocalInitialPosition;
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
    public JoystickHandle Joystick;

    public Vector2 Value { get => Joystick == null ? new Vector2() : Joystick.Value; }

    // Update is called once per frame
    protected virtual void Update()
    {
        if (Joystick)
        {
            Joystick.Center = Center;
            Joystick.Radius = Radius;
        }
    }
}
