using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShoot : MonoBehaviour
{
    private bool canShoot = true; // A modo de delay.
    private static bool autoShoot = false; // Por mayor comodidad.
    private bool spacePressed = false; // Para ajustar autoShoot.

    [SerializeField] private float timeBetweenBullets = 0.1f;
    [SerializeField] private float rechargeTime = 0.5f;
    public GameObject bulletPrefab;

    [SerializeField] public float bulletSpeed = 20f;
    private const float bulletHeightLimit = 6f;
    [SerializeField] public GameObject[] bullets;
    
    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.C))
        {
            autoShoot = !autoShoot;
        }

        spacePressed = Input.GetKey(KeyCode.Space);

        if((spacePressed || autoShoot) && canShoot)
        {
            if(spacePressed) { autoShoot = false; }

            Shoot(3);
        }
    }

    // Método para disparar.

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
            AudioManager.Instance.PlaySound("Shooting");
            StartCoroutine(BulletMovement(bullets[i]));
            yield return new WaitForSeconds(timeBetweenBullets);
        }
        StartCoroutine(WaitTillShootAgain());
    }

    // Movimiento de la bala.
    private IEnumerator BulletMovement(GameObject bullet) 
    {
        while(bullet != null && bullet.transform.position.y < bulletHeightLimit) 
        {
            if (bullet == null) yield break;

            Vector3 newPosition = new Vector3(bullet.transform.position.x, bullet.transform.position.y + bulletSpeed * Time.deltaTime, bullet.transform.position.z);
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
}
