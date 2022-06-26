using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace RuinsRaiders
{
    [RequireComponent(typeof(Character))]
    public class PlayerController : MonoBehaviour
    {
        [SerializeField]
        private ControlData control;

        private Character _character;
        private MovementController _movementController;
        private AttackController _attackController;

        private bool _previousAttackState = false;

        private void Awake()
        {
            _character = GetComponent<Character>();
            _movementController = GetComponent<MovementController>();
            _attackController = GetComponent<AttackController>();
        }

        public void Update()
        {
            if (control)
            {
                _movementController.Move(control.move);
                _attackController.chargeAttack = control.attack;

                if (_previousAttackState == true && !control.attack)
                    _attackController.Attack();

                _previousAttackState = control.attack;
            }
            else
            {
                var find = _character.GetComponent<PathfindingController>();
                if (find != null)
                {
                    if (Mouse.current.leftButton.isPressed && Camera.main != null)
                        find.MoveTo(Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue()));
                }
            }

        }

        public ControlData GetControlData()
        {
            return control;
        }
    }
}