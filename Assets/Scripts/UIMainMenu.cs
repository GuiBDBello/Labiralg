using UnityEngine;

public class UIMainMenu : MonoBehaviour
{
    public Animator animator;

    public void ButtonSettingsPressed ()
    {
        animator.SetTrigger(Animations.ChangeMenu);
    }
}
