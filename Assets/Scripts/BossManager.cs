using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BossManager : MonoBehaviour
{
    [SerializeField] private int health = 50;
    [SerializeField] private float minDelay = 2.5f;
    [SerializeField] private float maxDelay = 5f;
    [SerializeField] private Sprite bossSprite;
    [SerializeField] private RuntimeAnimatorController animControl;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private UserInterface ui;
    private GameObject player;
    private bool spawned = false;
    private bool attacking = false;
    private void Start()
    {
        spriteRenderer = this.gameObject.GetComponent<SpriteRenderer>();
        animator = this.gameObject.GetComponent<Animator>();

        ui = GameObject.Find("UserInterface").GetComponent<UserInterface>();
        if(ui != null)
        {
            ui.UpdateEnemiesLeft(-2);
        }

        if (player == null)
        {
            player = GameObject.Find("Player");
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
        spriteRenderer.sprite = bossSprite;
        animator.runtimeAnimatorController = animControl;
        spawned = true;

        // Mientras su vida sea mayor a 0.
        while(health > 0)
        {
            // Esperar aleatoriamente y atacar.
            yield return new WaitForSeconds(UnityEngine.Random.Range(minDelay, maxDelay+0.01f));

            Attack(UnityEngine.Random.Range(0, 1));

            attacking = true;

            while(attacking)
            {
                yield return null;
            }
        }
    }

    private void Attack(int ataque = 0)
    {
        // Se pueden añadir: 
            // - Ataques de otros enemigos pero mejorados.
            // - Ataques originales.
            // - Mezcla de los dos anteriores.

        switch(ataque)
        {
            case 0:
                StartCoroutine(TacklerAttack());
            break;

            case 1:
            break;

            case 2:
            break;

            case 3:
            break;
        }
    }

    // Métodos para los distintos ataques.

    // Ataque 0: Placaje.

    private IEnumerator TacklerAttack()
    {
        float followDuration = 2.5f; // Duración en la que seguirá al jugador
        float stopDuration = 0.6f; // Tiempo de espera después de seguir al jugador
        float descentDuration = 0.5f; // Tiempo para descender rápidamente
        float ascentDuration = 2f; // Tiempo para ascender lentamente
    
        if (player == null)
        {
            player = GameObject.Find("Player");
        }

        Vector2 initialPosition = transform.position; // Posición inicial
        Vector2 targetPosition = player.transform.position; // Suponiendo que 'player' es el GameObject al que sigue

        // Seguimiento solo en el eje X
        float followTime = 0f;
        while (followTime < followDuration)
        {
            // Solo mover en el eje X
            Vector2 newPosition = new Vector2(
                Mathf.MoveTowards(transform.position.x, player.transform.position.x, 7f * Time.deltaTime), // Mover solo en X
                transform.position.y // Mantener la posición Y constante
            );

            transform.position = newPosition;
            followTime += Time.deltaTime;
            yield return null;
        }

        // Espera
        yield return new WaitForSeconds(stopDuration);

        // Descenso rápido
        Vector2 descentTarget = new Vector2(transform.position.x, transform.position.y - 9f); // Aumentado el descenso en Y (ahora baja 10 unidades)
        float descentTime = 0f;
        while (descentTime < descentDuration)
        {
            transform.position = Vector2.MoveTowards(transform.position, descentTarget, 15f * Time.deltaTime); // Aumentada la velocidad de descenso
            descentTime += Time.deltaTime;
            yield return null;
        }

        // Ascenso lento
        Vector2 ascentTarget = new Vector2(transform.position.x, initialPosition.y); // Subir de vuelta a la altura inicial
        float ascentTime = 0f;
        while (ascentTime < ascentDuration)
        {
            transform.position = Vector2.MoveTowards(transform.position, ascentTarget, 4f * Time.deltaTime); // Aumentada la velocidad de ascenso
            ascentTime += Time.deltaTime;
            yield return null;
        }

        attacking = false;
    }

    // Ataque 1: ---

    // Detección de balas.
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(!spawned) return;
        
        if(collision.gameObject.CompareTag("PlayerBullet"))
        {
            if(this.health <= 0)
            {
                Defeat();
            }
            else
            {
                health--;
                Destroy(collision.gameObject);
            }
        }
    }

    private void Defeat()
    {
        AudioManager.Instance.StopBackgroundMusic();
        AudioManager.Instance.PlayBackgroundMusic("Menu");
        SceneManager.LoadScene("Menu");
    }
}
