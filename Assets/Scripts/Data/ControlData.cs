using UnityEngine;
using UnityEngine.InputSystem;

namespace RuinsRaiders
{

    [CreateAssetMenu(fileName = "PlayerData", menuName = "RuinsRaiders/ControlData", order = 1)]
    public class ControlData : ScriptableObject
    {
        public Vector2 move;
        public bool attack;

        public void Attack(InputAction.CallbackContext context)
        {
            this.attack = !context.canceled;
        }

        public void Attack(bool attack)
        {
            this.attack = attack;
        }

        public void Move(InputAction.CallbackContext context)
        {
            this.move = context.ReadValue<Vector2>();
        }

        public void MoveUp(bool isPressing)
        {
            this.move = new Vector2(this.move.x, isPressing ? 1f : 0f);
        }


        public void MoveDown(bool isPressing)
        {
            this.move = new Vector2(this.move.x, isPressing ? -1f : 0f);
        }

        public void MoveLeft(bool isPressing)
        {
            this.move = new Vector2(isPressing ? -1f : 0f, this.move.y);
        }

        public void MoveRight(bool isPressing)
        {
            this.move = new Vector2(isPressing ? 1f : 0f, this.move.y);
        }

    }
}