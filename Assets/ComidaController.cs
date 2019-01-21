using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComidaController : MonoBehaviour
{

    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
        InitPosition();
    }

    private void InitPosition()
    {
        Reubicate();
        animator.SetTrigger("Start");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //si choca con la cabeza, hacemos crecer la serpiente y reiniciamos su posición
        if (collision.name == "cabeza")
        {
            animator.SetTrigger("Comido");
        }
    }

    public void Reubicate()
    {
        //lo ponemos en un sitio random dentro de donde puede
        float minX, maxX, minY, maxY;

        maxX = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, 0, 0)).x - gameObject.GetComponent<SpriteRenderer>().bounds.size.x / 2;
        maxY = Camera.main.ScreenToWorldPoint(new Vector3(0, Screen.height, 0)).y - gameObject.GetComponent<SpriteRenderer>().bounds.size.y / 2;
        minX = 0f + gameObject.GetComponent<SpriteRenderer>().bounds.size.x / 2;
        minY = 0f + gameObject.GetComponent<SpriteRenderer>().bounds.size.y / 2;

        transform.position = new Vector2(Random.Range(minX, maxX), Random.Range(minY, maxY));
    }

    //se le llama desde la animación
    public void eventoComida()
    {
        GameController.gc.GetComponent<GameController>().Crece();
        InitPosition();
    }
}
