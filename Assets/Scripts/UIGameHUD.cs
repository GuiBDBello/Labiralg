using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class UIGameHUD : MonoBehaviour
{
    public float cronometer;
    public PlayerController playerController;
    public Maze maze;
    public GameObject pausePanel;
    public GameObject endGamePanel;
    public Text scoreText;
    public Text timeText;
    public Text dashText;
    public Text popUp;

    private long score;
    private long levelScore;
    private long pickupScore;
    private long portalScore;
    private float timeSurvived;
    private int pickUpsCollected;
    private UIGameHUD hud;
    private UIEndGameMenu uiEndGameMenu;
    private Color textColor;

    void Start()
    {
        score = 0;
        levelScore = maze.xSize * maze.zSize;
        pickupScore = 50;
        portalScore = 100;
        timeSurvived = 0;
        pickUpsCollected = 0;

        hud = GameObject.FindWithTag(Tags.UIGameHUD).GetComponent<UIGameHUD>();
        hud = gameObject.GetComponent<UIGameHUD>();
        uiEndGameMenu = endGamePanel.GetComponent<UIEndGameMenu>();
    }

    void Update()
    {
        UpdateHUD();
    }

    public void ButtonPausePressed()
    {
        playerController.isPlayable = false;
        pausePanel.SetActive(true);
    }

    public void ButtonReturnPressed()
    {
        playerController.isPlayable = true;
        pausePanel.SetActive(false);
    }

    public void ShowText(float time, string textValue, Transform lerpTo)
    {
        popUp.text = textValue;
        StartCoroutine(FadeAway(time, popUp, lerpTo));
    }

    public void PickUpCollected()
    {
        int dashAdded = 10;
        playerController.dashQuantity += dashAdded;
        pickUpsCollected++;
        score += pickupScore;
        ShowText(1f, "+ " + pickupScore, scoreText.transform);
        ShowText(1f, "+ " + dashAdded, dashText.transform);
    }

    public void PortalReached(float timeGained)
    {
        long scoreAdded = ((long)(portalScore)) + (maze.xSize * maze.zSize);
        cronometer += timeGained;
        score += scoreAdded;
        playerController.isPlayable = false;
        playerController.zoom.ChangeZoom();
        ShowText(1f, "+ " + scoreAdded, scoreText.transform);
        ShowText(1f, "+ " + timeGained+"s", timeText.transform);
    }

    public void UpdateCronometer()
    {
        if (cronometer > 0.0f)
        {
            Countdown();
        }
        else
        {
            TimesUp();
        }
    }

    private void UpdateHUD()
    {
        scoreText.text = "Pontuação: " + score;
        timeText.text = "Tempo: " + Math.Round(cronometer, 2);
        dashText.text = playerController.dashQuantity.ToString();
    }

    private void Countdown()
    {
        cronometer -= Time.deltaTime;
        timeSurvived += Time.deltaTime;
    }

    private void TimesUp()
    {
        cronometer = 0.0f;
        playerController.isPlayable = false;
        endGamePanel.SetActive(true);
        uiEndGameMenu.textTime.text = "Você sobreviveu por " + Math.Round(timeSurvived, 2) + "s";
        uiEndGameMenu.textItems.text = "Itens coletados: " + pickUpsCollected;
        uiEndGameMenu.textMazes.text = "Labirintos concluídos: " + (maze.xSize - 5);
        uiEndGameMenu.textTotalScore.text = "Pontuação total: " + score;
    }

    IEnumerator FadeAway(float time, Text text, Transform lerpTo)
    {
        text = Instantiate(text, transform.position, Quaternion.identity);
        text.gameObject.SetActive(true);
        text.transform.SetParent(hud.gameObject.transform);
        
        textColor = text.color;
        textColor.a = 1;

        float fading = 0;
        while (text.color.a > 0)
        {
            GoUpwards(text);
            fading += Time.deltaTime / time;
            textColor.a = Mathf.Lerp(1, 0, fading);
            text.transform.position = Vector3.Lerp(transform.position - new Vector3(popUp.rectTransform.rect.width / 2, 0, 0) + new Vector3(0, popUp.rectTransform.rect.height / 2, 0), lerpTo.position, fading);
            text.color = textColor;
            
            yield return null;
        }
        Destroy(text.gameObject, time);
    }

    private void GoUpwards(Text text)
    {
        text.transform.position += Vector3.up;
    }
}
