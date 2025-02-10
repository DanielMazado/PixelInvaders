using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyBehaviour : MonoBehaviour
{
    [SerializeField] private int health = 3;
    [SerializeField] public float speed = 5.0f;
    private bool movingRight;
    private float direction;
    private PlayerShoot ps; // Referencia al script PlayerShoot.
    private Rigidbody2D rb; // Referencia al Rigidbody2D.

    // Start is called before the first frame update
    void Start()
    {
        ps = GameObject.Find("Player").GetComponent<PlayerShoot>();
        rb = GetComponent<Rigidbody2D>();

        movingRight = (UnityEngine.Random.Range(0, 2) == 0) ? true : false;
        direction = (movingRight) ? 1f : -1f;

    }

    // Update is called once per frame
    void Update()
    {
        if(health == 0 && this.gameObject != null) 
        {
            Destroy(this.gameObject);
            return;
        }

        Vector2 movement;

        if(Math.Abs(transform.position.x) < 7.01)
        {
            movement = new Vector2(direction * speed, rb.velocity.y);
        }
        else 
        {
            movement = new Vector2(0.0f, rb.velocity.y);
            transform.position = new Vector3((float)Math.Round(transform.position.x), transform.position.y, transform.position.z);
            direction *= -1f;
        }

        rb.AddForce(movement - rb.velocity, ForceMode2D.Impulse);
    }

    // Método para ser destruido

    private void OnTriggerEnter2D(Collider2D collision) 
    {
        if(collision.gameObject.CompareTag("Bullet")) 
        {
            ps.StopCoroutine("BulletMovement");
            GameObject[] bullets = GameObject.FindGameObjectsWithTag("Bullet");

            foreach (GameObject bullet in bullets)
            {
                if (bullet == collision.gameObject)
                {
                    Destroy(bullet); // Destruir solo la bala que ha colisionado.
                    break; // Salir del ciclo una vez que la bala ha sido destruida.
                }
            }

            health--;
        }
    }
}
