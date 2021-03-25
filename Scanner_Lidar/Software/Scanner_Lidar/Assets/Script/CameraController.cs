using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    // Porizioni della camera
    private Vector3 position;
    private Vector3 rotation;

    // Velocità di rotazione della camera
    public float rotationSpeed = 0.00000000000000000000000003f;

    // Velocità di movimento della camera
    private float movementSpeed = 0.5f;

    void Start()
    {
        // Salvo la posizione iniziale della camera
        position = transform.position;
        rotation = transform.eulerAngles;
    }

    void Update()
    {
        // Se il pulsante premuto è W, la camera si muove avanti
        if (Input.GetKey(KeyCode.W))
        {
            position.z += movementSpeed;
            transform.position = position;
        }

        // Se il pulsante premuto è S, la camera si muove indietro
        if (Input.GetKey(KeyCode.S))
        {
            position.z -= movementSpeed;
            transform.position = position;
        }

        // Se il pulsante premuto è A, la camera si muove a sinistra
        if (Input.GetKey(KeyCode.A))
        {
            position.x -= movementSpeed;
            transform.position = position;
        }

        // Se il pulsante premuto è D, la camera si muove a destra
        if (Input.GetKey(KeyCode.D))
        {
            position.x += movementSpeed;
            transform.position = position;
        }

        //Se il pulsante premuto è RightArrow, la camera ruota verso sinistra
        if(Input.GetKey(KeyCode.LeftArrow))
        {
            rotation.y -= rotationSpeed;
            transform.eulerAngles = rotation;
        }

        //Se il pulsante premuto è LeftArrow, la camera ruota verso destra
        if (Input.GetKey(KeyCode.RightArrow))
        {
            rotation.y += rotationSpeed;
            transform.eulerAngles = rotation;
        }

        //Se il pulsante premuto è UpArrow, la camera ruota verso l'alto
        if (Input.GetKey(KeyCode.UpArrow))
        {
            rotation.x -= rotationSpeed;
            transform.eulerAngles = rotation;
        }

        //Se il pulsante premuto è DownArrow, la camera ruota verso il basso
        if (Input.GetKey(KeyCode.DownArrow))
        {
            rotation.x += rotationSpeed;
            transform.eulerAngles = rotation;
        }
    }
}
