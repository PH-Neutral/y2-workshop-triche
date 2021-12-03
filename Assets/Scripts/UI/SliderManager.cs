using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderManager : MonoBehaviour
{
    public Slider musicSlider, sfxSlider;

    void Start()
    {
        musicSlider.value = MusicManager.instance.musicVolume;
        sfxSlider.value = AudioManager.instance.sfxVolume;
    }
}
