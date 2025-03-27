using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMenu : MonoBehaviour
{
    public void StartGame() 
    {
        SceneManager.LoadScene("Level1");
        AudioManager.Instance.PlayBackgroundMusic("Gameplay");
    }

    public void ExitGame() 
    {
        Application.Quit();

        // En el editor de Unity, detiene la ejecución en el editor.
        #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}
