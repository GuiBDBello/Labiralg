using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public float speed;
    public float jumpForce;
    public float jumpDelay;
    public bool isPlayable;
    public int dashQuantity;
    public float dashForce;
    public Maze maze;
    public Zoom zoom;
    public UIGameHUD uiGameHUD;
    public Joystick joystick;
    public Joybutton dashJoybutton;
    public Joybutton minimapJoybutton;
    public AudioClip playerDashSound;
    public AudioClip pickUpCollectedSound;
    public AudioClip reachPortalSound;
    public Camera mainCamera;
    
    private Vector3 movement;
    private Vector3 jump;
    private Rigidbody rb;
    private float initialMass;
    private float initialDrag;
    private float initialSpeed;
    private float moveHorizontal;
    private float moveVertical;
    private float transitionTime;
    private float minimapCameraHeight;

    // Método chamado no primeiro frame que o script é ativo
    private void Start ()
    {
        jump = new Vector3(0.0f, 1.0f, 0.0f);
        rb = GetComponent<Rigidbody>();
        initialMass = rb.mass;
        initialDrag = rb.drag;
        initialSpeed = speed;
        isPlayable = true;
        dashQuantity = 0;
        dashForce = 20f;
        transitionTime = 0;
        minimapCameraHeight = 35F;
    }

    // Método chamado antes de renderizar um frame
    private void Update ()
    {
        // TODO: Apenas para Debug, deve ser removido antes do lançamento;
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        uiGameHUD.UpdateCronometer();
    }

    // Chamado antes de realizar cálculos de física
    private void FixedUpdate ()
    {
        if (isPlayable)
        {
            Move();
            Dash();
            //Jump();
        }
        ShowMinimap();
    }

    // Chamado quando ocorre uma colisão no objeto que esse script é atribuído
    private void OnTriggerEnter (Collider other)
    {
        if (other.gameObject.CompareTag(Tags.PickUp))
        {
            other.gameObject.SetActive(false);
            uiGameHUD.PickUpCollected();
            AudioController.audioSource.PlayOneShot(pickUpCollectedSound);
        }
        if (other.gameObject.CompareTag(Tags.Portal))
        {
            float timeGained = (maze.xSize + maze.zSize) / 4f;
            uiGameHUD.PortalReached(timeGained);
            AudioController.audioSource.PlayOneShot(reachPortalSound);
            minimapCameraHeight += 6.5F;
        }
        //Destroy(other.gameObject);
    }

    // Movimenta o Jogador
    private void Move ()
    {
        moveHorizontal = Input.GetAxis("Horizontal") + joystick.Horizontal;
        moveVertical = Input.GetAxis("Vertical") + joystick.Vertical;

        movement = new Vector3(moveHorizontal, 0.0f, moveVertical);
        rb.AddForce(movement * speed);
    }

    // Realiza um 'Dash';
    private void Dash ()
    {
        // Realiza um "dash" na direção pressionada,
        // enquanto aumenta a massa e diminui a velocidade,
        // deixando o Player mais lento com o passar do tempo
        if (dashJoybutton.Pressed && dashQuantity > 0)
        {
            rb.AddForce(movement * speed * dashForce);
            rb.mass = initialMass * dashForce;
            speed = speed / 1.5f;
            dashQuantity--;
            AudioController.audioSource.PlayOneShot(playerDashSound);
        }
        else
        {
            rb.mass = initialMass;
            speed = initialSpeed;
        }
    }

    // Faz o Jogador pular
    private void Jump ()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (transform.position.y < 0.75 && jumpDelay <= 0)
            {
                rb.AddForce(jump * jumpForce);
                //rb.drag = 1;
                jumpDelay = 1;
            }
            /*
            else
            {
                rb.drag = initialDrag;
            }
            */
        }
        jumpDelay -= Time.deltaTime;
    }

    private void ShowMinimap()
    {
        if (minimapJoybutton.Pressed)
        {
            isPlayable = false;
            transitionTime += Time.deltaTime / 1;
            mainCamera.GetComponent<CameraController>().enabled = false;
            mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, new Vector3(0F, minimapCameraHeight, 0F), transitionTime);
            mainCamera.transform.localRotation = Quaternion.Lerp(mainCamera.transform.localRotation, Quaternion.Euler(90F, 0F, 0F), transitionTime);
        }
        else
        {
            isPlayable = true;
            transitionTime = 0;
            mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, new Vector3(0F, 15.5F, -15F), transitionTime);
            mainCamera.transform.localRotation = Quaternion.Euler(45F, 0F, 0F);
            mainCamera.GetComponent<CameraController>().enabled = true;
        }
    }
}