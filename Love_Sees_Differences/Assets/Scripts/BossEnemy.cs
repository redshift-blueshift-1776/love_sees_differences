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
    [SerializeField] GameObject graphQuestionAttackTower;
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
                    if (!isInvincible) {
                        transform.position = Vector3.MoveTowards(transform.position, nextPos, speed * Time.deltaTime);
                    }

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
                StartCoroutine(changeAttackPhase());
                startedPhase3 = true;
            }
            if (healthRatio <= 0.25f && !startedPhase4) {
                phase = 4;
                Debug.Log("phase 4 entered");
                StartCoroutine(changeAttackPhase());
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
        if (isInvincible) {
            return false;
        }
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
        mazeGenerator.GenerateGraph();
        mazeGenerator.GenerateMaze();
        yield return new WaitForSeconds(2f);
        mazeGenerator.DeleteAllWallsAtOnce();
        isInvincible = false;
        yield return null;
    }

    private bool IsPlayerInRange(float range)
    {
        return Vector3.Distance(transform.position, player.transform.position) <= range;
    }

    void Attack1()
    {
        int currentBeat = BeatManager.Instance.GetCurrentBeatNumber();
        Vector3 size = new Vector3(1,1,1);
        if (phase == 1) {
            spawnOrangePedestrian(size, new Vector3(1, 0, 0), 25);
            spawnOrangePedestrian(size, new Vector3(-1, 0, 0), 25);
            spawnOrangePedestrian(size, new Vector3(0, 0, 1), 25);
            spawnOrangePedestrian(size, new Vector3(0, 0, -1), 25);
        } 

        if (phase == 2) {
            spawnOrangePedestrian(size, new Vector3(1, 0, 0), 25);
            spawnOrangePedestrian(size, new Vector3(-1, 0, 0), 25);
            spawnOrangePedestrian(size, new Vector3(0, 0, 1), 25);
            spawnOrangePedestrian(size, new Vector3(0, 0, -1), 25);
            spawnOrangePedestrian(size, new Vector3(0.6f, 0, 0.8f), 25);
            spawnOrangePedestrian(size, new Vector3(0.8f, 0, 0.6f), 25);
            spawnOrangePedestrian(size, new Vector3(-0.6f, 0, 0.8f), 25);
            spawnOrangePedestrian(size, new Vector3(-0.8f, 0, 0.6f), 25);
            spawnOrangePedestrian(size, new Vector3(0.6f, 0, -0.8f), 25);
            spawnOrangePedestrian(size, new Vector3(0.8f, 0, -0.6f), 25);
            spawnOrangePedestrian(size, new Vector3(-0.6f, 0, -0.8f), 25);
            spawnOrangePedestrian(size, new Vector3(-0.8f, 0, -0.6f), 25);
        }

        if (phase == 3) {
            if (currentBeat % 4 == 0) {
                StartCoroutine(LineAttack());
            }
            if (currentBeat % 8 == 0) {
                StartCoroutine(GraphQuestionAttack());
            }
            spawnOrangePedestrian(size, new Vector3(1, 0, 1), 50);
            spawnOrangePedestrian(size, new Vector3(-1, 0, 1), 50);
            spawnOrangePedestrian(size, new Vector3(-1, 0, 1), 50);
            spawnOrangePedestrian(size, new Vector3(-1, 0, -1), 50);
        }

        if (phase == 4) {
            if (currentBeat % 4 == 0) {
                StartCoroutine(LineAttack());
            }
            spawnOrangePedestrian(size, new Vector3(0.5f, 0, 0.5f), 25);
            spawnOrangePedestrian(size, new Vector3(-0.5f, 0, 0.5f), 25);
            spawnOrangePedestrian(size, new Vector3(0.5f, 0, -0.5f), 25);
            spawnOrangePedestrian(size, new Vector3(-0.5f, 0, -0.5f), 25);
            spawnOrangePedestrian(size, new Vector3(1, 0, 0), 25);
            spawnOrangePedestrian(size, new Vector3(-1, 0, 0), 25);
            spawnOrangePedestrian(size, new Vector3(0, 0, 1), 25);
            spawnOrangePedestrian(size, new Vector3(0, 0, -1), 25);
        }

        // Change attack
        if (currentBeat % 32 == 0) {
            changeAttack(2);
        }
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

    private IEnumerator LineAttack()
    {
        Vector3 size = new Vector3(1,1,1);
        Vector3 target = new Vector3(player.transform.position.x, player.transform.position.y, player.transform.position.z);
        for (int i = 0; i < 10; i++)
        {
            float d = Vector3.Distance(transform.position, player.transform.position);
            spawnOrangePedestrian(size, (target - transform.position) / d, 100f);
            yield return new WaitForSeconds(0.1f); // 10 bullets per second
        }
        yield break;
    }

    private bool isFirstAttack = true;

    private void Shuffle<T>(List<T> list) {
        for (int i = list.Count - 1; i > 0; i--) {
            int j = Random.Range(0, i + 1);
            T temp = list[i];
            list[i] = list[j];
            list[j] = temp;
        }
    }

    private IEnumerator spawnGraphQuestionAttackTower(Vector3 location) {
        Quaternion defaultRotation = Quaternion.Euler(0, 0, 0);
        GameObject newTower = Instantiate(graphQuestionAttackTower, location, defaultRotation);
        yield return new WaitForSeconds(2.5f);
        Destroy(newTower);
    }

    private IEnumerator GraphQuestionAttack() {
        int startBeat = BeatManager.Instance.GetCurrentBeatNumber();

        Vector3 playerPos = player.transform.position;
        List<Vector3> pathPositions = new List<Vector3> {
            playerPos + new Vector3(0, 0, 15),   // Forward
            playerPos + new Vector3(15, 0, 0),   // Right
            playerPos + new Vector3(0, 0, -15),  // Backward
            playerPos + new Vector3(-15, 0, 0)   // Left
        };

        if (!isFirstAttack) {
            Shuffle(pathPositions);
        }
        isFirstAttack = false;

        // Beats 0–3: spawn one tower per beat
        for (int i = 0; i < 4; i++) {
            int targetBeat = startBeat + i;
            yield return new WaitUntil(() => BeatManager.Instance.GetCurrentBeatNumber() >= targetBeat);
            StartCoroutine(spawnGraphQuestionAttackTower(pathPositions[i]));
        }

        // Beat 5–7: move pedestrian through points
        yield return new WaitUntil(() => BeatManager.Instance.GetCurrentBeatNumber() >= startBeat + 5);

        GameObject pedestrian = Instantiate(orangePedestrian, pathPositions[0], Quaternion.identity);

        float beatDuration = (float) BeatManager.Instance.secondsPerBeat; // Duration of a single beat in seconds
        for (int i = 1; i < pathPositions.Count; i++) {
            Vector3 start = pathPositions[i - 1];
            Vector3 end = pathPositions[i];
            float elapsed = 0f;
            while (elapsed < beatDuration) {
                if (pedestrian == null) yield break; // Exit if the object was somehow destroyed
                pedestrian.transform.position = Vector3.Lerp(start, end, elapsed / beatDuration);
                elapsed += Time.deltaTime;
                yield return null;
            }
            pedestrian.transform.position = end;

            // Wait until next beat
            yield return new WaitUntil(() => BeatManager.Instance.GetCurrentBeatNumber() >= startBeat + 5 + i);
        }

        Destroy(pedestrian);
    }

    void Attack2()
    {
        int currentBeat = BeatManager.Instance.GetCurrentBeatNumber();
        Vector3 size = new Vector3(1,1,1);
        if (phase == 1) {
            spawnOrangePedestrian(size, new Vector3(1, 0, 1), 25);
            spawnOrangePedestrian(size, new Vector3(-1, 0, 1), 25);
            spawnOrangePedestrian(size, new Vector3(1, 0, -1), 25);
            spawnOrangePedestrian(size, new Vector3(-1, 0, -1), 25);
            Vector3 target = new Vector3(player.transform.position.x, player.transform.position.y, player.transform.position.z);
            float d = Vector3.Distance(transform.position, player.transform.position);
            spawnOrangePedestrian(size, (target - transform.position) / d, 50f);
        }

        if (phase == 2) {
            spawnOrangePedestrian(size, new Vector3(1, 0, 1), 25);
            spawnOrangePedestrian(size, new Vector3(-1, 0, 1), 25);
            spawnOrangePedestrian(size, new Vector3(1, 0, -1), 25);
            spawnOrangePedestrian(size, new Vector3(-1, 0, -1), 25);
            spawnOrangePedestrian(size, new Vector3(1, 0, 0), 25);
            spawnOrangePedestrian(size, new Vector3(-1, 0, 0), 25);
            spawnOrangePedestrian(size, new Vector3(0, 0, 1), 25);
            spawnOrangePedestrian(size, new Vector3(0, 0, -1), 25);
            Vector3 target = new Vector3(player.transform.position.x, player.transform.position.y, player.transform.position.z);
            float d = Vector3.Distance(transform.position, player.transform.position);
            spawnOrangePedestrian(size, (target - transform.position) / d, 50f);
            target = new Vector3(player.transform.position.x + 5, player.transform.position.y, player.transform.position.z);
            d = Vector3.Distance(transform.position, target);
            spawnOrangePedestrian(size, (target - transform.position) / d, 50f);
            target = new Vector3(player.transform.position.x - 5, player.transform.position.y, player.transform.position.z);
            d = Vector3.Distance(transform.position, target);
            spawnOrangePedestrian(size, (target - transform.position) / d, 50f);
            if (currentBeat % 8 == 0) {
                StartCoroutine(GraphQuestionAttack());
            }
        }

        if (phase == 3) {
            if (currentBeat % 8 == 0) {
                StartCoroutine(GraphQuestionAttack());
            }
            Vector3 target = new Vector3(player.transform.position.x, player.transform.position.y, player.transform.position.z);
            float d = Vector3.Distance(transform.position, target);
            spawnOrangePedestrian(size, (target - transform.position) / d, 75f);
            target = new Vector3(player.transform.position.x + 5, player.transform.position.y, player.transform.position.z);
            d = Vector3.Distance(transform.position, target);
            spawnOrangePedestrian(size, (target - transform.position) / d, 75f);
            target = new Vector3(player.transform.position.x - 5, player.transform.position.y, player.transform.position.z);
            d = Vector3.Distance(transform.position, target);
            spawnOrangePedestrian(size, (target - transform.position) / d, 75f);
            target = new Vector3(player.transform.position.x + 10, player.transform.position.y, player.transform.position.z);
            d = Vector3.Distance(transform.position, target);
            spawnOrangePedestrian(size, (target - transform.position) / d, 75f);
            target = new Vector3(player.transform.position.x - 10, player.transform.position.y, player.transform.position.z);
            d = Vector3.Distance(transform.position, target);
            spawnOrangePedestrian(size, (target - transform.position) / d, 75f);
        }

        if (phase == 4) {
            if (currentBeat % 4 == 0) {
                StartCoroutine(LineAttack());
            }
            spawnOrangePedestrian(size, new Vector3(1, 0, 0), 25);
            spawnOrangePedestrian(size, new Vector3(-1, 0, 0), 25);
            spawnOrangePedestrian(size, new Vector3(0, 0, 1), 25);
            spawnOrangePedestrian(size, new Vector3(0, 0, -1), 25);
            spawnOrangePedestrian(size, new Vector3(0.6f, 0, 0.8f), 25);
            spawnOrangePedestrian(size, new Vector3(0.8f, 0, 0.6f), 25);
            spawnOrangePedestrian(size, new Vector3(-0.6f, 0, 0.8f), 25);
            spawnOrangePedestrian(size, new Vector3(-0.8f, 0, 0.6f), 25);
            spawnOrangePedestrian(size, new Vector3(0.6f, 0, -0.8f), 25);
            spawnOrangePedestrian(size, new Vector3(0.8f, 0, -0.6f), 25);
            spawnOrangePedestrian(size, new Vector3(-0.6f, 0, -0.8f), 25);
            spawnOrangePedestrian(size, new Vector3(-0.8f, 0, -0.6f), 25);
        }

        // Change attack
        if (currentBeat % 32 == 0) {
            changeAttack(1);
        }
    }

    void Die() {
        StartCoroutine(DieCoroutine());
    }

    private IEnumerator DieCoroutine()
    {
        //Debug.Log("test");
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

