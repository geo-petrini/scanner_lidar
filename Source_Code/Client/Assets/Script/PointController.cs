using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointController : MonoBehaviour
{
    /// <summary>
    /// @Author Veljko Markovic
    /// @Version 29.04.2021
    /// </summary>

    // Lista che contiene tutti i punti che vengono renderizzati
    private List<GameObject> points;

    void Start()
    {
        // Istanzio la lista
        points = new List<GameObject>();
    }

    // Prendo la stringa ricevuta dal Server e ne ricavo i valori che mi servono. Successivamente creo il punto
    public void Render(string point)
    {
        string[] temp = point.Split(',');
        float[] data = { float.Parse(temp[0]), float.Parse(temp[2]), float.Parse(temp[1]) };
        CreatePoint(data);
    }

    // Crea un punto
    private void CreatePoint(float[] data)
    {
        GameObject point = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        point.GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        point.GetComponent<MeshRenderer>().receiveShadows = false;
        Vector3 temp = new Vector3(data[0], data[1], data[2]);
        point.transform.position = temp;
        points.Add(point);
    }

    // Ritorna la grandezza della lista di punti (che rappresenta l'ultimo punto ricevuto)
    public int GetIndexOfLastPoint()
    {
        return points.Count;
    }
}
