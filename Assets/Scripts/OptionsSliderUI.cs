using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OptionsSliderUI : MonoBehaviour
{

    public Slider slider;
    public TextMeshProUGUI volText;

    public Toggle toggle;
    public Image toggleImage;
    public Sprite muted;
    public Sprite notMuted;

    public OptionType prefsKey;
    private void Start()
    {
        switch(prefsKey)
        {
            case OptionType.MASTERVOLUME:
                slider.value = Configuration.MasterVolume;
                volText.text = Configuration.MasterVolume.ToString();
                toggle.isOn = Configuration.MasterMuted == 1;
                toggleImage.sprite = toggle.isOn ? notMuted : muted;
                
                break;
            case OptionType.MUSICVOLUME:
                slider.value = Configuration.MusicVolume;
                toggle.isOn = Configuration.MusicMuted == 1;
                volText.text = Configuration.MusicVolume.ToString();
                toggleImage.sprite = toggle.isOn ? notMuted : muted;
                break;
            case OptionType.SFXVOLUME:
                slider.value = Configuration.SFXVolume;
                toggle.isOn = Configuration.SFXMuted == 1;
                volText.text = Configuration.SFXVolume.ToString();
                toggleImage.sprite = toggle.isOn ? notMuted : muted;
                break;
            
        }

        Debug.Log("### toggle is on " + toggle.isOn);

        slider.onValueChanged.AddListener((x) =>
        {
            volText.text = x.ToString();
        });

        slider.onValueChanged.AddListener((x) =>
        {
            switch (prefsKey)
            {
                case OptionType.MASTERVOLUME:
                    Configuration.MasterVolume = (int)x;
                    break;
                case OptionType.MUSICVOLUME:
                    Configuration.MusicVolume = (int)x;
                    break;
                case OptionType.SFXVOLUME:
                    Configuration.SFXVolume = (int)x;
                    break;
            }
            Configuration.AdjustVolume(prefsKey);
        });

        toggle.onValueChanged.AddListener((x) =>
        {
            Debug.Log("TOGGLE CHANGED " + x);
            if (x)
            {
                toggleImage.sprite = notMuted;
            }
            else
            {
                toggleImage.sprite = muted;
            }
        });

        toggle.onValueChanged.AddListener((x) =>
        {
            int y = x ? 1 : 0;
            Debug.Log("### " + x + " " + y);
            switch (prefsKey)
            {
                case OptionType.MASTERVOLUME:
                    Configuration.MasterMuted = y;
                    break;
                case OptionType.MUSICVOLUME:
                    Configuration.MusicVolume = y;
                    break;
                case OptionType.SFXVOLUME:
                    Configuration.SFXMuted = y;
                    break;
            }
            Configuration.AdjustVolume(prefsKey);
        });
    }
}

public enum OptionType
{
    MASTERVOLUME,
    MUSICVOLUME,
    SFXVOLUME
}
