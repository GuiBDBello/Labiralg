using UnityEngine;
using UnityEngine.SceneManagement;

public class UIController : MonoBehaviour
{
    public void ButtonSurvivalPressed ()
    {
        SceneManager.LoadScene(Scenes.SceneSurvival);
    }

    public void ButtonQuitPressed ()
    {
        Application.Quit();
    }
}