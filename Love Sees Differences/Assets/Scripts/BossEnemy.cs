using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum EnemyState
{
    Wander,
    Follow,
    Die,
    Attack1,
    Attack2,
    Attack3
};

public class BossEnemy : MonoBehaviour
{

    public EnemyState currState;
    public float range;

    Rigidbody myRigidbody;

    public int HP;
    public int maxHP;
    Color og;
    Color transparent;

    [SerializeField] public GameObject player;

    private bool gotHit = false;
    GameObject newBullet;

    public int phase;

    [SerializeField] public GameObject Game;

    public Game_Boss gameScript;

    public int xBound;
    bool direction;

    public bool isInvincible = false;

    bool startedPhase2 = false;
    bool startedPhase3 = false;
    bool startedPhase4 = false;

    [SerializeField] GameObject orangePedestrian;
    [SerializeField] float speed;

    [Header("Maze Configuration")]
    [SerializeField] public Maze_Generator mazeGenerator;  // Reference to the maze
    [SerializeField] public int mazeWidth = 5;  // X-axis size
    [SerializeField] public int mazeHeight = 5; // Y-axis size

    [Header("Grid Settings")]
    [SerializeField] public float cellSize = 50f;  // Distance between grid points in Unity world units
    [SerializeField] public float topLeftX = -100f;  // Distance between grid points in Unity world units
    [SerializeField] public float topLeftZ = 100f;  // Distance between grid points in Unity world units

    private Queue<Vector3> pathQueue = new Queue<Vector3>();
    private bool isMoving = false;

    private Vector2Int startCell;
    private Vector2Int goalCell;
    private Dictionary<int, List<int>> adjacencyList;

    private Vector3 lastPosition;

    private int lastProcessedBeat = -1;

    // Start is called before the first frame update
    void Start()
    {
        phase = 1;
        //maxHP = 4000;
        //HP = 4000;
        //moveSpeed = 5;
        currState = EnemyState.Attack1;
        HP = 100;
        maxHP = 100;
        myRigidbody = GetComponent<Rigidbody>();
        gameScript = Game.GetComponent<Game_Boss>();
        transparent = new Color(og.r, og.g, og.b, 0.5f);
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
    }

    // Update is called once per frame
    void Update()
    {
        if (gameScript.gameActive) {
            adjacencyList = BuildAdjacencyList(mazeGenerator);
            startCell = WorldToGrid(transform.position);
            if (goalCell != WorldToGrid(player.transform.position)) {
                goalCell = WorldToGrid(player.transform.position);
                pathQueue = new Queue<Vector3>();
            }

            if (startCell != goalCell) {
                List<Vector3> path = FindPathBFS(startCell, goalCell);
                if (path.Count > 0)
                {
                    foreach (var pos in path)
                    {
                        pathQueue.Enqueue(pos);
                        //Debug.Log(pos);
                    }
                }
                if (pathQueue.Count > 0)
                {
                    Vector3 nextPos = pathQueue.Peek();
                    //Debug.Log(nextPos);
                    transform.position = Vector3.MoveTowards(transform.position, nextPos, speed * Time.deltaTime);

                    if (Vector3.Distance(transform.position, nextPos) < 15f)
                    {
                        pathQueue.Dequeue();
                    }
                }
            }
            if (HP <= 0) {
                currState = EnemyState.Die;
            }

            float healthRatio = (float)HP / (float)maxHP;
            if (healthRatio <= 0.75f && !startedPhase2) {
                phase = 2;
                Debug.Log("phase 2 entered");
                StartCoroutine(changeAttackPhase());
                startedPhase2 = true;
            }
            if (healthRatio <= 0.50f && !startedPhase3) {
                phase = 3;
                Debug.Log("phase 3 entered");
                startedPhase3 = true;
            }
            if (healthRatio <= 0.25f && !startedPhase4) {
                phase = 4;
                Debug.Log("phase 4 entered");
                startedPhase4 = true;
            }

            // Beat-based attack logic
            int currentBeat = BeatManager.Instance.GetCurrentBeatNumber();
            //Debug.Log("Beat: " + currentBeat);
            if (currentBeat > lastProcessedBeat) {
                lastProcessedBeat = currentBeat;
                //Debug.Log("Attack");

                // Only trigger attacks on the beat
                if (ShouldAttackThisBeat(currentBeat)) {
                    HandleAttackOnBeat();
                }
            }

            // Any non-beat logic
            if (gotHit) {
                // respond to getting hit
            }
        }
    }

    private void HandleAttackOnBeat() {
        switch (currState) {
            case EnemyState.Attack1:
                Attack1();
                break;
            case EnemyState.Attack2:
                Attack2();
                break;
            case EnemyState.Die:
                Die();
                break;
            default:
                ChooseNextAttack(); // e.g., cycle or randomize attacks
                break;
        }
    }

