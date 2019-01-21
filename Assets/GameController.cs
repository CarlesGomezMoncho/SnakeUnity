using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{

    public Text text;   //Cuenta el nº de partes de la serpiente

    public static GameController gc;

    public GameObject snakePart;            //Prefab de cada parte del cuerpo
    public GameObject comida;               //prefab del item que hace crecer
    public gridController gridController;   //controlador del grid, se usa básicamente para debug, por ver las lineas de cambio de posición
    public GameObject snake;                //contenedor de la serpiente entera
    public int numPartesIniciales = 10;     //cuantas partes tendrà la serpiente inicialmente
    public GameObject menuController;

    public Animator gameOverAnimator;

    public float speed = 1;     //velocidad de la serpiente

    private float anchoMundo, altoMundo;    //ancho y alto de la pantalla en coordenadas en el juego

    private bool crecer;

    private GameObject cabeza;          //la primera parte de la serpiente, que es la que guia al resto
    private GameObject ultimaParte;     //la ultima parte de la serpiente

    private GameObject comidaInstanciada;   //instancia de la comida actual

    private int numParts = 1;           //inicialmente solo està la cabeza

    private bool gameOverState = true;

    private bool collisionsOn = true;

    private void Awake()
    {
        if (gc != null)
        {
            GameObject.Destroy(gc);
        }
        else
        {
            gc = this;
        }
    }

    void Start()
    {

        ConfigureGrid();
        menuController.GetComponent<MenuController>().ShowMenu();
    }

    void Update()
    {

        if (GetGameOverState() == false && Time.timeScale != 0)
        {
            //cojemos la cabeza primero y su controlador
            GameObject obj = cabeza;
            SnakePart part = obj.GetComponent<SnakePart>();

            //control de movimiento (solo cabeza)
            ControlMovement(part);

            //actualizamos la posicion siguiente de cada parte de la serpiente
            while (obj != null)
            {
                //calcula la siguiente posición
                CalculaNextPosition(part);

                //si la dirección actual es distinta de 0 (si hay movimiento) o si hay cambios de dirección pendientes
                if (part.GetCurrentDirection() != Vector2.zero || part.GetNumDirections() > 0)
                {
                    //Si la posición actual es >= que la siguiente (si ya se ha llegado a la siguiente posición)
                    if (DistanciaMaxima(obj.transform.position, part.GetNextPosition(), part.GetCurrentDirection()))
                    {

                        //actualizamos la actual a la siguiente (la nueva siguiente se actualiza en el update)
                        part.SetPrevPosition(part.GetNextPosition());

                        //por si nos pasamos actualizamos la posición del objeto
                        obj.transform.position = part.GetPrevPosition();

                        //si tiene que crecer, añadimos una nueva parte
                        if (crecer && obj == ultimaParte)
                        {
                            crecer = false;
                            AddPart();
                        }

                        //si hay direcciones pendientes, cambiamos de dirección
                        if (part.GetNumDirections() > 0)
                        {
                            Vector2 newDirection = part.NextDirection();

                            obj.transform.position = Vector3.Lerp(obj.transform.position, new Vector3(obj.transform.position.x + newDirection.x, obj.transform.position.y + newDirection.y), Time.deltaTime * speed);
                        }
                        //si no hay direccion pendiente mantenemos la actual
                        else
                        {
                            obj.transform.position = Vector3.Lerp(obj.transform.position, new Vector3(obj.transform.position.x + part.GetCurrentDirection().x, obj.transform.position.y + part.GetCurrentDirection().y), Time.deltaTime * speed);
                        }

                        //comprovamos si hay siguiente parte e iniciamos movimiento si no se mueve
                        GameObject nextGameObject = part.GetNextPart();
                        if (nextGameObject)
                        {
                            SnakePart partNextObject = nextGameObject.GetComponent<SnakePart>();

                            if (partNextObject.GetCurrentDirection() == Vector2.zero)
                            {
                                partNextObject.NextDirection();
                            }
                        }

                    }
                    //si aun no se ha llegado a la siguiente posición, continua con el actual movimiento
                    else
                    {
                        obj.transform.position = Vector3.Lerp(obj.transform.position, new Vector3(obj.transform.position.x + part.GetCurrentDirection().x, obj.transform.position.y + part.GetCurrentDirection().y), Time.deltaTime * speed);
                    }
                }

                //cambiamos a la parte siguiente si existe
                obj = part.GetNextPart();
                if (obj)
                {
                    part = obj.GetComponent<SnakePart>();
                }
            }

            text.text = numParts.ToString();
        }
    }

    private void ConfigureGrid()
    {
        //ancho del sprite de la serpiente (debe ser cuadrado)
        float anchoSprite = snakePart.GetComponent<SpriteRenderer>().bounds.size.x;

        //asignamos el ancho de celda al tamaño del sprite de la serpiente
        gridController.separacion = anchoSprite;

        //calculamos el ancho de la camara en coordenadas de mundo
        //screenToWorldPoint devuelve las coordenadas de la resolución de la pantalla, multiplicamos por 2 por que estamos en 0, 0
        anchoMundo = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, 0, 0)).x * 2;
        altoMundo = Camera.main.ScreenToWorldPoint(new Vector3(0, Screen.height, 0)).y * 2;

        //movemos el grid para que su primera casilla este en 0, 0 y así poder contar des de 0, 0 situaldo abajo a la izquierda
        gridController.transform.position = new Vector2(anchoMundo / 2, altoMundo / 2);

        //movemos la camara al centro del grid
        Camera.main.transform.position = new Vector3(gridController.transform.position.x, gridController.transform.position.y, -10);

        //asignamos el nº de filas dependiendo del ancho y alto de la pantalla. Dividimos el ancho y alto por el tamaño del sprite
        gridController.filas = (int)(altoMundo / anchoSprite) + 1;
        gridController.columnas = (int)(anchoMundo / anchoSprite);// + 1;

        //creamos el grid
        gridController.CrearLineas();
    }

    private void CalculaNextPosition(SnakePart part)
    {
        Vector2 currentDirection = part.GetCurrentDirection();  //dirección actual
        Vector2 currentPosition = part.GetPrevPosition();       //posición desde la que calculamos la siguiente
        float desplazacimentoTotal = part.getWidth();           //cuanto desplazamiento entre prev y next ha de existir (es el ancho del sprite de la serpiente)

        Vector2 nextPosition = new Vector2(currentPosition.x + (desplazacimentoTotal * currentDirection.x), currentPosition.y + (desplazacimentoTotal * currentDirection.y));

        //asignamos la anterior dirección como siguiente dirección de la siguiente parte si tiene siguiente parte
        if (part.GetNextPart())
            part.GetNextPart().GetComponent<SnakePart>().AddDirection(part.GetCurrentDirection());

        //asignamos la nueva siguiente posición calculada
        part.SetNextPosition(nextPosition);
    }

    private void ControlMovement(SnakePart part)
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        //si hay algun tipo de movimiento añadimos la dirección a la pila de direcciones
        if (moveHorizontal != 0 || moveVertical != 0)
        {
            //si pulsamos derecha
            if (moveHorizontal > 0)
                part.AddDirection(Vector2.right);
            //si pulsamos izquierda
            else if (moveHorizontal < 0)
                part.AddDirection(Vector2.left);
            //si pusamos arriba
            else if (moveVertical > 0)
                part.AddDirection(Vector2.up);
            //si pulsamos abajo
            else if (moveVertical < 0)
                part.AddDirection(Vector2.down);

            //si aun no nos movemos, cogemos directamente la siguiente dirección
            if (part.GetCurrentDirection() == Vector2.zero)
            {
                part.NextDirection();
            }
        }
    }

    //calcula si se ha llegado o pasado a la posición siguiente
    public bool DistanciaMaxima(Vector2 actual, Vector2 next, Vector2 direccion)
    {
        if (direccion == Vector2.up)
        {
            if (next.y - actual.y <= 0)
                return true;
        }
        else if (direccion == Vector2.down)
        {
            if (actual.y - next.y <= 0)
                return true;
        }
        else if (direccion == Vector2.right)
        {
            if (next.x - actual.x <= 0)
                return true;
        }
        else if (direccion == Vector2.left)
        {
            if (actual.x - next.x <= 0)
                return true;
        }

        return false;
    }

    public void Crece()
    {
        crecer = true;
    }

    public void AddPart()
    {
        GameObject g = Instantiate(snakePart, ultimaParte.transform.position, ultimaParte.transform.rotation);

        SnakePart ultimaParteP = ultimaParte.GetComponent<SnakePart>();
        SnakePart part = g.GetComponent<SnakePart>();

        g.name = "parte" + numParts;
        ultimaParteP.SetNextPart(g);
        ultimaParte = g;
        g.transform.parent = snake.transform;
        numParts++;

        part.SetNextPosition(ultimaParteP.GetNextPosition());
    }

    public void StartGame()
    {
        //ancho del sprite de la serpiente (debe ser cuadrado)
        float anchoSprite = snakePart.GetComponent<SpriteRenderer>().bounds.size.x;

        //si ya existe una cabeza la borramos y todas sus otras partes
        if (cabeza)
        {
            DeleteSnake(cabeza);
        }

        //instanciamos la cabeza
        cabeza = Instantiate(snakePart);
        //la movemos al centro
        cabeza.transform.position = new Vector2(gridController.columnas / 2 * anchoSprite + anchoSprite / 2, gridController.filas / 2 * anchoSprite + anchoSprite / 2);
        //le cambiamos el nombre
        cabeza.name = "cabeza";
        //lo guardamos como ultima parte
        ultimaParte = cabeza;
        //asignamos la cabeza al contenedor snake
        cabeza.gameObject.transform.parent = snake.transform;

        //resetemos numero de segmentos inicial
        numParts = 1;

        //añadimos algunas partes iniciales mas
        while (numParts < numPartesIniciales)
        {
            AddPart();
        }

        //si no existe una comida, la creamos
        if (!comidaInstanciada)
        {
            comidaInstanciada = Instantiate(comida);
            comidaInstanciada.name = "comida";
        }
        else
        {
            //si ya existe la reubicamos
            comidaInstanciada.GetComponent<ComidaController>().Reubicate();
        }

        gameOverState = false;
    }

    private void DeleteSnake(GameObject head)
    {
        SnakePart part = head.GetComponent<SnakePart>();

        //si hay siguiente parte llamamos a esta función pero de esa parte
        if (part.nextPart)
        {
            DeleteSnake(part.nextPart);
        }

        //finalmente borramos el objecto
        Destroy(head);
    }


    public void GameOver()
    {
        if (collisionsOn)
        {
            Time.timeScale = 0;

            SetGameOverState(true);

            gameOverAnimator.SetTrigger("start");
        }
    }

    public bool GetGameOverState()
    {
        return gameOverState;
    }

    public void SetGameOverState(bool newState)
    {
        gameOverState = newState;
    }

    public void RestartGame()
    {
        StartCoroutine(Espera());
    }

    private IEnumerator Espera()
    {
        yield return new WaitForSeconds(0.2f); //si pilla el reinicio a medio movimiento, si no se para un poco empieza moviendose y no desde el centro (ni idea por que) con un pequeño delay se arregla
        StartGame();    
    }

    public void SetCollisions(bool state)
    {
        collisionsOn = state;
    }

    public bool GetCollisions()
    {
        return collisionsOn;
    }
}
