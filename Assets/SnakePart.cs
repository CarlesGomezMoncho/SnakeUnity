using System.Collections.Generic;
using UnityEngine;

public class SnakePart : MonoBehaviour
{
    //public bool isHead = false;

    private float width;
    private float height;

    private List<Vector2> nextDirection;//direcciones futuras
    private Vector2 currentDirection;   //dirección actual

    private Vector2 prevPosition;   //posición previa (solo se puede cambiar de dirección en posiciones fijas, entre prev y next no se cambia)
    private Vector2 nextPosition;   //posición siguiente

    public GameObject nextPart;    //siguente parte de la serpiente //DEBUG despres fer privada

    private void Awake()
    {
        ResetDirections();
    }

    void Start()
    {
        width = gameObject.GetComponent<SpriteRenderer>().bounds.size.x;
        height = gameObject.GetComponent<SpriteRenderer>().bounds.size.y;

        currentDirection = Vector2.zero;        //inicialmente no se mueve

        SetPrevPosition(transform.position);    //se asigna la posición real a la actual y la siguente
        SetNextPosition(transform.position);
    }

    public void ResetDirections()
    {
        //Debug.Log(GetNumDirections());
        nextDirection = new List<Vector2>();
        //currentDirection = Vector2.zero;
    }

    //Añadimos una nueva dirección a la lista de direcciones
    public void AddDirection(Vector2 newDirection)
    {
        int numDirecciones = GetNumDirections();    //cuantas direcciones hay guardadas
        Vector2 ultimaGuardada;                     //ultima dirección guardada

        //si hay alguna dirección guardada en la lista, la asignamos en una variable
        if (numDirecciones != 0)
            ultimaGuardada = nextDirection[numDirecciones - 1];
        else
            //sino asignamos la actual dirección
            ultimaGuardada = currentDirection;

        //solo insertamos si la dirección es distinta de la ultima insertada
        if (newDirection != ultimaGuardada )
        {
            //solo podemos movernos en direcciones no opuestas (si vamos arriba, no podemos ir abajo, solo a izquierda o derecha)
            if (newDirection == Vector2.right && ultimaGuardada != Vector2.left)
                nextDirection.Add(newDirection);
            else if (newDirection == Vector2.left && ultimaGuardada != Vector2.right)
                nextDirection.Add(newDirection);
            else if (newDirection == Vector2.up && ultimaGuardada != Vector2.down)
                nextDirection.Add(newDirection);
            else if (newDirection == Vector2.down && ultimaGuardada != Vector2.up)
                nextDirection.Add(newDirection);
        }
    }

    //actualiza la siguiente dirección OJO, al llamar a esta función se quita una dirección de la lista
    public Vector2 NextDirection()
    {
        if (nextDirection.Count > 0)
        {
            currentDirection = nextDirection[0];
            nextDirection.Remove(nextDirection[0]);
        }

        return currentDirection;
    }

    //cuantas direcciones hay en la lista
    public int GetNumDirections()
    {
        if (nextDirection != null)
            return nextDirection.Count;
        else
            return 0;
    }

    //saca recursivamente la ultima de las partes de la serpiente
    public GameObject GetLastPart()
    {
        if (nextPart)
            return nextPart.GetComponent<SnakePart>().GetLastPart();
        else
            return gameObject;
    }

    public GameObject GetNextPart()
    {
        return nextPart;
    }

    public void SetNextPart(GameObject newPrevPart)
    {
        nextPart = newPrevPart;
    }

    public Vector2 GetCurrentDirection()
    {
        return currentDirection;
    }

    public float getWidth()
    {
        return width;
    }

    public float getHeight()
    {
        return height;
    }

    public Vector2 GetNextPosition()
    {
        return nextPosition;
    }

    public void SetNextPosition(Vector2 newNextPosition)
    {
        nextPosition = newNextPosition;
    }

    public Vector2 GetPrevPosition()
    {
        return prevPosition;
    }

    public void SetPrevPosition( Vector2 newPrevPosition)
    {
        prevPosition = newPrevPosition;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //si esta parte es la cabeza
        if (this.transform.name == "cabeza")
        {
            //si no choca contra el anterior
            if (collision.name != "parte1" && collision.name != "comida")
            {
                //si choca contra una parte parada, no cuenta
                if (collision.GetComponent<SnakePart>().GetCurrentDirection() != Vector2.zero)
                {
                    //si no está ya en pausa(seguramente por que ya se ha llamado 1 vez)
                    if (Time.timeScale != 0)
                        GameController.gc.GameOver();
                }
            }
        }
    }

    private void Update()
    {
        //si es la cabeza y estan activadas las colisiones
        if (this.transform.name == "cabeza" && GameController.gc.GetCollisions())
        {
            if (Time.timeScale != 0)
            {
                //si su posición actual dependiendo de la dirección sobrepasa el limite
                if (currentDirection == Vector2.right)
                {
                    //si sobrepasa la ultima linea del grid
                    if (transform.position.x > GameController.gc.gridController.GetRight().GetPosition(0).x)
                    {
                        GameController.gc.GameOver();
                        currentDirection = Vector2.zero;
                    }
                }

                if (currentDirection == Vector2.left)
                {
                    //si sobrepasa la ultima linea del grid
                    if (transform.position.x < GameController.gc.gridController.GetLeft().GetPosition(0).x)
                    {
                        GameController.gc.GameOver();
                        currentDirection = Vector2.zero;
                    }
                }

                if (currentDirection == Vector2.up)
                {
                    //si sobrepasa la ultima linea del grid
                    if (transform.position.y > GameController.gc.gridController.GetTop().GetPosition(0).y)
                    {
                        GameController.gc.GameOver();
                        currentDirection = Vector2.zero;
                    }
                }

                if (currentDirection == Vector2.down)
                {
                    //si sobrepasa la ultima linea del grid
                    if (transform.position.y < GameController.gc.gridController.GetBottom().GetPosition(0).y)
                    {
                        GameController.gc.GameOver();
                        currentDirection = Vector2.zero;
                    }
                }
            }
        }
    }
}
