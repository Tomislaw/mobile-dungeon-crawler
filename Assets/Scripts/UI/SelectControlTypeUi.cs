using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectControlTypeUi : MonoBehaviour
{
    private Toggle toggle;
    public OptionsData.TouchUiType type;
    public OptionsData data;
    void Start()
    {
        toggle = GetComponent<Toggle>();
        toggle.isOn = type == data.touchUiType;
        toggle.onValueChanged.AddListener(OnChange);
    }

    public void OnChange(bool change)
    {
        if (change)
            data.touchUiType = type;
    }

}
