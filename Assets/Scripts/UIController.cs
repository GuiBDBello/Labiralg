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

    public void OnButtonPostToLeaderboard()
    {
        long scoreToPost;

        if (long.TryParse(score, out scoreToPost))
        {
            GPGS.PostToLeaderboard(scoreToPost);
        }
        else
        {
            Debug.Log("Error: Could not post score to leaderboard.Please enter a valid score value.");
        }
    }

    public void OnButtonShowLeaderboard()
    {
        Debug.Log("Showing Leaderboard");
        GPGS.ShowLeaderboardUI();
    }
}