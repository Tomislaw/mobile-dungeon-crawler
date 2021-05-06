using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "OptionsData", menuName = "RuinsRaiders/OptionsData", order = 1)]
public class OptionsData : ScriptableObject
{
    public TouchUiType touchUiType;

    public float musicVolume = 1;
    public float effectsVolume = 1;

    public enum TouchUiType
    {
        Arrows1, 
        Arrows2,
        ArrowsSeparate,
        Joystick,
        None
    }


}