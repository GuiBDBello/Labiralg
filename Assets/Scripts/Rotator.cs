using UnityEngine;

// Classe criada para girar um Objeto;
public class Rotator : MonoBehaviour
{
    private Vector3 rotationAxis;

    private void Init ()
    {
        rotationAxis = new Vector3(15, 30, 45);
    }

    private void Start ()
    {
        Init();
    }
    void Update ()
    {
        transform.Rotate(rotationAxis * Time.deltaTime);
    }
}