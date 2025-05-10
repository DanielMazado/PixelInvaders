using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GamePause : MonoBehaviour
{
    // public Button resume;  // Referencia al botón para resumir la partida.
    public GameObject effect; // Referencia al "PauseEffect".

    public GameObject pauseText; // Referencia al texto "Pausa" que se encuentra en formato imagen.
    public Button exit; // Referencia del botón para salir del juego.
    public static bool isPaused = false;  // Indica si el juego está pausado o no.

    public Coroutine co; // Para salir del juego sin conflictos.

    // Método para obtener el estado actual.
    public bool getPauseState() 
    {
        return isPaused;
    }

    // Pausar o reanudar cuando se pulse ESCAPE.

    void Update() 
    {
        if(Input.GetKeyDown(KeyCode.Escape) && co == null) 
        {
            AudioManager.Instance.PlaySound("Pause");

            switch(isPaused) 
            {
                case true:
                    ResumeGame();
                break;

                case false:
                    PauseGame();
                break;
            }
        }
    }

    // Método para mostrar o ocultar el menú de pausa.
    private void ShowPauseMenu(bool canShow) 
    {
        effect.gameObject.SetActive(canShow);
        pauseText.gameObject.SetActive(canShow);
        exit.gameObject.SetActive(canShow);
    }
    
    // Método para pausar el juego.
    public void PauseGame() 
    {
        // Para evitar problemas, que no esté pausado el juego para que funcione.

        if(!isPaused) 
        {
            isPaused = true;
            AudioManager.Instance.PauseBackgroundMusic();
            ShowPauseMenu(true);
            Time.timeScale = 0;
        }
    }

    // Método para reanudar la partida.
    public void ResumeGame() 
    {
        // Para evitar problemas, que esté pausado el juego para que se reanude la partida.

        if(isPaused) 
        {
            isPaused = false;
            AudioManager.Instance.ResumeBackgroundMusic();
            ShowPauseMenu(false);
            Time.timeScale = 1;
        }
    }

    // Método para regresar al menú principal.

    public void BackToMenu() 
    {
        if(AudioManager.Instance != null) 
        {
            AudioManager.Instance.StopBackgroundMusic();
            AudioManager.Instance.PlayBackgroundMusic("Menu");
        }
        
        isPaused = false;
        SceneManager.LoadScene("Menu");
    }

    // Método para salir del juego en medio de una partida.
    public void ExitGame()
    {
        AudioManager.Instance.PlaySound("Button");

        co = StartCoroutine(WaitAndLeave());
    }

    private IEnumerator WaitAndLeave()
    {
        yield return new WaitForSeconds(1f);
        Application.Quit();

        // En el editor de Unity, detiene la ejecución en el editor.
        #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}
