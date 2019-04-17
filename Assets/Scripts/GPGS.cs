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
    // Start is called before the first frame update
    void Start()
    {
        AuthenticateUser();
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
                Debug.Log("Logged in to Google Play Services");
            }
            else
            {
                Debug.LogError("Unable to sign in to Google Play Games Services");
            }
        });
    }

    public static void PostToLeaderboard(long newScore)
    {
        AuthenticateUser();

        Social.ReportScore(newScore, GPGSIds.leaderboard_maior_pontuacao, (bool success) =>
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
        AuthenticateUser();
        PlayGamesPlatform.Instance.ShowLeaderboardUI();//GPGSIds.leaderboard_maior_pontuacao);
    }
}
