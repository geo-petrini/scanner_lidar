using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    // Rotazione della camera
    private Vector3 rotation;

    // Velocità di rotazione della camera
    public float rotationSpeed;

    // Velocità di movimento della camera
    private float movementSpeed;

    void Start()
    {
        // Salvo la posizione iniziale della camera
        rotation = transform.eulerAngles;

        //Setto i valori delle velocità
        rotationSpeed = 4.0f;
        movementSpeed = 40.0f;
    }

    void Update()
    {
        // Se il pulsante premuto è LeftShift, il movimento della camera sarà dimezzato
        if(Input.GetKeyDown(KeyCode.LeftShift))
        {
            movementSpeed /= 2;
        }

        // Se il pulsante premuto è LeftShift, il movimento della camera tornerà normale
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            movementSpeed *= 2;
        }

        // Se il pulsante premuto è W, la camera si muove avanti
        if (Input.GetKey(KeyCode.W))
        {
            transform.position += transform.forward * movementSpeed * Time.deltaTime;
        }

        // Se il pulsante premuto è S, la camera si muove indietro
        if (Input.GetKey(KeyCode.S))
        {
            transform.position -= transform.forward * movementSpeed * Time.deltaTime;
        }

        // Se il pulsante premuto è A, la camera si muove a sinistra
        if (Input.GetKey(KeyCode.A))
        {
            transform.position -= transform.right * movementSpeed * Time.deltaTime;
        }

        // Se il pulsante premuto è D, la camera si muove a destra
        if (Input.GetKey(KeyCode.D))
        {
            transform.position += transform.right * movementSpeed * Time.deltaTime;
        }

        // Se il pulsante premuto è Space, la camera salirà in alto
        if (Input.GetKey(KeyCode.Space))
        {
            transform.position += new Vector3(0, transform.up.y * movementSpeed * Time.deltaTime, 0);
        }

        // Se il pulsante premuto è LeftControl, la camera scenderà in basso
        if (Input.GetKey(KeyCode.LeftControl))
        {
            transform.position -= new Vector3(0, transform.up.y * movementSpeed * Time.deltaTime, 0);
        }

        //Se il pulsante premuto è RightArrow, la camera ruota verso destra
        if (Input.GetKey(KeyCode.RightArrow))
        {
            rotation.y += rotationSpeed;
            transform.eulerAngles = rotation;
        }

        //Se il pulsante premuto è LeftArrow, la camera ruota verso sinistra
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            rotation.y -= rotationSpeed;
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