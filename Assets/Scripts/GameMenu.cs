using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMenu : MonoBehaviour
{
    private Coroutine co;

    public void StartGame() 
    {
        if(co == null)
        {
            AudioManager.Instance.PlaySound("Button");
            SceneManager.LoadScene("Level1");
            AudioManager.Instance.PlayBackgroundMusic("Gameplay");
        }
    }

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
