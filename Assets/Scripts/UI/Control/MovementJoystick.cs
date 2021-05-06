using UnityEngine;

public class MovementJoystick : BasicJoystick
{
    public ControlData control;
    public Vector2 graceDistance = new Vector2(0.2f, 0.5f);
    public float sensitivityX = 2;
    public float sensitivityY = 1000;
    private bool previousPressedState = false;
    private void LateUpdate()
    {
        if (Joystick.Pressed)
        {
            var vector = new Vector2();
            if (Value.x > graceDistance.x)
                vector.x = Mathf.Min(1, sensitivityX * Value.x);
            if (-Value.x > graceDistance.x)
                vector.x = Mathf.Max(-1, sensitivityX * Value.x);
            if (Value.y > graceDistance.y)
                vector.y = Mathf.Min(1, sensitivityY * Value.y);
            if (-Value.y > graceDistance.y)
                vector.y = Mathf.Max(-1, sensitivityY * Value.y);

            control.move = vector;

        }
        else if (Joystick.Pressed != previousPressedState)
        {
            control.move = new Vector2();
        }

        previousPressedState = Joystick.Pressed;
    }
}