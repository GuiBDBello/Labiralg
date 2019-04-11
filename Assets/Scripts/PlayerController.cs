using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class PlayerController : MonoBehaviour
{
    public float speed;
    public float jumpForce;
    public float jumpDelay;
    public float cronometer;
    public Text scoreText;
    public Text timeText;
    public Text dashText;
    public GameObject endGamePanel;
    public Maze maze;
    public Zoom zoom;

    private Vector3 movement;
    private Vector3 jump;
    private Rigidbody rb;
    private float initialMass;
    private float initialDrag;
    private float initialSpeed;
    private float moveHorizontal;
    private float moveVertical;
    private int dashQuantity;
    private float dashForce;
    private long score;
    private long levelScore;
    private long pickupScore;
    private long portalScore;

    // Inicia os valores das propriedades
    private void Init ()
    {
        jump = new Vector3(0.0f, 1.0f, 0.0f);
        rb = GetComponent<Rigidbody>();
        initialMass = rb.mass;
        initialDrag = rb.drag;
        initialSpeed = speed;
        dashQuantity = 0;
        dashForce = 20f;
        score = 0;
        levelScore = maze.xSize * maze.zSize;
        pickupScore = 10;
        portalScore = 100;
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

        if (cronometer > 0.0f)
        {
            Countdown();
        }
        else
        {
            TimesUp();
        }
        UpdateHUD();
    }

    // Chamado antes de realizar cálculos de física
    private void FixedUpdate ()
    {
        Move();
        Dash();
        //Jump();
    }

    // Chamado quando ocorre uma colisão no objeto que esse script é atribuído
    private void OnTriggerEnter (Collider other)
    {
        if (other.gameObject.CompareTag(Tags.PickUp))
        {
            other.gameObject.SetActive(false);
            dashQuantity+=10;
            score += pickupScore;
        }
        if (other.gameObject.CompareTag(Tags.Portal))
        {
            cronometer += ((maze.xSize + maze.zSize) / 2);
            zoom.ChangeZoom();
            score += ((long) (portalScore));
        }
        //Destroy(other.gameObject);
    }

    // Movimenta o Jogador
    private void Move ()
    {
        moveHorizontal = Input.GetAxis("Horizontal");
        moveVertical = Input.GetAxis("Vertical");

        movement = new Vector3(moveHorizontal, 0.0f, moveVertical);
        rb.AddForce(movement * speed);
    }

    // Realiza um 'Dash';
    private void Dash ()
    {
        // Realiza um "dash" na direção pressionada,
        // enquanto aumenta a massa e diminui a velocidade,
        // deixando o Player mais lento com o passar do tempo
        if (Input.GetKey(KeyCode.LeftShift))
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

    private void UpdateHUD ()
    {
        scoreText.text = "Score: " + score;
        timeText.text = "Time: " + Math.Round(cronometer, 2);
        dashText.text = "Dash: " + dashQuantity;
    }

    private void Countdown()
    {
        cronometer -= Time.deltaTime;
    }
    private void TimesUp()
    {
        cronometer = 0.0f;
        Time.timeScale = 0.0f;
        endGamePanel.SetActive(true);
    }
}