using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;



public class Configuration : MonoBehaviour
{
    string AIM_ASSIST_KEY = "aimAssist";
    string INVINCE_AFTER_SWAP_KEY = "selfInvincibleAfterSwap";
    string DEBUG_LASERS_KEY = "debugLasers";
    [SerializeField]
    public bool selfInvincibleAfterSwap = false;
    public bool aimAssist = false;
    public bool debugLasers = false;

    [HideInInspector]
    public Toggle selfInvincibleAfterSwapToggle;
    [HideInInspector]
    public Toggle aimAssistToggle;
    [HideInInspector]
    public Toggle debugLasersToggle;

    //Options
    public static string MASTER_VOLUME_KEY = "MasterVolume";
    public static int MasterVolume;
    public static string MASTER_MUTED_KEY = "MasterMuted";
    public static int MasterMuted;

    public static string MUSIC_VOLUME_KEY = "MusicVolume";
    public static int MusicVolume;
    public static string MUSIC_MUTED_KEY = "MusicMuted";
    public static int MusicMuted;

    public static string SFX_VOLUME_KEY = "SFXVolume";
    public static int SFXVolume;
    public static string SFX_MUTED_KEY = "SFXMuted";
    public static int SFXMuted;

    //Audio
    public AudioMixer masterMixer;
    private float min_volume = 0.0001f; //avoiding log10 infinity error


    public static Configuration Instance;
    void Awake() {
        if (Instance != null) {
            Destroy(gameObject);
        }
        Instance = this;
        //if (PlayerPrefs.HasKey(INVINCE_AFTER_SWAP_KEY))
        //{
        //    selfInvincibleAfterSwapToggle.isOn = PlayerPrefs.GetString(INVINCE_AFTER_SWAP_KEY) == "True";
        //}
        //if (PlayerPrefs.HasKey(AIM_ASSIST_KEY))
        //{
        //    aimAssistToggle.isOn = PlayerPrefs.GetString(AIM_ASSIST_KEY) == "True";
        //}
        //if (PlayerPrefs.HasKey(DEBUG_LASERS_KEY))
        //{
        //    debugLasersToggle.isOn = PlayerPrefs.GetString(DEBUG_LASERS_KEY) == "True";
        //}

        //Setup Volume Levels from Player Prefs
        MasterVolume = PlayerPrefs.GetInt(MASTER_VOLUME_KEY, 100);
        MusicVolume = PlayerPrefs.GetInt(MUSIC_VOLUME_KEY, 100);
        SFXVolume = PlayerPrefs.GetInt(SFX_VOLUME_KEY, 100);

        //1 is not muted, 0 is muted - Used for multiplication
        MasterMuted = PlayerPrefs.GetInt(MASTER_MUTED_KEY, 1);
        MusicMuted = PlayerPrefs.GetInt(MUSIC_MUTED_KEY, 1);
        SFXMuted = PlayerPrefs.GetInt(SFX_MUTED_KEY, 1);



        Debug.Log("## " + MasterMuted + " " + MusicMuted + " " + SFXMuted );
        Debug.Log("*** Config Finished");
    }

    void Start()
    {
        //Setting Audio Mixer values in Awake doesn't work smh
        //Adjust Volumes
        AdjustVolume(OptionType.MASTERVOLUME);
        AdjustVolume(OptionType.SFXVOLUME);
        AdjustVolume(OptionType.MUSICVOLUME);
    }

    public void Save()
    {
        PlayerPrefs.SetInt(MASTER_VOLUME_KEY, MasterVolume);
        PlayerPrefs.SetInt(MUSIC_VOLUME_KEY, MusicVolume);
        PlayerPrefs.SetInt(SFX_VOLUME_KEY, SFXVolume);
        PlayerPrefs.SetInt(MASTER_MUTED_KEY, MasterMuted);
        PlayerPrefs.SetInt(MUSIC_MUTED_KEY, MusicMuted);
        PlayerPrefs.SetInt(SFX_MUTED_KEY, SFXMuted);

        PlayerPrefs.Save();

    }

    public void OnChangeSelfInvincibleAfterSwap(bool b)
    {
        var newVal = b ? "True" : "False";
        PlayerPrefs.SetString(INVINCE_AFTER_SWAP_KEY, newVal);
        selfInvincibleAfterSwap = b;
    }

    public void OnChangeAimAssist(bool b)
    {
        var newVal = b ? "True" : "False";
        PlayerPrefs.SetString(AIM_ASSIST_KEY, newVal);
        aimAssist = b;
    }

    public  void OnDebugLasers(bool b)
    {
        var newVal = b ? "True" : "False";
        PlayerPrefs.SetString(DEBUG_LASERS_KEY, newVal);
        debugLasers = b;
    }

    public static void AdjustVolume(OptionType optionType)
    {
        switch(optionType)
        {
            case OptionType.MASTERVOLUME:
                Debug.Log("## Master" + MasterVolume / 100f + " " + MasterMuted);
                Instance.masterMixer.SetFloat("MasterVolume", Mathf.Log10(Mathf.Max(MasterVolume / 100f * MasterMuted, Instance.min_volume)) * 20);
                float audio = 0;
                Instance.masterMixer.GetFloat("MasterVolume",out audio);
                Debug.Log("## Set Audio " + audio);

                break;
            case OptionType.SFXVOLUME:
                Instance.masterMixer.SetFloat("SFXVolume", Mathf.Log10(Mathf.Max(SFXVolume / 100f * SFXMuted, Instance.min_volume)) * 20);
                break;
            case OptionType.MUSICVOLUME:
                Instance.masterMixer.SetFloat("MusicVolume", Mathf.Log10(Mathf.Max(MusicVolume * MusicMuted/ 100f, Instance.min_volume)) * 20);
                break;
        }
    }


}
