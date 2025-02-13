using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    [SerializeField] private int limit = 10;
    [SerializeField] private float delay = 5.0f;
    [SerializeField] private int maxEnemiesAtOnce = 2;
    private const float boundary = 5.0f;
    private int currentAmount = 0;
    private int amountSpawned = 0;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(spawnEnemies());
    }

    // Update is called once per frame
    void Update()
    {
        currentAmount = GameObject.FindGameObjectsWithTag("Enemy").Length;
    }

    // Corrutina para invocar enemigos hasta el límite.

    private IEnumerator spawnEnemies() 
    {
        while(amountSpawned < limit) 
        {
            spawnEnemy();
            yield return new WaitForSeconds(delay);
        }
    }

    private void spawnEnemy() 
    {
        if(currentAmount < maxEnemiesAtOnce) 
        {
            Vector3 position = transform.position;
            position.x = UnityEngine.Random.Range(-boundary, boundary+0.01f);

            GameObject obj = Instantiate(enemyPrefab, position, Quaternion.identity);
            EnemyBehaviour eb = obj.GetComponent<EnemyBehaviour>();

            switch(UnityEngine.Random.Range(0, 3)) 
            {
                case 0:
                    eb.SetType(0);
                break;

                case 1:
                    eb.SetType(1);
                break;

                case 2:
                    eb.SetType(2);
                break;
            }

            amountSpawned++;
        }
    }
}
