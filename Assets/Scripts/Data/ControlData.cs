using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerData", menuName = "RuinsRaiders/ControlData", order = 1)]
public class ControlData : ScriptableObject
{
    public Vector2 move;
    public bool attack;

    public void Attack(bool attack)
    {
        this.attack = attack;
    }

    public void MoveLeft(bool move)
    {
        this.move.x = move ? -1 : 0;

    }

    public void MoveRight(bool move)
    {
        this.move.x = move ? 1 : 0;
    }

    public void MoveUp(bool move)
    {
        this.move.y = move ? 1 : 0;
    }

    public void MoveDown(bool move)
    {
        this.move.y = move ? -1 : 0;
    }
}