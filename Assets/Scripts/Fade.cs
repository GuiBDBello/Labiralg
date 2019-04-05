using UnityEngine;
using UnityEngine.SceneManagement;

public class Fade : MonoBehaviour
{
    public Animator animator;

    private int LevelToLoad { get; set; }

    public void FadeToLevel(int levelIndex)
    {
        LevelToLoad = levelIndex;
        animator.SetTrigger(Animations.FadeOut);
    }

    public void OnFadeComplete ()
    {
        SceneManager.LoadScene(LevelToLoad);
    }
}
