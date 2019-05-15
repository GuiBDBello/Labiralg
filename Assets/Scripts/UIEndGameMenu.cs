using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIEndGameMenu : MonoBehaviour
{
    public Text textTime;
    public Text textItems;
    public Text textMazes;
    public Text textTotalScore;

    public void ButtonRestartPressed()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ButtonMenuPressed()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }
}