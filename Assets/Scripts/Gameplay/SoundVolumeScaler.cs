using UnityEngine;
using UnityEngine.UI;

namespace RuinsRaiders
{
    public class SoundVolumeScaler : MonoBehaviour
    {
        [SerializeField]
        private OptionsData data;

        [SerializeField]
        private Type soundType = Type.Effects;

        private AudioSource _source;
        private Slider _slider;
        public enum Type
        {
            Music, Effects
        }
        void Start()
        {
            _source = GetComponent<AudioSource>();
            _slider = GetComponent<Slider>();
            if (_slider != null)
                if (soundType == Type.Effects)
                    _slider.value = data.effectsVolume;
                else
                    _slider.value = data.musicVolume;
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            if (_source != null)
                _source.volume = soundType == Type.Effects ? data.effectsVolume : data.musicVolume;
            if (_slider != null)
                if (soundType == Type.Effects)
                    data.effectsVolume = _slider.value;
                else
                    data.musicVolume = _slider.value;
        }
    }
}