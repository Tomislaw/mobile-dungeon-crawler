﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

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

}