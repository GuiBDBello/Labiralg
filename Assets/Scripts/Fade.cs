using UnityEngine;
using UnityEngine.SceneManagement;

public class Fade : MonoBehaviour {

    public Animator animator;

    private int LevelToLoad { get; set; }

    // Update is called once per frame
    void Update() {
        if (Input.GetMouseButtonDown(0)) {
            //FadeToLevel(1);
        }
    }

    public void FadeToLevel(int levelIndex) {
        this.LevelToLoad = levelIndex;
        animator.SetTrigger("Fade Out");
    }

    public void OnFadeComplete() {
        SceneManager.LoadScene(this.LevelToLoad);
    }
}
