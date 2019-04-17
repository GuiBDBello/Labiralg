using UnityEngine;
using UnityEngine.SceneManagement;

using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    string score;

    public void ButtonSurvivalPressed ()
    {
        SceneManager.LoadScene(Scenes.SceneSurvival);
    }

    public void ButtonQuitPressed ()
    {
        Application.Quit();
    }

    public void OnButtonShowLeaderboard()
    {
        Debug.Log("Showing Leaderboard");
        GPGS.ShowLeaderboardUI();
    }
}