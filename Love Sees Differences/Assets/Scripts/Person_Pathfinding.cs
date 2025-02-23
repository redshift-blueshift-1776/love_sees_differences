using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Person_Pathfinding : MonoBehaviour
{
    [SerializeField] private float speed = 2f;
    
    [Header("Maze Configuration")]
    [SerializeField] private Maze_Generator mazeGenerator;  // Reference to the maze
    [SerializeField] private int mazeWidth = 5;  // X-axis size
    [SerializeField] private int mazeHeight = 5; // Y-axis size

    [Header("Grid Settings")]
    [SerializeField] private float cellSize = 50f;  // Distance between grid points in Unity world units
    [SerializeField] private float topLeftX = -100f;  // Distance between grid points in Unity world units
    [SerializeField] private float topLeftZ = 100f;  // Distance between grid points in Unity world units


    [Header("Goal Points")]
    [SerializeField] private Transform[] goalPoints;  // Assign in Unity Inspector

    private Queue<Vector3> pathQueue = new Queue<Vector3>();
    private bool isMoving = false;

    private Vector2Int startCell;
    private Vector2Int goalCell;
    private Dictionary<int, List<int>> adjacencyList;

    void Start()
    {
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
        Debug.Log(startCell);

        // Choose a random goal
        if (goalPoints.Length > 0)
        {
            goalCell = WorldToGrid(goalPoints[Random.Range(0, goalPoints.Length)].position);
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
                Debug.Log(pos);
            }
            isMoving = true;
        }
    }

    void Update()
    {
        if (isMoving && pathQueue.Count > 0)
        {
            Vector3 nextPos = pathQueue.Peek();
            transform.position = Vector3.MoveTowards(transform.position, nextPos, speed * Time.deltaTime);

            if (Vector3.Distance(transform.position, nextPos) < 0.1f)
            {
                pathQueue.Dequeue();
            }
        }
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
        float worldZ = gridPos.y * cellSize + topLeftZ;
        return new Vector3(worldX, 0, worldZ);
    }

    private bool IsValidCell(Vector2Int cell)
    {
        return cell.x >= 0 && cell.x < mazeWidth && cell.y >= 0 && cell.y < mazeHeight;
    }
}
