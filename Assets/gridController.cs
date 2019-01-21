using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gridController : MonoBehaviour
{
    public GameObject presetLinea;

    public int filas;
    public int columnas;

    public float separacion = 0.2f;

    private LineRenderer top, bottom, left, right;

    void Start()
    {
        CrearLineas();
    }

    public void CrearLineas()
    {
        //borramos posibles lineas existentes
        foreach (Transform child in transform)
        {
            GameObject.Destroy(child.gameObject);
        }

        //creem una linea
        GameObject objLinea;
        LineRenderer linea = null;

        //lineas verticales
        for (float i = 0; i < columnas * separacion ; i += separacion)
        {
            objLinea = Instantiate(presetLinea, transform);
            linea = objLinea.GetComponent<LineRenderer>();
            linea.SetPosition(0, new Vector3(i, 0, 0));
            linea.SetPosition(1, new Vector3(i, filas * separacion, 0));

            //si es la primera linea vertical
            if (i == 0)
            {
                left = linea;
            }
        }

        //ultima linea, si se dibuixa alguna
        if (linea != null)
        {
            right = linea;
        }

        //lineas horizontales
        for (float i = 0; i < filas * separacion; i += separacion)
        {
            objLinea = Instantiate(presetLinea, transform);
            linea = objLinea.GetComponent<LineRenderer>();
            linea.SetPosition(0, new Vector3(0, i, 0));
            linea.SetPosition(1, new Vector3(columnas * separacion, i, 0));

            //si es la primera linea
            if (i == 0)
            {
                bottom = linea;
            }
        }

        //ultima linea horitzontal, si se dibuixa alguna
        if (linea != null)
        {
            top = linea;
        }
    }

    public LineRenderer GetTop()
    {
        return top;
    }

    public LineRenderer GetBottom()
    {
        return bottom;
    }

    public LineRenderer GetLeft()
    {
        return left;
    }

    public LineRenderer GetRight()
    {
        return right;
    }
}
