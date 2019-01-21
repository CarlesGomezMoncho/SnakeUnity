using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    public Toggle showGridT;
    public Toggle collsionsT;

    public GameObject grid;

    private bool showingMenu = false;

    private Animator animator;

    private int showGrid = 0;
    private int collisions = 1;

    void Start()
    {
        animator = GetComponent<Animator>();

        showGrid = PlayerPrefs.GetInt("ShowGrid");
        collisions = PlayerPrefs.GetInt("Collisions");

        if (showGrid == 1)
        {
            showGridT.isOn = true;
            grid.SetActive(true);
        }
        else
        {
            showGridT.isOn = false;
            grid.SetActive(false);
        }

        if (collisions == 1)
        {
            collsionsT.isOn = true;
            GameController.gc.SetCollisions(true);
        }
        else
        {
            collsionsT.isOn = false;
            GameController.gc.SetCollisions(false);
        }

    }

    void Update()
    {
        if (Input.GetButtonDown("Cancel") && GameController.gc.GetGameOverState() == false)
        {
            if (animator)
                animator.SetBool("showingMenu", showingMenu);

            if (showingMenu)
            {
                HideMenu();
            }
            else
            {
                ShowMenu();
            }
            
        }
    }

    public void UnPauseGame()
    {
        Time.timeScale = 1;
    }

    public void HideMenu()
    {
        animator.SetTrigger("hideMenu");
        showingMenu = false;
        //Time.timeScale = 1;   //se hace al final de la animación
    }

    public void ShowMenu()
    {
        if (!animator)
        {
            animator = GetComponent<Animator>();
        }

        //si no se ha empezado el juego, no se muestra el botón resume y se cambia el texto de restart por start
        if (GameController.gc.GetGameOverState() == true)
        {
            transform.GetChild(0).GetChild(1).gameObject.SetActive(false);  //segundo objeto dentro del panel(primer child)
            transform.GetChild(0).GetChild(2).GetChild(0).GetComponent<TextMeshProUGUI>().text = "Start";   //cambiamos el texto del label que esta en el objeto 3 dentro del panel dentro del panel de menu
        }
        else
        {
            transform.GetChild(0).GetChild(1).gameObject.SetActive(true);  //segundo objeto dentro del panel(primer child)
            transform.GetChild(0).GetChild(2).GetChild(0).GetComponent<TextMeshProUGUI>().text = "Restart";   //cambiamos el texto del label que esta en el objeto 3 dentro del panel dentro del panel de menu
        }

        animator.SetTrigger("showMenu");
        showingMenu = true;
        Time.timeScale = 0;
    }

    public void quitApp()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit ();
#endif
    }

    public void SwitchShowGrid()
    {
        if (showGridT.isOn)
            showGrid = 1;
        else
            showGrid = 0;

        PlayerPrefs.SetInt("ShowGrid", showGrid);
        PlayerPrefs.Save();
    }

    public void SwitchCollisions()
    {
        if (collsionsT.isOn)
        {
            collisions = 1;
            GameController.gc.SetCollisions(true);
        }
        else
        {
            collisions = 0;
            GameController.gc.SetCollisions(false);
        }
        PlayerPrefs.SetInt("Collisions", collisions);
        PlayerPrefs.Save();
    }
}
