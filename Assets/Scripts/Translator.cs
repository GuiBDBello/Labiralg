using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Translator : MonoBehaviour {

    public int rotationSpeed;
    private Vector3 origin = new Vector3(0.0f, 0.0f, 0.0f);

    // Update is called once per frame
    void FixedUpdate() {
        transform.LookAt(origin);
        transform.Translate(Vector3.right * rotationSpeed * Time.deltaTime);
    }
}