using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Character))]
public class PlayerController : MonoBehaviour
{
    private Character character;

    public ControlData control;
    public PlayerControlls inputControl;

    private void Awake()
    {
        character = GetComponent<Character>();
    }

    // Update is called once per frame

    private bool previousAttackState = false;
    public void Update()
    {
        if (inputControl)
            inputControl.Update();

        var find = character.GetComponent<PathfindingController>();
        if (find != null)
        {
            if (Input.GetMouseButtonDown(0))
                find.MoveTo(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        }
            
        character.Move = control.move;

        character.ChargeAttack = control.attack;
        if (previousAttackState == true && !control.attack)
            character.Attack();

        previousAttackState = control.attack;


    }
}