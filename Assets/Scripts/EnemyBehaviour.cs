using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class EnemyBehaviour : MonoBehaviour
{
    private int health;
    [SerializeField] public float speed = 5.0f;
    [SerializeField] public float speedMultiplier = 1.0f;
    private const float boundary = 7.01f;
    private static float originalYPos;
    private bool movingRight;
    private bool canShoot = true;
    private float direction;
    private PlayerShoot ps; // Referencia al script PlayerShoot.
    private Rigidbody2D rb; // Referencia al Rigidbody2D.

    private GameObject[] bullets;
    public GameObject bulletPrefab;

    private GameObject player;
    private const float bulletHeightLimit = -5.2f;

    private enum EnemyType {Basic, Fast, Shooting, StillShooter, Tackler};
    [SerializeField] private EnemyType thisEnemyType;

    [SerializeField] private float timeBetweenBullets = 0.2f;
    [SerializeField] private float rechargeTime = 1f;

    [SerializeField] public float bulletSpeed = 3f;
    [SerializeField] private float verticalOffset = 1f;

    private float followDuration = 3f; // Duración en la que seguirá al jugador
    private float stopDuration = 1f; // Tiempo de espera después de seguir al jugador
    private float descentDuration = 0.5f; // Tiempo para descender rápidamente
    private float ascentDuration = 2f; // Tiempo para ascender lentamente

    private Coroutine shootingCoroutine;
    private Coroutine tacklerCoroutine;

    private UserInterface ui; // Referencia a la UI.
    private EnemySpawner es; // Referencia al EnemySpawner.

    [SerializeField] private Sprite[] enemySprites;
    [SerializeField] private Sprite[] bulletSprites;
    [SerializeField] private RuntimeAnimatorController[] enemyAnimators;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");
        ps = GameObject.Find("Player").GetComponent<PlayerShoot>();
        rb = GetComponent<Rigidbody2D>();
        ui = GameObject.Find("UserInterface").GetComponent<UserInterface>();
        es = GameObject.Find("Spawner").GetComponent<EnemySpawner>();

        originalYPos = transform.position.y;

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

                case EnemyType.Fast:
                    ui.AddScore(150);
                break;

                case EnemyType.Shooting:
                    ui.AddScore(200);
                break;

                case EnemyType.StillShooter:
                    ui.AddScore(150);
                break;

                case EnemyType.Tackler:
                    ui.AddScore(150);
                break;
            }
            ui.UpdateEnemiesLeft(es.GetRemainingEnemies());
            return;
        }

        if(thisEnemyType != EnemyType.StillShooter && thisEnemyType != EnemyType.Tackler) 
        {
            MoveEnemy(speedMultiplier);
        }
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
            transform.position = new Vector3((float)Math.Round(transform.position.x), transform.position.y - verticalOffset, transform.position.z);
            direction *= -1f;

            if(thisEnemyType == EnemyType.Shooting) 
            {
                List<Transform> hijos = new List<Transform>();

                foreach (Transform hijo in transform)
                {
                    hijos.Add(hijo);  // Agregamos cada hijo a la lista
                }

                foreach (Transform hijo in hijos)
                {
                    hijo.transform.position = new Vector3(hijo.transform.position.x, hijo.transform.position.y + verticalOffset, hijo.transform.position.z);
                }
            }

            if(transform.position.y <= bulletHeightLimit) 
            {
                transform.position = new Vector3((float)Math.Round(transform.position.x), originalYPos, transform.position.z);
            }
        }

        rb.AddForce(movement - rb.velocity, ForceMode2D.Impulse);
    }

    // Corrutina para movimiento Tackler.

    private IEnumerator TacklerMovement()
    {
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

        // Esperar 1 segundo antes de repetir
        yield return new WaitForSeconds(1f);

        // Repetir el proceso
        StartCoroutine(TacklerMovement());
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
        Vector2 dirShoot;

        if(thisEnemyType == EnemyType.StillShooter) 
        {
            dirShoot = (player.transform.position - this.transform.position).normalized;
        }
        else
        {
            dirShoot = new Vector2(0, -1);
        }

        StartCoroutine(ShootBullets(bulletsToShoot, dirShoot));
    }

    // Instanciar las balas.

    private IEnumerator ShootBullets(int bulletsToShoot, Vector2 dirShoot) 
    {
        for(int i = 0; i < bullets.Length; i++) 
        {
            bullets[i] = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
            if (thisEnemyType == EnemyType.StillShooter)
            {
                bullets[i].GetComponent<SpriteRenderer>().sprite = bulletSprites[1];
            } 
            else 
            { 
                bullets[i].GetComponent<SpriteRenderer>().sprite = bulletSprites[0]; 
            }
            bullets[i].transform.SetParent(transform);
            bullets[i].transform.localPosition = Vector3.zero;

            StartCoroutine(BulletMovement(bullets[i], dirShoot));
            yield return new WaitForSeconds(timeBetweenBullets);
        }
        
        yield return new WaitForSeconds(rechargeTime);

        canShoot = true;
    }

    // Movimiento de la bala.
    private IEnumerator BulletMovement(GameObject bullet, Vector2 dirShoot) 
    {
        while(bullet != null && bullet.transform.position.y > bulletHeightLimit) 
        {
            if (bullet == null) yield break;

            bullet.transform.position = new Vector3(bullet.transform.position.x + dirShoot.x * bulletSpeed * Time.deltaTime, bullet.transform.position.y + dirShoot.y * bulletSpeed * Time.deltaTime, bullet.transform.position.z);
            
            yield return null;
        }

        if(bullet != null) { Destroy(bullet); }
    }

    // Método para ser destruido

    private void OnTriggerEnter2D(Collider2D collision) 
    {
        if(collision.gameObject.CompareTag("PlayerBullet")) 
        {
            if(this.health <= 0) 
            {
                if(thisEnemyType == EnemyType.Shooting || thisEnemyType == EnemyType.StillShooter) 
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
            } 
            else 
            {
                health--;
            }
        }
    }

    // Método para cambiar el tipo de enemigo.

    public void SetType(int type) 
    {
        if(type == 0) 
        {
            this.thisEnemyType = EnemyType.Basic;
            health = 4;
            this.GetComponent<SpriteRenderer>().sprite = enemySprites[0];
            this.GetComponent<Animator>().runtimeAnimatorController = enemyAnimators[0];
            speedMultiplier = 1f;
            if(shootingCoroutine != null) 
            {
                StopCoroutine(ShootingCorroutine());
            }
        }
        else if (type == 1) 
        {
            this.thisEnemyType = EnemyType.Fast;
            health = 2;
            speedMultiplier = 2f;
            this.GetComponent<SpriteRenderer>().sprite = enemySprites[1];
            this.GetComponent<Animator>().runtimeAnimatorController = enemyAnimators[1];
            if(shootingCoroutine != null) 
            {
                StopCoroutine(ShootingCorroutine());
            }
        }
        else if (type == 2) 
        {
            this.thisEnemyType = EnemyType.Shooting;
            health = 3;
            speedMultiplier = 1f;
            this.GetComponent<SpriteRenderer>().sprite = enemySprites[2];
            this.GetComponent<Animator>().runtimeAnimatorController = enemyAnimators[2];
            if(shootingCoroutine == null) 
            {
                StartCoroutine(ShootingCorroutine());
            }
        }
        else if (type == 3) 
        {
            this.thisEnemyType = EnemyType.StillShooter;
            health = 3;
            speedMultiplier = 0f;
            this.GetComponent<SpriteRenderer>().sprite = enemySprites[3];
            this.GetComponent<Animator>().runtimeAnimatorController = enemyAnimators[3];
            if(shootingCoroutine == null) 
            {
                StartCoroutine(ShootingCorroutine());
            }
        }
        else if (type == 4) 
        {
            this.thisEnemyType = EnemyType.Tackler;
            health = 4;
            speedMultiplier = 1f;
            this.GetComponent<SpriteRenderer>().sprite = enemySprites[4];
            this.GetComponent<Animator>().runtimeAnimatorController = enemyAnimators[4];
            if(tacklerCoroutine == null) 
            {
                StartCoroutine(TacklerMovement());
            }
        }
    }
}
