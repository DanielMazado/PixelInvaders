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
            SceneManager.LoadScene("Level1");
        }
    }

    // Método para recibir daño.

    private void OnTriggerEnter2D(Collider2D collision) 
    {
        if(collision.gameObject.CompareTag("EnemyBullet") && !damageProcessed) 
        {
            damageProcessed = true;
            health--;

            ui.UpdateLife(health);

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
        else if(collision.gameObject.CompareTag("Enemy") && !damageProcessed) 
        {
            damageProcessed = true;
            
            health--;

            ui.UpdateLife(health);

            Destroy(collision.gameObject);

            StartCoroutine(ResetDamageProcessedFlag());
        }
    }
    private IEnumerator ResetDamageProcessedFlag()
    {
        yield return new WaitForSeconds(0.1f);
        damageProcessed = false;
    }

}