using UnityEngine;

public class UIMainMenu : MonoBehaviour {

    public Animator animator;

    // Start is called before the first frame update
    void Start() {
        
    }

    // Update is called once per frame
    void Update() {
        
    }

    public void ButtonSettingsPressed() {
        animator.SetTrigger("Change Menu");
    }
}
