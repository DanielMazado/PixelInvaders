using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int health = 3;
    private UserInterface ui;
    private bool damageProcessed = false;

    // Start is called before the first frame update
    void Start()
    {
        ui = GameObject.Find("UserInterface").GetComponent<UserInterface>();
    }

    void Update() 
    {
        if(health <= 0) 
        { 
            Destroy(this.gameObject);
            AudioManager.Instance.StopBackgroundMusic();
            EnemySpawner.Instance.SetupLevel(EnemySpawner.Instance.GetLevelID());
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            
            if(EnemySpawner.Instance.GetLevelID() < 3) 
            {
                AudioManager.Instance.PlayBackgroundMusic("Gameplay");
            }
            else if(EnemySpawner.Instance.GetLevelID() < 5) 
            {
                AudioManager.Instance.PlayBackgroundMusic("Gameplay_2");
            }
            else
            {
                AudioManager.Instance.PlayBackgroundMusic("Gameplay_3");
            }
        }
    }

    // Método para curar toda la vida.

    public void FullHeal() 
    {
        health = 3;

        if(ui == null) 
        {
            ui = GameObject.Find("UserInterface").GetComponent<UserInterface>();
        }

        ui.UpdateLife(health);
    }

    // Método para recibir daño.

    private void OnTriggerEnter2D(Collider2D collision) 
    {
        if(collision.gameObject.CompareTag("EnemyBullet") && !damageProcessed) 
        {
            damageProcessed = true;
            
            TakeDamage();

            GameObject[] bullets = GameObject.FindGameObjectsWithTag("EnemyBullet");

            foreach (GameObject bullet in bullets)
            {
                if (bullet == collision.gameObject)
                {
                    Destroy(bullet); // Destruir solo la bala que ha colisionado.
                    break; // Salir del ciclo una vez que la bala ha sido destruida.
                }
            }
            StartCoroutine(ResetDamageProcessedFlag());
        }
        else if((collision.gameObject.CompareTag("Enemy") || collision.gameObject.CompareTag("Obstacle")) && !damageProcessed) 
        {
            damageProcessed = true;
            
            TakeDamage();

            Destroy(collision.gameObject);

            StartCoroutine(ResetDamageProcessedFlag());
        }
    }

    // Método auxiliar de recibir daño.
    private void TakeDamage() 
    {
        health--;
        AudioManager.Instance.PlaySound("PlayerHurt");
        ui.UpdateLife(health);
    }

    private IEnumerator ResetDamageProcessedFlag()
    {
        yield return new WaitForSeconds(0.1f);
        damageProcessed = false;
    }

}