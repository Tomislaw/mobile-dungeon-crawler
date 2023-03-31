using UnityEngine;

namespace RuinsRaiders.UI
{
    // responsible for showing correct mobile input controls
    public class ControlsUi : MonoBehaviour
    {
        [SerializeField]
        private OptionsData options;

        [SerializeField]
        private GameObject arrows;
        [SerializeField]
        private GameObject arrowsSeparate;
        [SerializeField]
        private GameObject joystick;

        // Update is called once per frame
        void Update()
        {
            switch (options.touchUiType)
            {
                case OptionsData.TouchUiType.Arrows:
                    arrows.SetActive(true);
                    arrowsSeparate.SetActive(false);
                    joystick.SetActive(false);
                    break;
                case OptionsData.TouchUiType.ArrowsSeparate:
                    arrows.SetActive(false);
                    arrowsSeparate.SetActive(true);
                    joystick.SetActive(false);
                    break;
                case OptionsData.TouchUiType.Joystick:
                    arrows.SetActive(false);
                    arrowsSeparate.SetActive(false);
                    joystick.SetActive(true);
                    break;
                case OptionsData.TouchUiType.None:
                    arrows.SetActive(false);
                    arrowsSeparate.SetActive(false);
                    joystick.SetActive(false);
                    break;
            }
        }
    }
}