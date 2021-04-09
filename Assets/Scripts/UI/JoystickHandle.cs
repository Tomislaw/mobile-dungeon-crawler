using UnityEngine;
using UnityEngine.EventSystems;

[ExecuteAlways]
[RequireComponent(typeof(RectTransform))]
public class JoystickHandle : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    internal float Radius;

    internal Vector2 Center;
    internal Vector2 Value;
    internal bool Pressed;

    private Vector2 LocalInitialPosition;
    private Vector2 WorldInitialPosition;

    public void Awake()
    {
        LocalInitialPosition = transform.localPosition;
    }

    public void Update()
    {
        if (Pressed)
        {
            Vector2 point = Input.mousePosition;
            var normalized = (point - WorldInitialPosition).normalized;

            transform.position = point;
            var distance = Vector2.Distance(LocalInitialPosition, transform.localPosition);
            if (distance > Radius)
                transform.localPosition = normalized * Radius;

            Value = normalized * distance / Radius;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        transform.localPosition = LocalInitialPosition;
        WorldInitialPosition = transform.position;
        Pressed = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Pressed = false;
        transform.localPosition = LocalInitialPosition;
        Value = new Vector2();
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