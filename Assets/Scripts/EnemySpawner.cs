using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemySpawner : MonoBehaviour
{
    // Instancia Singleton
    public static EnemySpawner Instance;

    public GameObject enemyPrefab;
    public GameObject meteorPrefab;
    public GameObject satelitePrefab;
    private int limit;
    private float delay;
    private int maxEnemiesAtOnce;
    private const float boundary = 5.0f;
    private const float obstacleBoundary = 10.5f;
    private int currentAmount = 0;
    private int amountSpawned = 0;
    private int typesToSpawn;
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

        // Asegúrate de que no haya instancias duplicadas
        if (Instance == null)
        {
            Instance = this; // Asegúrate de que la instancia esté correctamente asignada.
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

            int typeToSet = UnityEngine.Random.Range(0, typesToSpawn);
            eb.SetType(typeToSet);
            amountSpawned++;
        }
    }

    // Método para saber cuántos enemigos quedan.
    public int GetRemainingEnemies()
    {
        return (limit - amountSpawned + currentAmount);
    }

    // Corrutina para spawnear obstáculos.

    private IEnumerator spawnObstacles() 
    {
        yield return new WaitForSeconds(5f);

        while (amountSpawned < limit)
        {
            spawnObstacle();
            yield return new WaitForSeconds(UnityEngine.Random.Range(1.5f * delay, 3 * delay));
        }
    }

    private void spawnObstacle() 
    {
        int obstacleToSpawn = UnityEngine.Random.Range(0, 2);
        Vector3 position = transform.position;

        int side = UnityEngine.Random.Range(0, 2);

        // Mover el obstáculo a la posición correcta dependiendo del lado.
        if (side == 0)
        {
            position.x = obstacleBoundary;
        }
        else // side == 1
        {
            position.x = -obstacleBoundary;
        }

        // Spawnear obstáculos.

        if (obstacleToSpawn == 0) 
        {
            // Meteorito.
            GameObject obj = Instantiate(meteorPrefab, position, Quaternion.identity);

            // Movimiento del meteorito dependiendo del lado.
            StartCoroutine(MoveMeteor(obj.transform, side));
        } 
        else if (obstacleToSpawn == 1) 
        {
            // Modificar la posición en el eje y aleatoriamente.
            position.y = UnityEngine.Random.Range(-4, 2.51f);

            // Satélite.
            GameObject obj = Instantiate(satelitePrefab, position, Quaternion.identity);

            // Movimiento del satélite dependiendo del lado.
            StartCoroutine(MoveSatellite(obj.transform, side));
        }
    }

    private IEnumerator MoveMeteor(Transform meteorTransform, int side)
    {
        if (meteorTransform != null && SceneManager.GetActiveScene().isLoaded)
        {
            if (meteorTransform == null) yield break;

            float moveSpeedX = 10f;
            float moveSpeedY = 5f;
            Vector3 targetPosition;

            // Determinar la posición final en el eje X
            targetPosition = (side == 0) ? new Vector3(-obstacleBoundary, meteorTransform.position.y, meteorTransform.position.z) :
                                            new Vector3(obstacleBoundary, meteorTransform.position.y, meteorTransform.position.z);

            // Mover el meteorito tanto en X como en Y simultáneamente
            while (meteorTransform != null && (meteorTransform.position.x != targetPosition.x || meteorTransform.position.y > -10f))
            {
                if (meteorTransform == null) yield break;  // Asegurarse de que no se acceda al transform si ya ha sido destruido

                meteorTransform.position = new Vector3(
                    Mathf.MoveTowards(meteorTransform.position.x, targetPosition.x, moveSpeedX * Time.deltaTime),
                    Mathf.MoveTowards(meteorTransform.position.y, -10f, moveSpeedY * Time.deltaTime),
                    meteorTransform.position.z
                );

                yield return null;
            }

            // Destruir el meteorito después de que haya alcanzado la posición final
            if (meteorTransform != null)
            {
                Destroy(meteorTransform.gameObject);
            }
        }
    }

    private IEnumerator MoveSatellite(Transform satelliteTransform, int side)
    {
        if (satelliteTransform == null)
        {
            yield break;  // Si el satélite ya ha sido destruido, salir de la corrutina inmediatamente.
        }

        float moveSpeed = 2f;
        Vector3 targetPosition;

        if (side == 0) // Movimiento hacia la izquierda
        {
            targetPosition = new Vector3(-obstacleBoundary, satelliteTransform.position.y, satelliteTransform.position.z);
            while (satelliteTransform != null && satelliteTransform.position.x > targetPosition.x)
            {
                if (satelliteTransform == null)
                {
                    yield break;  // Si el satélite ha sido destruido, salimos de la corrutina
                }

                satelliteTransform.position = Vector3.MoveTowards(satelliteTransform.position, targetPosition, moveSpeed * Time.deltaTime);
                yield return null;
            }
        }
        else // side == 1, movimiento hacia la derecha
        {
            targetPosition = new Vector3(obstacleBoundary, satelliteTransform.position.y, satelliteTransform.position.z);
            while (satelliteTransform != null && satelliteTransform.position.x < targetPosition.x)
            {
                if (satelliteTransform == null)
                {
                    yield break;  // Si el satélite ha sido destruido, salimos de la corrutina
                }

                satelliteTransform.position = Vector3.MoveTowards(satelliteTransform.position, targetPosition, moveSpeed * Time.deltaTime);
                yield return null;
            }
        }

        // Destruir el satélite una vez que haya alcanzado su destino.
        if (satelliteTransform != null)
        {
            Destroy(satelliteTransform.gameObject);
        }
    }

    // Métodos para preparar cada nivel nuevo y cambiar entre ellos.
    public void SetupLevel(int id)
    {
        currentAmount = 0;
        amountSpawned = 0;

        switch (id)
        {
            case 1:
                limit = 10;
                delay = 5.0f;
                maxEnemiesAtOnce = 2;
                typesToSpawn = 2;
                break;
            case 2:
                limit = 15;
                delay = 4.0f;
                maxEnemiesAtOnce = 2;
                typesToSpawn = 3;
                break;
            case 3:
                limit = 18;
                delay = 3.5f;
                maxEnemiesAtOnce = 3;
                typesToSpawn = 4;
                break;
            case 4:
                limit = 21;
                delay = 3.0f;
                maxEnemiesAtOnce = 3;
                typesToSpawn = 5;
                break;
            case 5:
                limit = 25;
                delay = 3.0f;
                maxEnemiesAtOnce = 4;
                typesToSpawn = 5;
                break;
        }

        if(co == null)
        {
            StartCoroutine(spawnEnemies());
            StartCoroutine(spawnObstacles());
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
                SetupLevel(4);
                SceneManager.LoadScene("Level4");
                break;
            case "Level4":
                SetupLevel(5);
                SceneManager.LoadScene("Level5");
                break;
            case "Level5":
                // Destruir el singleton cuando se termine el nivel 3 y se regrese al menú
                if (Instance != null)
                {
                    Destroy(Instance.gameObject); // Destruir la instancia de EnemySpawner
                }
                AudioManager.Instance.StopBackgroundMusic();
                AudioManager.Instance.PlayBackgroundMusic("Menu");
                SceneManager.LoadScene("Menu");
                break;
        }
    }

    // Método para obtener número basado en el nivel activo.

    public int GetLevelID() 
    {
        int ID;

        switch(SceneManager.GetActiveScene().name) 
        {
            case "Level1":
                ID = 1;
            break;
            case "Level2":
                ID = 2;
            break;
            case "Level3":
                ID = 3;
            break;
            case "Level4":
                ID = 4;
            break;
            case "Level5":
                ID = 5;
            break;
            default:
                ID = 0;
            break;
        }

        return ID;
    }
}