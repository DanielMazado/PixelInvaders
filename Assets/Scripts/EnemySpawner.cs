using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemySpawner : MonoBehaviour
{
    // Instancia Singleton
    public static EnemySpawner Instance;

    [SerializeField] private int[] limits;
    [SerializeField] private float[] delays;
    [SerializeField] private int[] maxEnemiesAtOnceList;
    [SerializeField] private int[] typesToSpawnList;
    public GameObject enemyPrefab;
    public GameObject meteorPrefab;
    public GameObject satelitePrefab;
    private int limit;
    private float delay;
    private int maxEnemiesAtOnce;
    private const float boundary = 2.0f;
    private const float obstacleBoundary = 5f;
    private int currentAmount = 0;
    private int amountSpawned = 0;
    private int typesToSpawn;
    private static Coroutine co, co2;
    private UserInterface ui;

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
        else if (SceneManager.GetActiveScene().name == "Level5")
        {
            SetupLevel(5);
        }
        else if (SceneManager.GetActiveScene().name == "Boss")
        {
            if (Instance != null)
            {
                Destroy(Instance.gameObject); // Destruir la instancia de EnemySpawner
            }
        }
        SceneManager.sceneLoaded += OnSceneLoaded;
        ui = GameObject.Find("UserInterface").GetComponent<UserInterface>();
        if (ui != null)
        {
            ui.UpdateEnemiesLeft(GetRemainingEnemies() + 1);  // Actualizar el contador de enemigos
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Esto se ejecuta después de que la escena se haya cargado
        ui = GameObject.Find("UserInterface").GetComponent<UserInterface>();
        if (ui != null)
        {
            ui.UpdateEnemiesLeft(GetRemainingEnemies() + 1);  // Actualizar el contador de enemigos
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
        if (side == 1)
        {
            meteorTransform.gameObject.GetComponent<SpriteRenderer>().flipX = true;
        }

        if (meteorTransform != null && SceneManager.GetActiveScene().isLoaded)
        {
            if (meteorTransform == null) yield break;

            float moveSpeedX = 3f;
            float moveSpeedY = 1.5f;
            Vector3 targetPosition;

            // Determinar la posición final en el eje X
            targetPosition = (side == 0) ? new Vector3(-obstacleBoundary, meteorTransform.position.y, meteorTransform.position.z) :
                                            new Vector3(obstacleBoundary, meteorTransform.position.y, meteorTransform.position.z);

            // Mover el meteorito tanto en X como en Y simultáneamente
            while (meteorTransform != null && (meteorTransform.position.x != targetPosition.x || meteorTransform.position.y > -3f))
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

    // Método para resetear corrutinas.

    public void ResetCoroutines()
    {
        if(co != null) 
        {
            StopCoroutine(co);
            co = null;
        }

        if(co2 != null) 
        {
            StopCoroutine(co2);
            co2 = null;
        }
    }

    // Métodos para preparar cada nivel nuevo y cambiar entre ellos.
    public void SetupLevel(int id)
    {
        currentAmount = 0;
        amountSpawned = 0;

        ResetCoroutines();
        if (id != 6)
        {
            limit = limits[id - 1];
            delay = delays[id - 1];
            maxEnemiesAtOnce = maxEnemiesAtOnceList[id - 1];
            typesToSpawn = typesToSpawnList[id - 1];

            if (co == null)
            {
                co = StartCoroutine(spawnEnemies());
            }

            if (co2 == null)
            {
                co2 = StartCoroutine(spawnObstacles());
            }
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
                AudioManager.Instance.StopBackgroundMusic();
                AudioManager.Instance.PlayBackgroundMusic("Gameplay_2");
                break;
            case "Level3":
                SetupLevel(4);
                SceneManager.LoadScene("Level4");
                break;
            case "Level4":
                SetupLevel(5);
                SceneManager.LoadScene("Level5");
                AudioManager.Instance.StopBackgroundMusic();
                AudioManager.Instance.PlayBackgroundMusic("Gameplay_3");
                break;
            case "Level5":
                SetupLevel(6);
                SceneManager.LoadScene("Boss");
                AudioManager.Instance.StopBackgroundMusic();
                AudioManager.Instance.PlayBackgroundMusic("Boss");
                if (Instance != null)
                {
                    Destroy(Instance.gameObject); // Destruir la instancia de EnemySpawner
                }
                break;
        }
        ui = GameObject.Find("UserInterface").GetComponent<UserInterface>();
        if(ui != null)
        {
            ui.UpdateEnemiesLeft(GetRemainingEnemies()+1);
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