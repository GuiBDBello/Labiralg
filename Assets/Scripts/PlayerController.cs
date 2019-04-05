using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public float speed;
    public float jumpForce;
    public float jumpDelay;
    public Text scoreText;
    public Text winText;

    private Vector3 movement;
    public Vector3 jump;
    private Rigidbody rb;
    private float initialMass;
    private float initialDrag;
    private float initialSpeed;
    private float moveHorizontal;
    private float moveVertical;
    private float dashForce;
    private float score;

    // Inicia os valores das propriedades
    private void Init ()
    {
        jump = new Vector3(0.0f, 1.0f, 0.0f);
        rb = GetComponent<Rigidbody>();
        initialMass = rb.mass;
        initialDrag = rb.drag;
        initialSpeed = speed;
        dashForce = 20f;
        score = 0;
        SetScoreText();
        winText.text = "";
    }

    // Método chamado no primeiro frame que o script é ativo
    private void Start ()
    {
        Init();
    }

    // Método chamado antes de renderizar um frame
    private void Update ()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    // Chamado antes de realizar cálculos de física
    private void FixedUpdate ()
    {
        Move();
        Dash();
        Jump();
    }

    // Chamado quando ocorre uma colisão no objeto que esse script é atribuído
    private void OnTriggerEnter (Collider other)
    {
        if (other.gameObject.CompareTag(Tags.PickUp))
        {
            other.gameObject.SetActive(false);
            score++;
            SetScoreText();
        }
        if (other.gameObject.CompareTag(Tags.Portal))
        {
            Debug.Log("Tocou no Portal");
        }
        //Destroy(other.gameObject);
    }

    // Movimenta o Jogador
    private void Move()
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
        if (Input.GetKey (KeyCode.LeftShift))
        {
            rb.AddForce(movement * speed * dashForce);
            rb.mass = initialMass * dashForce;
            speed = speed / 1.5f;
        }
        else
        {
            rb.mass = initialMass;
            speed = initialSpeed;
        }
    }

    // Faz o Jogador pular
    private void Jump()
    {
        if (Input.GetKeyDown (KeyCode.Space))
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

    private void SetScoreText ()
    {
        scoreText.text = "Score: " + score.ToString();
    }
}