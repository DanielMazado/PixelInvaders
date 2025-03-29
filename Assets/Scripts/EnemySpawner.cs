using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
        yield return new WaitForSeconds(3f);
        
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

            int typeToSet = UnityEngine.Random.Range(0, 5);
            eb.SetType(typeToSet);
            amountSpawned++;
        }

        Debug.Log(this.GetRemainingEnemies());
    }

    // Método para saber cuántos enemigos quedan.
    public int GetRemainingEnemies() 
    {
        return (limit - amountSpawned + currentAmount);
    }
}
