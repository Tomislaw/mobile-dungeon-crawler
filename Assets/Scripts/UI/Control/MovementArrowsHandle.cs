using UnityEngine;


namespace RuinsRaiders.UI
{
    public class MovementArrowsHandle : ArrowsHandle
    {
        [SerializeField]
        private ControlData control;

        private bool _previousPressedState = false;
        private void LateUpdate()
        {
            if (Pressed)
            {
                control.move = Value;
            }
            else if (Pressed != _previousPressedState)
            {
                control.move = new Vector2();
            }

            _previousPressedState = Pressed;
        }
    }
}