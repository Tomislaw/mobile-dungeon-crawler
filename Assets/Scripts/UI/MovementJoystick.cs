using UnityEngine;

public class MovementJoystick : BasicJoystick
{
    public ControlData control;
    public float graceDisstance = 0.5f;

    private bool previousPressedState = false;
    private void LateUpdate()
    {
        if (Joystick.Pressed)
        {
            var vector = new Vector2();
            if (Value.x > graceDisstance)
                vector.x = 1;
            if (-Value.x > graceDisstance)
                vector.x = -1;
            if (Value.y > graceDisstance)
                vector.y = 1;
            if (-Value.y > graceDisstance)
                vector.y = -1;

            control.move = vector;

        }
        else if (Joystick.Pressed != previousPressedState)
        {
            control.move = new Vector2();
        }

        previousPressedState = Joystick.Pressed;
    }
}