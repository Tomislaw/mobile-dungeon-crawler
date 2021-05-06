using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

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
