using UnityEngine;

public class Zoom : MonoBehaviour
{
    private Animator animator;

    // Start is called before the first frame update
    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void ChangeZoom()
    {
        animator.SetTrigger(Animations.Zoom);
    }
}
