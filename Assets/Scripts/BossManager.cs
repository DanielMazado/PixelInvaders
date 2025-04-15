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
    private void Start()
    {
        spriteRenderer = this.gameObject.GetComponent<SpriteRenderer>();
        animator = this.gameObject.GetComponent<Animator>();

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
        spriteRenderer.sprite = bossSprite;
        animator.runtimeAnimatorController = animControl;

        // Mientras su vida sea mayor a 0.
        while(health > 0)
        {
            // Esperar aleatoriamente y atacar.
            yield return new WaitForSeconds(UnityEngine.Random.Range(minDelay, maxDelay+0.01f));

            // Se pueden añadir: 
            // - Ataques de otros enemigos pero mejorados.
            // - Ataques originales.
            // - Mezcla de los dos anteriores.
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("PlayerBullet"))
        {
            if(this.health <= 0)
            {
                Defeat();
            }
            else
            {
                health--;
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
