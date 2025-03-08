using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour {
    //public Slider sensitivitySlider;
    public Toggle ScreenTintEnabledToggle;

    //public static float mouseSensitivity = 1;
    
    private void Start() {
        // Load saved sensitivity from PlayerPrefs or default to 1.0 if not set
        //float savedSensitivity = PlayerPrefs.GetFloat("MouseSensitivity", 1.0f);
        //sensitivitySlider.value = savedSensitivity;
        bool ScreenTintEnabled = PlayerPrefs.GetInt("ScreenTintEnabled", 1) == 1;
        ScreenTintEnabledToggle.isOn = ScreenTintEnabled;
    }
    // public void SetMouseSensitivity(float sensitivity) {
    //     mouseSensitivity = sensitivity;
    //     PlayerPrefs.SetFloat("MouseSensitivity", sensitivity); // Save to PlayerPrefs
    // }
    public void SetUseScreenTint(bool b) {
        if (b) {
            PlayerPrefs.SetInt("ScreenTintEnabled", 1); // Save to PlayerPrefs
        }
        else {
            PlayerPrefs.SetInt("ScreenTintEnabled", 0); // Save to PlayerPrefs
        }
        
    }
}