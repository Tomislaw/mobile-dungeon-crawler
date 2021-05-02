using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlsUi : MonoBehaviour
{
    public OptionsData options;

    public GameObject arrows1;
    public GameObject arrows2;

    public GameObject arrowsSeparate;
    public GameObject joystick;
    public GameObject attack;

    // Update is called once per frame
    void Update()
    {
        switch (options.touchUiType)
        {
            case OptionsData.TouchUiType.Arrows1:
                arrows1.SetActive(true);
                arrows2.SetActive(false);
                arrowsSeparate.SetActive(false);
                joystick.SetActive(false);
                attack.SetActive(true);
                break;
            case OptionsData.TouchUiType.Arrows2:
                arrows1.SetActive(false);
                arrows2.SetActive(true);
                arrowsSeparate.SetActive(false);
                joystick.SetActive(false);
                attack.SetActive(true);
                break;
            case OptionsData.TouchUiType.ArrowsSeparate:
                arrows1.SetActive(false);
                arrows2.SetActive(false);
                arrowsSeparate.SetActive(true);
                joystick.SetActive(false);
                attack.SetActive(true);
                break;
            case OptionsData.TouchUiType.Joystick:
                arrows1.SetActive(false);
                arrows2.SetActive(false);
                arrowsSeparate.SetActive(false);
                joystick.SetActive(true);
                attack.SetActive(true);
                break;
            case OptionsData.TouchUiType.None:
                arrows1.SetActive(false);
                arrows2.SetActive(false);
                arrowsSeparate.SetActive(false);
                joystick.SetActive(false);
                attack.SetActive(false);
                break;
        }
    }
}