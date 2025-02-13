using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int health = 3;
    private UserInterface ui;

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
        if(collision.gameObject.CompareTag("EnemyBullet")) 
        {
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
        }
        else if(collision.gameObject.CompareTag("Enemy")) 
        {
            health--;

            ui.UpdateLife(health);

            Destroy(collision.gameObject);
        }
    }
}
