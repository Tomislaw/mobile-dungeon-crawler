using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "PlayerControlls", menuName = "RuinsRaiders/PlayerControlls", order = 1)]
public class PlayerControlls : ScriptableObject
{
    public ControlData control;

    public InputAction XAxis;
    public InputAction YAxis;
    public InputAction Attack;
    public InputAction Menu;
    private bool updating = false;
    public void Update()
    {
        //XAxis.controls.
        //float x = XAxis.activeControl.;
        //float y = Input.GetAxis(YAxis);
        //float attack = Input.GetAxis(Attack);
        //if (x != 0 || y != 0 || attack!= 0)
        //    updating = true;

        //if (updating)
        //{
        //    control.move.x = x;
         //   control.move.y = y;
         //   control.attack = attack >= 1;
         //   if (x == 0 && y == 0 && attack == 0)
         //       updating = false;
        //}

    }

    private void OnEnable()
    {
        //Debug.LogError("attack");
        //Attack.started += (input) => {
        //    control.attack = true;
        //    Debug.LogError("attack");
        //};
        //Attack.performed += (input)=>{
        //    control.attack = false;
        //};
    }


}

