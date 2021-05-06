
using UnityEngine;

public class MovementArrowsHandle : ArrowsHandle
{
    public ControlData control;

    private bool previousPressedState = false;
    private void LateUpdate()
    {
        if (Pressed)
        {
            control.move = Value;
        }
        else if (Pressed != previousPressedState)
        {
            control.move = new Vector2();
        }

        previousPressedState = Pressed;
    }
}