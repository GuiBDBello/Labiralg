using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

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
    public Joybutton joybutton;
    
    private Vector3 movement;
    private Vector3 jump;
    private Rigidbody rb;
    private float initialMass;
    private float initialDrag;
    private float initialSpeed;
    private float moveHorizontal;
    private float moveVertical;

    // Inicia os valores das propriedades
    private void Init ()
    {
        jump = new Vector3(0.0f, 1.0f, 0.0f);
        rb = GetComponent<Rigidbody>();
        initialMass = rb.mass;
        initialDrag = rb.drag;
        initialSpeed = speed;
        isPlayable = true;
        dashQuantity = 0;
        dashForce = 20f;
    }

    // Método chamado no primeiro frame que o script é ativo
    private void Start ()
    {
        Init();
    }

    // Método chamado antes de renderizar um frame
    private void Update ()
    {
        // TODO: Debug only, must be removed before release;
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        if (isPlayable)
        {
            uiGameHUD.UpdateCronometer();
        }
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
    }

    // Chamado quando ocorre uma colisão no objeto que esse script é atribuído
    private void OnTriggerEnter (Collider other)
    {
        if (other.gameObject.CompareTag(Tags.PickUp))
        {
            other.gameObject.SetActive(false);
            uiGameHUD.PickUpCollected();
        }
        if (other.gameObject.CompareTag(Tags.Portal))
        {
            float timeGained = (maze.xSize + maze.zSize) / 4f;
            uiGameHUD.PortalReached(timeGained);
        }
        //Destroy(other.gameObject);
    }

    // Movimenta o Jogador
    private void Move ()
    {
        moveHorizontal = Input.GetAxis("Horizontal") + joystick.Horizontal;
        moveVertical = Input.GetAxis("Vertical") + joystick.Vertical;

        //Debug.Log(moveHorizontal);
        //Debug.Log(moveVertical);

        movement = new Vector3(moveHorizontal, 0.0f, moveVertical);
        rb.AddForce(movement * speed);
    }

    // Realiza um 'Dash';
    private void Dash ()
    {
        // Realiza um "dash" na direção pressionada,
        // enquanto aumenta a massa e diminui a velocidade,
        // deixando o Player mais lento com o passar do tempo
        if (joybutton.Pressed)
        {
            if (dashQuantity > 0)
            {
                rb.AddForce(movement * speed * dashForce);
                rb.mass = initialMass * dashForce;
                speed = speed / 1.5f;
                dashQuantity--;
            }
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
}