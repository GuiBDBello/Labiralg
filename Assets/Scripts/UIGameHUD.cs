using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class UIGameHUD : MonoBehaviour
{
    public float cronometer;
    public PlayerController playerController;
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
        levelScore = playerController.maze.xSize * playerController.maze.zSize;
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
        playerController.dashQuantity += 10;
        pickUpsCollected++;
        score += pickupScore;
        ShowText(1f, "+" + pickupScore, scoreText.transform);
    }

    public void PortalReached(float timeGained)
    {
        cronometer += timeGained;
        score += ((long)(portalScore)) + (playerController.maze.xSize * playerController.maze.zSize);
        playerController.isPlayable = false;
        playerController.zoom.ChangeZoom();
        ShowText(0.8f, "+" + timeGained, timeText.transform);
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
        timeSurvived += Time.deltaTime;
    }

    private void Countdown()
    {
        cronometer -= Time.deltaTime;
    }

    private void TimesUp()
    {
        cronometer = 0.0f;
        playerController.isPlayable = false;
        endGamePanel.SetActive(true);
        uiEndGameMenu.textTime.text = "Você sobreviveu por " + Math.Round(timeSurvived, 2) + "s";
        uiEndGameMenu.textItems.text = "Itens coletados: " + pickUpsCollected;
        uiEndGameMenu.textMazes.text = "Labirintos concluídos: " + (playerController.maze.xSize - 5);
        uiEndGameMenu.textTotalScore.text = "Pontuação total: " + score;

        GPGS.PostToLeaderboard(score);
    }

    IEnumerator FadeAway(float time, Text text, Transform lerpTo)
    {
        float contador = Time.deltaTime / time;
        text = Instantiate(text, transform.position, Quaternion.identity);
        //text.transform.position = new Vector3(hud.transform.position.x, hud.transform.position.y + 200, 0);
        text.gameObject.SetActive(true);
        text.transform.parent = hud.gameObject.transform;

        //Vector3 direction = lerpTo.position - transform.position;

        textColor = text.color;
        textColor.a = 1;

        float fading = 0;
        while (text.color.a > 0)
        {
            GoUpwards(text);
            fading += Time.deltaTime / time;
            textColor.a = Mathf.Lerp(1, 0, fading);
            text.transform.position = Vector3.Lerp(transform.position, lerpTo.position, fading);
            text.color = textColor;
            
            yield return null;
        }
        Destroy(text, 1);
    }

    private void GoUpwards(Text text)
    {
        text.transform.position += Vector3.up;
    }
}
