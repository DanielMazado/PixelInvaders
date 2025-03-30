using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemySpawner : MonoBehaviour
{
    // Instancia Singleton
    public static EnemySpawner Instance;

    public GameObject enemyPrefab;
    private int limit;
    private float delay;
    private int maxEnemiesAtOnce;
    private const float boundary = 5.0f;
    private int currentAmount = 0;
    private int amountSpawned = 0;
    private Coroutine co;

    void Awake()
    {
        // Implementar el patrón Singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Si ya existe una instancia, destruir la nueva
        }
        else
        {
            Instance = this; // Asignar la instancia única
            DontDestroyOnLoad(gameObject); // No destruir al cargar una nueva escena
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        if (SceneManager.GetActiveScene().name == "Level1")
        {
            SetupLevel(1);
        }
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

        while (amountSpawned < limit)
        {
            spawnEnemy();
            yield return new WaitForSeconds(delay);
        }

        while (currentAmount > 0)
        {
            yield return null;
        }

        SwitchLevel();
    }

    private void spawnEnemy()
    {
        if (currentAmount < maxEnemiesAtOnce)
        {
            Vector3 position = transform.position;
            position.x = UnityEngine.Random.Range(-boundary, boundary + 0.01f);

            GameObject obj = Instantiate(enemyPrefab, position, Quaternion.identity);
            EnemyBehaviour eb = obj.GetComponent<EnemyBehaviour>();

            int typeToSet = UnityEngine.Random.Range(0, 5);
            eb.SetType(typeToSet);
            amountSpawned++;
        }
    }

    // Método para saber cuántos enemigos quedan.
    public int GetRemainingEnemies()
    {
        return (limit - amountSpawned + currentAmount);
    }

    // Métodos para preparar cada nivel nuevo y cambiar entre ellos.
    private void SetupLevel(int id)
    {
        currentAmount = 0;
        amountSpawned = 0;

        switch (id)
        {
            case 1:
                limit = 10;
                delay = 5.0f;
                maxEnemiesAtOnce = 2;
                break;
            case 2:
                limit = 15;
                delay = 4.0f;
                maxEnemiesAtOnce = 2;
                break;
            case 3:
                limit = 18;
                delay = 3.5f;
                maxEnemiesAtOnce = 3;
                break;
            case 4:
                // Configura el nivel 4 aquí si es necesario
                break;
            case 5:
                // Configura el nivel 5 aquí si es necesario
                break;
        }

        if(co == null)
        {
            StartCoroutine(spawnEnemies());
        }
    }

    private void SwitchLevel()
    {
        switch (SceneManager.GetActiveScene().name)
        {
            case "Level1":
                SetupLevel(2);
                SceneManager.LoadScene("Level2");
                break;

            case "Level2":
                SetupLevel(3);
                SceneManager.LoadScene("Level3");
                break;

            case "Level3":
                // Destruir el singleton cuando se termine el nivel 3 y se regrese al menú
                if (Instance != null)
                {
                    Destroy(Instance.gameObject); // Destruir la instancia de EnemySpawner
                }
                SceneManager.LoadScene("Menu");
                break;
        }
    }
}