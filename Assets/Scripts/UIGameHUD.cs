using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIGameHUD : MonoBehaviour
{
    public Text popUp;

    private UIGameHUD hud;
    private Color textColor;

    private void Start()
    {
        hud = GameObject.FindWithTag(Tags.UIGameHUD).GetComponent<UIGameHUD>();
        hud = gameObject.GetComponent<UIGameHUD>();
    }

    public void ShowText(string textValue)
    {
        popUp.text = textValue;
        StartCoroutine(FadeAway(0.5f, popUp));
    }

    void GoUpwards(Text text)
    {
        text.transform.position += Vector3.up;
    }

    IEnumerator FadeAway(float time, Text text)
    {
        text = Instantiate(text, transform.position, Quaternion.identity);
        text.transform.position = new Vector3(hud.transform.position.x, hud.transform.position.y + 100, 0);
        text.gameObject.SetActive(true);
        text.transform.parent = hud.gameObject.transform;

        textColor = text.color;
        textColor.a = 1;

        float fading = 0;
        while (text.color.a > 0)
        {
            GoUpwards(text);
            fading += Time.deltaTime / time;
            textColor.a = Mathf.Lerp(1, 0, fading);
            text.color = textColor;
            if (text.color.a <= 0)
            {
                text.gameObject.SetActive(false);
            }
            yield return null;
        }

        Destroy(popUp);
    }
}
