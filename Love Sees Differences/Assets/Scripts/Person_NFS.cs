using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Person_NFS : MonoBehaviour
{
    [SerializeField] public float speed = 20f;
    [SerializeField] public float despawnRadius = 20f; // Distance at which pedestrian despawns

    [SerializeField] public GameObject player;
    private Player_Movement playerMovement;

    [SerializeField] public GameObject game;
    [SerializeField] public AudioSource sound;

    private Game gameScript;
    private Screen_Tint screenTint;


    [Header("Goal Points")]
    [SerializeField] public Transform[] goalPoints;  // Assign in Unity Inspector

    public Vector3 endGoal;

    private Vector3 lastPosition;
    private float stuckTime = 0f;

    public string levelName;  // Will hold the level name (e.g., "DayMode_Level1")
    private string collisionKeyPrefix; // Used to differentiate between different levels

    private bool despawning;

    void Start()
    {
        despawning = false;
        player = GameObject.Find("Truck_Thing");
        playerMovement = player.GetComponent<Player_Movement>();
        game = GameObject.Find("Game");
        gameScript = game.GetComponent<Game>();
        screenTint = game.GetComponent<Screen_Tint>();
        endGoal = goalPoints[Random.Range(0, goalPoints.Length)].position;
    }

    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, endGoal, speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, endGoal) <= despawnRadius)
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

        lastPosition = transform.position;
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
        Debug.Log("Hit at:");
        Debug.Log(time);
        PlayerPrefs.SetFloat(collisionKeyPrefix + "PosX_" + collisionCount, position.x);
        PlayerPrefs.SetFloat(collisionKeyPrefix + "PosY_" + collisionCount, position.y);
        PlayerPrefs.SetFloat(collisionKeyPrefix + "PosZ_" + collisionCount, position.z);

        // Increment and save the new collision count
        PlayerPrefs.SetInt(collisionKeyPrefix + "Count", collisionCount + 1);
        PlayerPrefs.Save();  // Save immediately to ensure persistence
    }
}
