using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    [SerializeField] public float speed = 5f;
    [SerializeField] private float acceleration = 2f;

    public GameObject bulletPrefab;

    private const float slowingMultiplier = 4f;
    private const float minSpeed = 5f;
    private const float maxSpeed = 10f;

    [SerializeField] public float bulletSpeed = 50f;
    private const float bulletHeightLimit = 6f;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        float moveInput = Input.GetAxis("Horizontal");
        
        MoveCharacter(moveInput, speed);

        if(Input.GetKeyDown(KeyCode.Space)) { Shoot(); }
    }

    // Métodos para mover al personaje.

    private void MoveCharacter(float moveInput, float speed) 
    {
        Vector2 movement;

        if(Math.Abs(transform.position.x) < 8.01)
        {
            movement = new Vector2(moveInput * speed, rb.velocity.y);
        }
        else 
        {
            movement = new Vector2(0.0f, rb.velocity.y);
            transform.position = new Vector3((float)Math.Round(transform.position.x), transform.position.y, transform.position.z);
        }

        switch(moveInput) 
        {
            case 0:
                speed = (speed > minSpeed) ? speed - acceleration * slowingMultiplier * Time.deltaTime : minSpeed;
            break;
            
            default:
                speed = (speed < maxSpeed) ? speed + acceleration * Time.deltaTime : maxSpeed;
            break;
        }

        SetSpeed(speed);
        rb.AddForce(movement - rb.velocity, ForceMode2D.Impulse);
    }

    // Método para establecer la velocidad.
    private void SetSpeed(float speedToSet) { this.speed = speedToSet; }

    // Métodos para disparar.

    private void Shoot(int bulletsToShoot = 1) 
    {
        GameObject[] bullets = new GameObject[bulletsToShoot];
        for(int i = 0; i < bulletsToShoot; i++) 
        {
            bullets[i] = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
            StartCoroutine(BulletMovement(bullets[i]));
        }
    }

    private IEnumerator BulletMovement(GameObject bullet) 
    {
        while(bullet.transform.position.y < bulletHeightLimit) 
        {
            Vector3 newPosition = new Vector3(bullet.transform.position.x, bullet.transform.position.y + bulletSpeed * Time.deltaTime, bullet.transform.position.z);
            bullet.transform.position = newPosition;
            yield return null;
        }

        Destroy(bullet);
    }

}
