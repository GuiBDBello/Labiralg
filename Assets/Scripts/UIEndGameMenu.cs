using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIEndGameMenu : MonoBehaviour
{
    public static Text textTime;
    public static Text textItems;
    public static Text textMazes;
    public static Text textTotalScore;

    public void ButtonRestartPressed()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ButtonMenuPressed()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }
}
