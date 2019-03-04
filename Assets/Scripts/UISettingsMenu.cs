using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class UISettingsMenu : MonoBehaviour {

    public GameObject panelMain;
    public GameObject panelSettings;
    public AudioMixer audioMixer;
    public Dropdown resolutionDropdown;

    Resolution[] resolutions;

    // Start is called before the first frame update
    void Start() {
        resolutions = Screen.resolutions;

        resolutionDropdown.ClearOptions();

        List<string> options = new List<string>();

        int currentResolutionIndex = 0;
        
        for (int i = 0; i < resolutions.Length; i++) {
            options.Add(resolutions[i].width + " x " + resolutions[i].height);

            if (resolutions[i].width == Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height) {
                currentResolutionIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
    }

    // Update is called once per frame
    void Update() {
        
    }

    public void SetVolume(float volume) {
        audioMixer.SetFloat("Volume", volume);
    }

    public void SetQuality(int qualityIndex) {
        QualitySettings.SetQualityLevel(qualityIndex);
    }

    public void SetFullScreen(bool isFullScreen) {
        Screen.fullScreen = isFullScreen;
    }

    public void SetResolution(int resolutionIndex) {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }

    public void ButtonBackPressed() {
        panelMain.GetComponent<RectTransform>().anchoredPosition = new Vector2(0.0f, 0.0f);
        panelSettings.GetComponent<RectTransform>().anchoredPosition = new Vector2(500.0f, 0.0f);
    }
}
