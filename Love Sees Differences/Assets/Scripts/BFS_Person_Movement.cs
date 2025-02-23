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
        if (c.name == "Truck_Thing")
        {
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
        screenTint.TintAndFade();
        gameScript.addCollision();
        yield return new WaitForSeconds(0.5f);
        Destroy(gameObject);
        yield return null;
    }

    // public void SetPath(List<Vector3> bfsPath, Transform endGoal)
    // {
    //     path = bfsPath;
    //     goal = endGoal;
    // }
}
