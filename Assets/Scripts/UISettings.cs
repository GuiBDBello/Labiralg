using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISettings : MonoBehaviour {

    public GameObject panelMain;
    public GameObject panelSettings;

    // Start is called before the first frame update
    void Start() {
        
    }

    // Update is called once per frame
    void Update() {
        
    }

    public void ButtonSettingsPressed() {
        panelMain.GetComponent<RectTransform>().anchoredPosition = new Vector2(-500.0f, 500.0f);
        panelSettings.GetComponent<RectTransform>().anchoredPosition = new Vector2(0.0f, 0.0f);
    }

    public void ButtonBackPressed() {
        panelMain.GetComponent<RectTransform>().anchoredPosition = new Vector2(0.0f, 0.0f);
        panelSettings.GetComponent<RectTransform>().anchoredPosition = new Vector2(500.0f, -500.0f);
    }
}
