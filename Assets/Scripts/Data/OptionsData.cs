using System;
using UnityEngine;

namespace RuinsRaiders
{

    [CreateAssetMenu(fileName = "OptionsData", menuName = "RuinsRaiders/OptionsData", order = 1)]
    [Serializable]
    public class OptionsData : ScriptableObject, SaveableData
    {
        public TouchUiType touchUiType;

        [Range(0f, 1f)]
        public float touchUiSpacingX = 0;

        [Range(0f, 1f)]
        public float touchUiSpacingY = 0;

        [Range(0.5f, 1.5f)]
        public float touchUiScale= 0;

        [Range(0f, 1f)]
        public float musicVolume = 1;

        [Range(0f, 1f)]
        public float effectsVolume = 1;

        string SaveableData.GetFileName()
        {
            return name;
        }

        public enum TouchUiType
        {
            Arrows,
            ArrowsSeparate,
            Joystick,
            None
        }


    }
}