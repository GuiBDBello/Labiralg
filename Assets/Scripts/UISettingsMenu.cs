using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class UISettingsMenu : MonoBehaviour {
    
    public AudioMixer audioMixer;
    public Dropdown resolutionDropdown;
    public Animator animator;

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
        animator.SetTrigger("Change Menu");
    }
}
