using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

// Script used for selecting first selected item in none  are selected
// The main idea behind this class was support for gamepads and problems when nothing was selected
// Todo: need teo check if it is needed on current version of Unity, if not then remove
namespace RuinsRaiders.UI
{
    public class UiFirstSelected : MonoBehaviour
    {
        public List<string> Axis;
        public InputAction action;

        // Update is called once per frame
        void Update()
        {
            //if(EventSystem.current.currentSelectedGameObject == null || !EventSystem.current.currentSelectedGameObject.activeInHierarchy)
            //{
            //    foreach(var item in Axis)
            //    {
            //        if (Input.GetAxis(item) != 0)
            //            EventSystem.current.SetSelectedGameObject(gameObject);

            //    }
            //}
        }
    }
}
