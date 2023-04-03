using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace RuinsRaiders.UI
{
    public class ControlTypeListenerUI : MonoBehaviour
    {
        public GameObject onVirtualArrows;
        public GameObject onVirtualJoystick;

        public GameObject onArrows;
        public GameObject onWsad;
        public GameObject onJoystick;

        public OptionsData data;

        private InputMethod method = InputMethod.Unknown;
        

        // Update is called once per frame
        void Update()
        {
            if (data != null && data.touchUiType != OptionsData.TouchUiType.None)
            {
                var type = data.touchUiType switch
                {
                    OptionsData.TouchUiType.Arrows => InputMethod.VirtualArrows,
                    OptionsData.TouchUiType.ArrowsSeparate => InputMethod.VirtualArrows,
                    OptionsData.TouchUiType.Joystick => InputMethod.VirtualJoystick,
                    OptionsData.TouchUiType.None => InputMethod.Joystick,
                    _ => InputMethod.Joystick,
                };

                if (type != method)
                    Resolve(type);
            }
        }

        private void Resolve(InputMethod method)
        {
            this.method = method;
            onVirtualArrows.SetActive(false);
            onVirtualJoystick.SetActive(false);
            onArrows.SetActive(false);
            onWsad.SetActive(false);
            onJoystick.SetActive(false);
            GameObject enabled = method switch
            {
                InputMethod.VirtualArrows => onVirtualArrows,
                InputMethod.VirtualJoystick => onVirtualJoystick,
                InputMethod.Arrows => onArrows,
                InputMethod.Wsad => onWsad,
                InputMethod.Joystick => onJoystick,
                _ => onWsad,
            };
            enabled.SetActive(true);
        }

        enum InputMethod
        {
            VirtualArrows, VirtualJoystick, Arrows, Wsad, Joystick, Unknown
        }

    }
}
