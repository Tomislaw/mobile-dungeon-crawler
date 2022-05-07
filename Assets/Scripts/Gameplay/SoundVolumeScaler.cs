using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundVolumeScaler : MonoBehaviour
{
    public OptionsData data;

    public Type soundType = Type.Effects;

    private AudioSource source;
    private Slider slider;
    public enum Type
    {
        Music, Effects
    }
    void Start()
    {
        source = GetComponent<AudioSource>();
        slider = GetComponent<Slider>();
        if (slider != null)
            if (soundType == Type.Effects)
                 slider.value = data.effectsVolume;
            else
                slider.value = data.musicVolume;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (source != null)
            source.volume = soundType == Type.Effects ? data.effectsVolume : data.musicVolume;
        if (slider != null)
            if (soundType == Type.Effects)
                data.effectsVolume = slider.value;
            else
                data.musicVolume = slider.value;
    }
}
