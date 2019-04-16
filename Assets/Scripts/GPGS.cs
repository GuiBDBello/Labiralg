using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine.SocialPlatforms;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GPGS : MonoBehaviour
{
    public static bool isAuthenticated = false;

    // Start is called before the first frame update
    void Start()
    {
        if (!isAuthenticated)
        {
            AuthenticateUser();
        }
    }

    private static void AuthenticateUser()
    {
        PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder().Build();
        PlayGamesPlatform.InitializeInstance(config);
        PlayGamesPlatform.Activate();
        Social.localUser.Authenticate((bool success) =>
        {
            if (success == true)
            {
                isAuthenticated = true;
                Debug.Log("Logged in to Google Play Services");
            }
            else
            {
                isAuthenticated = false;
                Debug.LogError("Unable to sign in to Google Play Games Services");
            }
        });
    }

    public static void PostToLeaderboard(long newScore)
    {
        if (!isAuthenticated)
        {
            AuthenticateUser();
        }

        Social.ReportScore(newScore, GPGSIds.leaderboard_high_score, (bool success) =>
        {
            if (success)
            {
                Debug.Log("Posted new score to leaderboard");
            }
            else
            {
                Debug.Log("Unable to post new score to leaderboard");
            }
        });
    }

    public static void ShowLeaderboardUI()
    {
        if (!isAuthenticated)
        {
            AuthenticateUser();
        }
        else
        {
            PlayGamesPlatform.Instance.ShowLeaderboardUI(GPGSIds.leaderboard_high_score);
        }
        
    }
}
