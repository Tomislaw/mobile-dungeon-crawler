using UnityEngine;


namespace RuinsRaiders.UI
{
    public class MovementJoystick : BasicJoystick
    {
        [SerializeField]
        private ControlData control;
        [SerializeField]
        private Vector2 graceDistance = new Vector2(0.2f, 0.5f);
        [SerializeField]
        private float sensitivityX = 2;
        [SerializeField]
        private float sensitivityY = 1000;

        private bool previousPressedState = false;
        private void LateUpdate()
        {
            if (joystick.Pressed)
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
            else if (joystick.Pressed != previousPressedState)
            {
                control.move = new Vector2();
            }

            previousPressedState = joystick.Pressed;
        }
    }
}