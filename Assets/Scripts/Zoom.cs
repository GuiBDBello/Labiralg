using UnityEngine;

public class Zoom : MonoBehaviour
{
    private Animator animator;
    private Vector3 playerStartPosition;

    public Maze maze;

    // Start is called before the first frame update
    private void Start()
    {
        animator = GetComponent<Animator>();
        playerStartPosition = new Vector3(0.0f, 0.5f, 0.0f);
    }

    public void ChangeZoom()
    {
        animator.SetTrigger(Animations.Zoom);
    }

    public void OnZoomComplete()
    {
        GameObject.FindGameObjectWithTag(Tags.Player).GetComponent<Rigidbody>().transform.position = playerStartPosition;
        maze.NextMaze();
    }
}
