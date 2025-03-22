using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Person_Pathfinding : MonoBehaviour
{
    [SerializeField] public float speed = 20f;
    [SerializeField] public float despawnTime = 30f;
    [SerializeField] public float despawnRadius = 20f; // Distance at which pedestrian despawns

    [SerializeField] public GameObject defaultColor;
    [SerializeField] public GameObject WColor;
    [SerializeField] public GameObject AColor;
    [SerializeField] public GameObject SColor;
    [SerializeField] public GameObject DColor;

    [SerializeField] public GameObject player;
    private Player_Movement playerMovement;

    [SerializeField] public GameObject game;
    [SerializeField] public AudioSource sound;

    private Game gameScript;
    private Screen_Tint screenTint;
    
    [Header("Maze Configuration")]
    [SerializeField] public Maze_Generator mazeGenerator;  // Reference to the maze
    [SerializeField] public int mazeWidth = 5;  // X-axis size
    [SerializeField] public int mazeHeight = 5; // Y-axis size

    [Header("Grid Settings")]
    [SerializeField] public float cellSize = 50f;  // Distance between grid points in Unity world units
    [SerializeField] public float topLeftX = -100f;  // Distance between grid points in Unity world units
    [SerializeField] public float topLeftZ = 100f;  // Distance between grid points in Unity world units


    [Header("Goal Points")]
    [SerializeField] public Transform[] goalPoints;  // Assign in Unity Inspector

    private Queue<Vector3> pathQueue = new Queue<Vector3>();
    private bool isMoving = false;

    private Vector2Int startCell;
    private Vector2Int goalCell;
    private Dictionary<int, List<int>> adjacencyList;

    private Vector3 lastPosition;
    private float stuckTime = 0f;

    public string levelName;  // Will hold the level name (e.g., "DayMode_Level1")
    private string collisionKeyPrefix; // Used to differentiate between different levels

    private bool despawning;

    public int type; // 0 is default, 1 goes to W, 2 goes to A, 3 goes to S, 4 goes to D.
    public float probabilityOfDefault = 0.5f;
    private const float pickupRadius = 20.0f;

    void Start()
    {
        despawning = false;
        player = GameObject.Find("Truck_Thing");
        playerMovement = player.GetComponent<Player_Movement>();
        game = GameObject.Find("Game");
        gameScript = game.GetComponent<Game>();
        screenTint = game.GetComponent<Screen_Tint>();
        collisionKeyPrefix = "Collision_" + levelName + "_";
        // Ensure maze reference is assigned
        if (!mazeGenerator)
        {
            mazeGenerator = FindObjectOfType<Maze_Generator>();
        }

        if (mazeGenerator == null)
        {
            Debug.LogError("Maze_Generator not found! Assign it in the inspector.");
            return;
        }

        // mazeWidth = mazeGenerator.size;  // Assuming square for now, could be mazeGenerator.width and height
        // mazeHeight = mazeGenerator.size;

        adjacencyList = BuildAdjacencyList(mazeGenerator);

        startCell = WorldToGrid(transform.position);
        //Debug.Log(startCell);

        // Choose a random goal
        int goalNumber = Random.Range(0, goalPoints.Length);
        bool isDefault = Random.Range(0f, 1f) < probabilityOfDefault;
        if (isDefault) {
            type = 0;
        } else {
            type = goalNumber+ 1;
        }

        StartCoroutine(waitAndDespawn());
        defaultColor.SetActive(true);
        WColor.SetActive(false);
        AColor.SetActive(false);
        SColor.SetActive(false);
        DColor.SetActive(false);
        if (type == 1) {
            WColor.SetActive(true);
            defaultColor.SetActive(false);
        } else if (type == 2) {
            AColor.SetActive(true);
            defaultColor.SetActive(false);
        } else if (type == 3) {
            SColor.SetActive(true);
            defaultColor.SetActive(false);
        } else if (type == 4) {
            DColor.SetActive(true);
            defaultColor.SetActive(false);
        }
        if (goalPoints.Length > 0)
        {
            goalCell = WorldToGrid(goalPoints[goalNumber].position);
        }
        else
        {
            Debug.LogError("No goal points assigned!");
            return;
        }

        // Find path using BFS
        List<Vector3> path = FindPathBFS(startCell, goalCell);
        
        if (path.Count > 0)
        {
            foreach (var pos in path)
            {
                pathQueue.Enqueue(pos);
                //Debug.Log(pos);
            }
            isMoving = true;
        }
    }

    private IEnumerator waitAndDespawn() {
        yield return new WaitForSeconds(despawnTime);
        Destroy(gameObject);
        yield return null;
    }

    void Update()
    {
        if (isMoving && pathQueue.Count > 0)
        {
            Vector3 nextPos = pathQueue.Peek();
            transform.position = Vector3.MoveTowards(transform.position, nextPos, speed * Time.deltaTime);

            if (Vector3.Distance(transform.position, nextPos) < 15f)
            {
                pathQueue.Dequeue();
            }
        }
        if (Vector3.Distance(transform.position, GridToWorld(goalCell)) <= despawnRadius)
        {
            Destroy(gameObject); // Despawn pedestrian
        }
        if (Vector3.Distance(transform.position, lastPosition) < 0.1f)
        {
            stuckTime += Time.deltaTime;
            if (stuckTime > 1f)
            {
                Debug.Log("Pedestrian is stuck, forcing movement!");
                Vector3 goABit = new Vector3(5, 5, 5);
                transform.Translate(goABit * Time.deltaTime * Random.Range(-2, 2));
                stuckTime = 0;
            }
        }
        else
        {
            stuckTime = 0;
        }

        if (type != 0) {
            if (gameScript.peopleCarried < gameScript.maxCarryCapacity) {
                if (Vector3.Distance(transform.position, player.transform.position) < pickupRadius) {
                    if (type == 1) {
                        gameScript.peopleCarriedW++;
                    }
                    if (type == 2) {
                        gameScript.peopleCarriedA++;
                    }
                    if (type == 3) {
                        gameScript.peopleCarriedS++;
                    }
                    if (type == 4) {
                        gameScript.peopleCarriedD++;
                    }
                    Destroy(gameObject);
                }
            }
        }

        lastPosition = transform.position;
    }

    private Dictionary<int, List<int>> BuildAdjacencyList(Maze_Generator maze)
    {
        Dictionary<int, List<int>> adjList = new Dictionary<int, List<int>>();

        if (maze.mstEdges == null)
        {
            Debug.LogError("Maze MST edges not generated yet!");
            return adjList;
        }

        foreach (var edge in maze.mstEdges)
        {
            if (!adjList.ContainsKey(edge.from))
                adjList[edge.from] = new List<int>();

            if (!adjList.ContainsKey(edge.to))
                adjList[edge.to] = new List<int>();

            adjList[edge.from].Add(edge.to);
            adjList[edge.to].Add(edge.from);
        }

        return adjList;
    }

    private List<Vector3> FindPathBFS(Vector2Int start, Vector2Int goal)
    {
        Queue<Vector2Int> queue = new Queue<Vector2Int>();
        Dictionary<Vector2Int, Vector2Int?> cameFrom = new Dictionary<Vector2Int, Vector2Int?>();
        queue.Enqueue(start);
        cameFrom[start] = null;

        while (queue.Count > 0)
        {
            Vector2Int current = queue.Dequeue();

            if (current == goal)
                break;
            //Debug.Log(current);
            int currentIndex = GridToIndex(current);
            if (!adjacencyList.ContainsKey(currentIndex))
            {
                Debug.LogWarning($"Skipping out-of-bounds index {currentIndex} for cell {current}");
                continue;
            }

            foreach (int neighborIndex in adjacencyList[currentIndex])
            {
                Vector2Int neighbor = IndexToGrid(neighborIndex);

                if (!cameFrom.ContainsKey(neighbor) && IsValidCell(neighbor))
                {
                    queue.Enqueue(neighbor);
                    cameFrom[neighbor] = current;
                }
            }
        }

        List<Vector3> path = new List<Vector3>();
        Vector2Int? step = goal;

        while (step != null && cameFrom.ContainsKey(step.Value))
        {
            path.Add(GridToWorld(step.Value));
            step = cameFrom[step.Value];
        }

        path.Reverse();
        return path;
    }

    private int GridToIndex(Vector2Int cell)
    {
        if (!IsValidCell(cell))
        {
            Debug.LogError($"Invalid cell coordinates: {cell}");
            return -1; // Return invalid index
        }
        return cell.y * mazeWidth + cell.x;
    }

    private Vector2Int IndexToGrid(int index)
    {
        if (index < 0 || index >= mazeWidth * mazeHeight)
        {
            Debug.LogError($"Invalid index: {index}");
            return new Vector2Int(-1, -1); // Return invalid coordinates
        }
        return new Vector2Int(index % mazeWidth, index / mazeWidth);
    }

    private Vector2Int WorldToGrid(Vector3 worldPos)
    {
        int gridX = Mathf.RoundToInt((worldPos.x - topLeftX) / cellSize);
        int gridY = Mathf.RoundToInt((topLeftZ - worldPos.z) / cellSize);
        return new Vector2Int(gridX, gridY);
    }

    private Vector3 GridToWorld(Vector2Int gridPos)
    {
        float worldX = gridPos.x * cellSize + topLeftX;
        float worldZ = topLeftZ - gridPos.y * cellSize;
        return new Vector3(worldX, 0, worldZ);
    }

    private bool IsValidCell(Vector2Int cell)
    {
        return cell.x >= 0 && cell.x < mazeWidth && cell.y >= 0 && cell.y < mazeHeight;
    }

    private void OnTriggerEnter(Collider c)
    {
        if (c.name == "Truck_Thing" && !despawning) {
            despawning = true;
            sound.Play();
            Die();
            return;
        }
    }

    void Die()
    {
        StartCoroutine(DieCoroutine());
    }

    private IEnumerator DieCoroutine()
    {
        // Get the collision time and position.
        float collisionTime = gameScript.timer;
        Vector3 collisionPosition = transform.position;

        // Save collision data for this specific level.
        SaveCollisionData(collisionTime, collisionPosition);
        
        // collision
        screenTint.TintAndFade();
        gameScript.addCollision();
        yield return new WaitForSeconds(0.5f);
        Destroy(gameObject);
        yield return null;
    }

    private void SaveCollisionData(float time, Vector3 position)
    {
        // Save the collision data to PlayerPrefs, storing all collisions
        // Get the current count of saved collisions
        int collisionCount = PlayerPrefs.GetInt(collisionKeyPrefix + "Count", 0);
        Debug.Log(collisionCount);

        // Save the collision time and position for each entry
        PlayerPrefs.SetFloat(collisionKeyPrefix + "Time_" + collisionCount, time);
        //Debug.Log("Hit at:");
        //Debug.Log(time);
        PlayerPrefs.SetFloat(collisionKeyPrefix + "PosX_" + collisionCount, position.x);
        PlayerPrefs.SetFloat(collisionKeyPrefix + "PosY_" + collisionCount, position.y);
        PlayerPrefs.SetFloat(collisionKeyPrefix + "PosZ_" + collisionCount, position.z);

        // Increment and save the new collision count
        PlayerPrefs.SetInt(collisionKeyPrefix + "Count", collisionCount + 1);
        PlayerPrefs.Save();  // Save immediately to ensure persistence
    }
}