    private void ChooseNextAttack() {
        switch (phase) {
            case 1:
                currState = EnemyState.Attack1;
                break;
            case 2:
                currState = (Random.value < 0.5f) ? EnemyState.Attack1 : EnemyState.Attack2;
                break;
            case 3:
                currState = EnemyState.Attack2;
                break;
            case 4:
                currState = (Random.value < 0.5f) ? EnemyState.Attack1 : EnemyState.Attack2;
                break;
        }
    }

    bool ShouldAttackThisBeat(int beat) {
        switch (phase) {
            case 1:
                return beat % 8 == 0; // Slower
            case 2:
                return beat % 4 == 0; // Medium
            case 3:
                return beat % 4 == 0 || beat % 8 == 2; // More aggressive
            case 4:
                return beat % 2 == 0; // Fast
            default:
                return false;
        }
    }


    private void OnTriggerEnter(Collider c)
    {
        if (c.name.Contains("Arrow") && currState != EnemyState.Die && !isInvincible) {
            Destroy(c.gameObject);
            HP -= 1;
        } else if (c.name == "Truck_Thing_Boss") {
            if (!player.GetComponent<Player_Movement_Boss>().isInvincible) {
                gameScript.addCollision();
            }
        }
    }


    void changePhase(int i) {
        phase = i;
    }

    private IEnumerator changeAttackPhase()
    {
        isInvincible = true;
        // Will do a cutscene?
        yield return new WaitForSeconds(.5f);
        isInvincible = false;
        yield return null;
    }

    private bool IsPlayerInRange(float range)
    {
        return Vector3.Distance(transform.position, player.transform.position) <= range;
    }

    void Attack1()
    {
        Vector3 size = new Vector3(1,1,1);
        if (phase == 1) {
            spawnOrangePedestrian(size, new Vector3(1, 0, 0), 20);
            spawnOrangePedestrian(size, new Vector3(-1, 0, 0), 20);
            spawnOrangePedestrian(size, new Vector3(0, 0, 1), 20);
            spawnOrangePedestrian(size, new Vector3(0, 0, -1), 20);
        } 

        if (phase == 2) {
            spawnOrangePedestrian(size, new Vector3(1, 0, 0), 20);
            spawnOrangePedestrian(size, new Vector3(-1, 0, 0), 20);
            spawnOrangePedestrian(size, new Vector3(0, 0, 1), 20);
            spawnOrangePedestrian(size, new Vector3(0, 0, -1), 20);
            spawnOrangePedestrian(size, new Vector3(0.6f, 0, 0.8f), 20);
            spawnOrangePedestrian(size, new Vector3(0.8f, 0, 0.6f), 20);
            spawnOrangePedestrian(size, new Vector3(-0.6f, 0, 0.8f), 20);
            spawnOrangePedestrian(size, new Vector3(-0.8f, 0, 0.6f), 20);
            spawnOrangePedestrian(size, new Vector3(0.6f, 0, -0.8f), 20);
            spawnOrangePedestrian(size, new Vector3(0.8f, 0, -0.6f), 20);
            spawnOrangePedestrian(size, new Vector3(-0.6f, 0, -0.8f), 20);
            spawnOrangePedestrian(size, new Vector3(-0.8f, 0, -0.6f), 20);
        }

        if (phase == 3) {
            
        }

        if (phase == 4) {
            
        }

        // Change attack
    }

    void changeAttack (int i) {
        switch (i)
        {
            case (1):
                currState = EnemyState.Attack1;
                break;
            case (2):
                currState = EnemyState.Attack2;
                break;
        }    
    }

    void spawnOrangePedestrian(Vector3 size, Vector3 pedestrianDirection, float speed) {
        GameObject newPerson = Instantiate(orangePedestrian, transform.position, transform.rotation);
        newPerson.transform.localScale = size;
        newPerson.GetComponent<Orange_Pedestrian>().startingPos = pedestrianDirection;
        newPerson.GetComponent<Orange_Pedestrian>().speed = speed;
    }

    void Attack2()
    {
        if (phase == 1) {
            Vector3 size = new Vector3(1,1,1);
            spawnOrangePedestrian(size, new Vector3(1, 0, 1), 20);
            spawnOrangePedestrian(size, new Vector3(-1, 0, 1), 20);
            spawnOrangePedestrian(size, new Vector3(-1, 0, 1), 20);
            spawnOrangePedestrian(size, new Vector3(-1, 0, -1), 20);
        }

        if (phase == 2) {

        }

        if (phase == 3) {
            
        }

        if (phase == 4) {
            
        }

        // Change attack
    }

    void Die() {
        StartCoroutine(DieCoroutine());
    }

    private IEnumerator DieCoroutine()
    {
        Debug.Log("test");
        player.GetComponent<Player_Movement_Boss>().isInvincible = true;
        yield return new WaitForSeconds(3f);
        //SceneManager.LoadScene(2); //Replace with actual scene when we make it
        Destroy(gameObject);
        yield return null;
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


}

