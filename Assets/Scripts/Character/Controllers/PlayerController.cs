using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Character))]
public class PlayerController : MonoBehaviour
{
    private Character character;
    private MovementController movementController;
    private AttackController attackController;

    public ControlData control;
    public PlayerControlls inputControl;

    private void Awake()
    {
        character = GetComponent<Character>();
        movementController = GetComponent<MovementController>();
        attackController = GetComponent<AttackController>();
    }

    // Update is called once per frame

    private bool previousAttackState = false;
    public void Update()
    {


        if (inputControl)
        {
            inputControl.Update();
            movementController.Move(control.move);

            attackController.ChargeAttack = control.attack;
            if (previousAttackState == true && !control.attack)
                attackController.Attack();

            previousAttackState = control.attack;
        }
        else
        {
            var find = character.GetComponent<PathfindingController>();
            if (find != null)
            {
                if (Mouse.current.leftButton.isPressed)
                    find.MoveTo(Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue()));
            }
        }

    }
}