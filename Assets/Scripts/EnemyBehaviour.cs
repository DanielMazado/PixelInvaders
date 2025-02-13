using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyBehaviour : MonoBehaviour
{
    [SerializeField] private int health = 3;
    [SerializeField] public float speed = 5.0f;
    [SerializeField] public float speedMultiplier = 1.0f;
    private const float boundary = 7.01f;
    private bool movingRight;
    private bool canShoot = true;
    private float direction;
    private PlayerShoot ps; // Referencia al script PlayerShoot.
    private Rigidbody2D rb; // Referencia al Rigidbody2D.

    private GameObject[] bullets;
    public GameObject bulletPrefab;
    private float bulletHeightLimit = -6f;

    private enum EnemyType {Basic, Fast, Shooting};
    [SerializeField] private EnemyType thisEnemyType;

    [SerializeField] private float timeBetweenBullets = 0.2f;
    [SerializeField] private float rechargeTime = 1f;

    [SerializeField] public float bulletSpeed = 10f;

    private Coroutine shootingCoroutine;

    private UserInterface ui; // Referencia a la UI.

    // Start is called before the first frame update
    void Start()
    {
        ps = GameObject.Find("Player").GetComponent<PlayerShoot>();
        rb = GetComponent<Rigidbody2D>();
        ui = GameObject.Find("UserInterface").GetComponent<UserInterface>();

        movingRight = (UnityEngine.Random.Range(0, 2) == 0) ? true : false;
        direction = (movingRight) ? 1f : -1f;

        speedMultiplier = (thisEnemyType == EnemyType.Fast) ? 2f : 1f;

        if(thisEnemyType == EnemyType.Shooting) 
        { 
            shootingCoroutine = StartCoroutine(ShootingCorroutine()); 
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(health == 0 && this.gameObject != null) 
        {
            Destroy(this.gameObject);
            switch(thisEnemyType) 
            {
                case EnemyType.Basic:
                    ui.AddScore(100);
                break;

                default:
                    ui.AddScore(150);
                break;
            }
            return;
        }

        MoveEnemy(speedMultiplier);
    }

    // Método para moverse.

    private void MoveEnemy(float speedMultiplier = 1f) 
    {
        Vector2 movement;

        if(Math.Abs(transform.position.x) < boundary)
        {
            movement = new Vector2(direction * speed * speedMultiplier, rb.velocity.y);
        }
        else 
        {
            movement = new Vector2(0.0f, rb.velocity.y);
            transform.position = new Vector3((float)Math.Round(transform.position.x), transform.position.y, transform.position.z);
            direction *= -1f;
        }

        rb.AddForce(movement - rb.velocity, ForceMode2D.Impulse);
    }

    // Corrutina para disparar periódicamente.

    private IEnumerator ShootingCorroutine() 
    {
        yield return new WaitForSeconds(1f);

        while(this.gameObject != null) 
        {
            if(canShoot) { Shoot(); }

            yield return new WaitForSeconds(1f);
        }
    }

    // Método para disparar hacia abajo.

    private void Shoot(int bulletsToShoot = 1) 
    {
        canShoot = false;
        bullets = new GameObject[bulletsToShoot];
        StartCoroutine(ShootBullets(bulletsToShoot));
    }

    // Instanciar las balas.

    private IEnumerator ShootBullets(int bulletsToShoot = 1) 
    {
        for(int i = 0; i < bullets.Length; i++) 
        {
            bullets[i] = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
            bullets[i].transform.SetParent(transform);
            bullets[i].transform.localPosition = Vector3.zero;

            StartCoroutine(BulletMovement(bullets[i]));
            yield return new WaitForSeconds(timeBetweenBullets);
        }
        StartCoroutine(WaitTillShootAgain());
    }

    // Movimiento de la bala.
    private IEnumerator BulletMovement(GameObject bullet) 
    {
        while(bullet != null && bullet.transform.position.y > bulletHeightLimit) 
        {
            if (bullet == null) yield break;

            Vector3 newPosition = new Vector3(bullet.transform.position.x, bullet.transform.position.y - bulletSpeed * Time.deltaTime, bullet.transform.position.z);
            bullet.transform.position = newPosition;
            
            yield return null;
        }

        if(bullet != null) { Destroy(bullet); }
    }

    // Delay antes de disparar de nuevo.
    private IEnumerator WaitTillShootAgain() 
    {
        yield return new WaitForSeconds(rechargeTime);

        canShoot = true;
    }

    // Método para ser destruido

    private void OnTriggerEnter2D(Collider2D collision) 
    {
        if(collision.gameObject.CompareTag("PlayerBullet")) 
        {
            if(thisEnemyType == EnemyType.Shooting) 
            {
                for (int i = 0; i < transform.childCount; i++)
                {
                    Destroy(transform.GetChild(i).gameObject);
                }
            }

            ps.StopCoroutine("BulletMovement");
            GameObject[] bullets = GameObject.FindGameObjectsWithTag("PlayerBullet");

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

    // Método para cambiar el tipo de enemigo.

    public void SetType(int type) 
    {
        if(type == 0) 
        {
            this.thisEnemyType = EnemyType.Basic;
            speedMultiplier = 1f;
            if(shootingCoroutine != null) 
            {
                StopCoroutine(ShootingCorroutine());
            }
        }
        else if (type == 1) 
        {
            this.thisEnemyType = EnemyType.Fast;
            speedMultiplier = 2f;
            if(shootingCoroutine != null) 
            {
                StopCoroutine(ShootingCorroutine());
            }
        }
        else if (type == 2) 
        {
            this.thisEnemyType = EnemyType.Shooting;
            speedMultiplier = 1f;
            if(shootingCoroutine == null) 
            {
                StartCoroutine(ShootingCorroutine());
            }
        }
    }
}
