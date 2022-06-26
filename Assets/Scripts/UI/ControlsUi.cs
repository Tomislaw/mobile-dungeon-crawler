using UnityEngine;

namespace RuinsRaiders.UI
{
    // responsible for showing correct mobile input controls
    public class ControlsUi : MonoBehaviour
    {
        [SerializeField]
        private OptionsData options;

        [SerializeField]
        private GameObject arrows1;
        [SerializeField]
        private GameObject arrows2;
        [SerializeField]
        private GameObject arrowsSeparate;
        [SerializeField]
        private GameObject joystick;
        [SerializeField]
        private GameObject attack;

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
}