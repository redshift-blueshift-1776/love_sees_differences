using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BFS_Person_Movement : MonoBehaviour
{
    [SerializeField] public float speed;
    [SerializeField] private Transform goal;  // The target endpoint
    [SerializeField] private float despawnRadius = 20f; // Distance at which pedestrian despawns

    [SerializeField] public GameObject player;
    private Player_Movement playerMovement;

    [SerializeField] public GameObject game;
    [SerializeField] public AudioSource sound;

    private Game gameScript;
    private Screen_Tint screenTint;

    private Vector3 nextPosition; // Next position from BFS pathfinding
    private int pathIndex; // Index in the path

    public string levelName;  // Will hold the level name (e.g., "DayMode_Level1")
    private string collisionKeyPrefix; // Used to differentiate between different levels

    private bool despawning;

    private List<Vector3> path; // The path from BFS

    void Start()
    {
        player = GameObject.Find("Truck_Thing");
        playerMovement = player.GetComponent<Player_Movement>();
        game = GameObject.Find("Game");
        gameScript = game.GetComponent<Game>();
        screenTint = game.GetComponent<Screen_Tint>();

        //pathIndex = 0;
    }

    void Update()
    {
        // Move along the BFS path
        // if (path != null && pathIndex < path.Count)
        // {
        //     Vector3 direction = (path[pathIndex] - transform.position).normalized;
        //     transform.position += direction * speed * Time.deltaTime;

        //     // Check if we reached the current path point
        //     if (Vector3.Distance(transform.position, path[pathIndex]) < 1f)
        //     {
        //         pathIndex++; // Move to the next path step
        //     }
        // }

        // Check if we reached the goal
        if (Vector3.Distance(transform.position, goal.position) <= despawnRadius)
        {
            Destroy(gameObject); // Despawn pedestrian
        }
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
        float collisionTime = Time.time;
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

        // Save the collision time and position for each entry
        PlayerPrefs.SetFloat(collisionKeyPrefix + "Time_" + collisionCount, time);
        PlayerPrefs.SetFloat(collisionKeyPrefix + "PosX_" + collisionCount, position.x);
        PlayerPrefs.SetFloat(collisionKeyPrefix + "PosY_" + collisionCount, position.y);
        PlayerPrefs.SetFloat(collisionKeyPrefix + "PosZ_" + collisionCount, position.z);

        // Increment and save the new collision count
        PlayerPrefs.SetInt(collisionKeyPrefix + "Count", collisionCount + 1);
        PlayerPrefs.Save();  // Save immediately to ensure persistence
    }

    // public void SetPath(List<Vector3> bfsPath, Transform endGoal)
    // {
    //     path = bfsPath;
    //     goal = endGoal;
    // }
}
