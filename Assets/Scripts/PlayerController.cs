using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed;

    private Rigidbody rb;

    // Chamado no primeiro frame que o script é ativo
    void Start ()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Chamado antes de renderizar um frame
    void Update ()
    {
        
    }

    // Chamado antes de realizar cálculos de física
    void FixedUpdate ()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

	Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);

        rb.AddForce(movement * speed);
    }
}