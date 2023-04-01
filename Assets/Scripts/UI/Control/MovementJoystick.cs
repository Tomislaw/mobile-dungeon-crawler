using UnityEngine;

namespace RuinsRaiders.UI
{
    public class MovementJoystick : BasicJoystick
    {
        [SerializeField]
        private ControlData control;

        [SerializeField]
        public Vector2 maxValues = new Vector2(1f,1f);

        [SerializeField]
        public Vector2 minValues = new Vector2(-1f, -1f);

        [SerializeField]
        private Vector2 graceDistance = new(0.2f, 0.5f);

        [SerializeField]
        private float sensitivityX = 2;

        [SerializeField]
        private float sensitivityY = 1000;

        [SerializeField]
        private bool dynamicPosition = true;

        [SerializeField]
        private Transform dynamicPositionOrigin;

        private bool _previousPressedState = false;


        private void LateUpdate()
        {
            if (joystick == null)
            {
                return;
            }

            if (joystick._pressed != _previousPressedState)
            {
                control.move = new Vector2();
                if (dynamicPosition)
                {
                    if (joystick._pressed)
                    {
                        transform.position = joystick._lastPointerPosition;
                        joystick.transform.position = joystick._lastPointerPosition;
                        joystick._worldInitialPosition = joystick._lastPointerPosition;
                        joystick._localInitialPosition = joystick.transform.localPosition;
                    }
                    else
                    {
                        transform.position = dynamicPositionOrigin.position;
                        joystick.transform.position = dynamicPositionOrigin.position;
                        joystick._worldInitialPosition = dynamicPositionOrigin.position;
                        joystick._localInitialPosition = joystick.transform.localPosition;
                    }
                }
            }

            if (joystick._pressed)
            {
                var vector = new Vector2();
                if (Value.x > graceDistance.x)
                    vector.x = Mathf.Min(maxValues.x, sensitivityX * Value.x);
                if (-Value.x > graceDistance.x)
                    vector.x = Mathf.Max(minValues.x, sensitivityX * Value.x);
                if (Value.y > graceDistance.y)
                    vector.y = Mathf.Min(maxValues.y, sensitivityY * Value.y);
                if (-Value.y > graceDistance.y)
                    vector.y = Mathf.Max(minValues.y, sensitivityY * Value.y);

                control.move = vector;
            }

            _previousPressedState = joystick._pressed;
        }
    }
}