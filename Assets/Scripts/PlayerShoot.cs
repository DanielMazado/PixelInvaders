using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShoot : MonoBehaviour
{
    private bool canShoot = true; // A modo de delay.
    [SerializeField] private float rechargeTime = 0.5f;
    public GameObject bulletPrefab;

    [SerializeField] public float bulletSpeed = 50f;
    private const float bulletHeightLimit = 6f;
    [SerializeField] public GameObject[] bullets;
    
    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space) && canShoot) { Shoot(); }
    }

    // Método para disparar.

    private void Shoot(int bulletsToShoot = 1) 
    {
        canShoot = false;
        bullets = new GameObject[bulletsToShoot];
        for(int i = 0; i < bulletsToShoot; i++) 
        {
            bullets[i] = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
            StartCoroutine(BulletMovement(bullets[i]));
        }
        StartCoroutine(WaitTillShootAgain());
    }

    // Movimiento de la bala.
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

    // Delay antes de disparar de nuevo.
    private IEnumerator WaitTillShootAgain() 
    {
        yield return new WaitForSeconds(rechargeTime);

        canShoot = true;
    }
}
