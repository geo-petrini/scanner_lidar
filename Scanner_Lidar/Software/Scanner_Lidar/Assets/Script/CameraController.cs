using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    //Script di aiuto per il movimento con il mouse: https://answers.unity.com/questions/1344322/free-mouse-rotating-camera.html

    // Rotazione della camera
    private Vector3 rotation;

    // Velocità di rotazione della camera
    public float rotationSpeed;

    // Velocità di movimento della camera
    private float movementSpeed;

    // Indica se usare il mouse look
    private bool useMouseLook;

    // Sensibilità del mouse
    public float sensitivity = 6f;

    // Angolazione massima di 80° in verticale
    public float maxYAngle = 80f;

    // Rotazione corrente della camera (usato solo con il mouse)
    private Vector2 currentRotation;

    void Start()
    {
        // Setto i valori delle velocità
        rotationSpeed = 4.0f;
        movementSpeed = 40.0f;

        // Inizialmente non uso il mouse look
        useMouseLook = false;
    }

    void Update()
    {
        // Se il pulsante premuto è Tab, si swithcerà tra mouse look o no
        if (Input.GetKeyUp(KeyCode.Tab))
        {
            if(Cursor.lockState == CursorLockMode.Locked)
            {
                Cursor.lockState = CursorLockMode.None;
                useMouseLook = false;
            }else
            {
                Cursor.lockState = CursorLockMode.Locked;
                useMouseLook = true;
            }
        }

        // Se il pulsante premuto è LeftShift, il movimento della camera sarà dimezzato
        if (Input.GetKeyDown(KeyCode.LeftShift))
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

        // Se sono in "mouse look mode", ruoto la camera con il mouse, altrimenti con le freccette
        if (useMouseLook)
        {
            currentRotation.x += Input.GetAxis("Mouse X") * sensitivity * (Time.deltaTime + 0.5f);
            currentRotation.y -= Input.GetAxis("Mouse Y") * sensitivity * (Time.deltaTime + 0.5f);
            currentRotation.x = Mathf.Repeat(currentRotation.x, 360);
            currentRotation.y = Mathf.Clamp(currentRotation.y, -maxYAngle, maxYAngle);
            transform.rotation = Quaternion.Euler(currentRotation.y, currentRotation.x, 0);
        }
        else
        {
            // Salvo la posizione iniziale della camera
            rotation = transform.eulerAngles;

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
}