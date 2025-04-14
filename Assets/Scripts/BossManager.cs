using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BossManager : MonoBehaviour
{
    [SerializeField] private int health = 50;
    [SerializeField] private float minDelay = 2.5f;
    [SerializeField] private float maxDelay = 5f;
    private UserInterface ui;
    private void Start()
    {
        ui = GameObject.Find("UserInterface").GetComponent<UserInterface>();
        if(ui != null)
        {
            ui.UpdateEnemiesLeft(-2);
        }

        if(!AudioManager.Instance.isBackgroundMusicPlaying())
        {
            AudioManager.Instance.PlayBackgroundMusic("Boss");
        }

        StartCoroutine(BossCoroutine());
    }

    private IEnumerator BossCoroutine()
    {
        yield return new WaitForSeconds(3f);

        // Jefe activa sprite, colliders y todo eso.

        // Esperar aleatoriamente y atacar.

        // Mientras su vida sea mayor a 0.

        while(health > 0)
        {
            yield return new WaitForSeconds(UnityEngine.Random.Range(minDelay, maxDelay+0.01f));
        }
    } 
    private void Defeat()
    {
        AudioManager.Instance.StopBackgroundMusic();
        AudioManager.Instance.PlayBackgroundMusic("Menu");
        SceneManager.LoadScene("Menu");
    }
}
