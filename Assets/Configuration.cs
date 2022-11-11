using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class Configuration : MonoBehaviour
{
    string AIM_ASSIST_KEY = "aimAssist";
    string INVINCE_AFTER_SWAP_KEY = "selfInvincibleAfterSwap";
    string DEBUG_LASERS_KEY = "debugLasers";
    [SerializeField]
    public bool selfInvincibleAfterSwap = false;
    public bool aimAssist = false;
    public bool debugLasers = false;
    public Toggle selfInvincibleAfterSwapToggle;
    public Toggle aimAssistToggle;
    public Toggle debugLasersToggle;

    public static Configuration Instance;
    void Awake() {
        if (Instance != null) {
            Destroy(gameObject);
        }
        Instance = this;
        if (PlayerPrefs.HasKey(INVINCE_AFTER_SWAP_KEY))
        {
            selfInvincibleAfterSwapToggle.isOn = PlayerPrefs.GetString(INVINCE_AFTER_SWAP_KEY) == "True";
        }
        if (PlayerPrefs.HasKey(AIM_ASSIST_KEY))
        {
            aimAssistToggle.isOn = PlayerPrefs.GetString(AIM_ASSIST_KEY) == "True";
        }
        if (PlayerPrefs.HasKey(DEBUG_LASERS_KEY))
        {
            debugLasersToggle.isOn = PlayerPrefs.GetString(DEBUG_LASERS_KEY) == "True";
        }
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


}
