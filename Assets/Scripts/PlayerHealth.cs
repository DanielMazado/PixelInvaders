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

    [Header("Invencibilidad")]
    public float invincibilityDuration = 2.0f; // Duración de los frames de invencibilidad.
    private bool isInvincible = false;        // Flag para frames de invencibilidad.
    private SpriteRenderer spriteRenderer;     // Referencia al SpriteRenderer para efectos visuales.

    // Start is called before the first frame update
    void Start()
    {
        ui = GameObject.Find("UserInterface").GetComponent<UserInterface>();
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
    }

    void Update() 
    {
        if(health <= 0) 
        { 
            Destroy(this.gameObject);
            AudioManager.Instance.StopBackgroundMusic();
            ui.AddScore(-1000);
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
        else
        {
            if(Input.GetKeyDown(KeyCode.Space) && ui.GetScore() >= 500 && health < 3)
            {
                health++;
                ui.AddScore(-500);
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
        if(isInvincible) return;

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

            if(EnemySpawner.Instance != null)
            {
                ui.UpdateEnemiesLeft(EnemySpawner.Instance.GetRemainingEnemies());
                Destroy(collision.gameObject);
            }

            StartCoroutine(ResetDamageProcessedFlag());
        }
    }

    // Método auxiliar de recibir daño.
    private void TakeDamage() 
    {
        health--;
        if(health > 0)
        {
            StartCoroutine(InvincibilityFrames());
        }
        AudioManager.Instance.PlaySound("PlayerHurt");
        ui.UpdateLife(health);
    }

    private IEnumerator ResetDamageProcessedFlag()
    {
        yield return new WaitForSeconds(0.1f);
        damageProcessed = false;
    }

    // Frames de invencibilidad.
    private IEnumerator InvincibilityFrames()
    {
        isInvincible = true; // Activar invencibilidad.

        // Hacer parpadear al jugador como indicación visual.
        float elapsedTime = 0f;
        while (elapsedTime < invincibilityDuration)
        {
            spriteRenderer.enabled = !spriteRenderer.enabled; // Alternar visibilidad.
            elapsedTime += 0.2f;
            yield return new WaitForSeconds(0.2f);
        }

        spriteRenderer.enabled = true; // Asegurar que el sprite esté visible.
        isInvincible = false;         // Desactivar invencibilidad.
    }
}