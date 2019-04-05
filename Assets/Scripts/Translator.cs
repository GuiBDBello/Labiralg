using UnityEngine;

// Classe criada para girar a Câmera ao redor da Origem (posição 0, 0, 0 da Cena) no Menu Inicial;
public class Translator : MonoBehaviour
{
    public int rotationSpeed;

    private Vector3 origin;

    private void Init ()
    {
        origin = new Vector3(0.0f, 0.0f, 0.0f);
    }

    private void Start ()
    {
        Init();
    }
    
    private void FixedUpdate ()
    {
        SpinAroundOrigin();
    }

    private void SpinAroundOrigin ()
    {
        transform.LookAt(origin);
        transform.Translate(Vector3.right * rotationSpeed * Time.deltaTime);
    }
}