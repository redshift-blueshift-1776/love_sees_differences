using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Person_NFS : MonoBehaviour
{
    [SerializeField] public float speed = 20f;
    [SerializeField] public float despawnTime = 30f;
    [SerializeField] public float despawnRadius = 20f; // Distance at which pedestrian despawns

    [SerializeField] public GameObject defaultColor;
    [SerializeField] public GameObject WColor;
    [SerializeField] public GameObject AColor;
    [SerializeField] public GameObject SColor;
    [SerializeField] public GameObject DColor;

    public int type; // 0 is default, 1 goes to W, 2 goes to A, 3 goes to S, 4 goes to D.

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

    public float probabilityOfDefault = 0.5f;

    private const float pickupRadius = 20.0f;

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
        collisionKeyPrefix = "Collision_" + levelName + "_";
        float distance0 = Vector3.Distance(transform.position, goalPoints[0].position);
        float distance1 = Vector3.Distance(transform.position, goalPoints[1].position);
        float distance2 = Vector3.Distance(transform.position, goalPoints[2].position);
        float distance3 = Vector3.Distance(transform.position, goalPoints[3].position);
        float minDistance = Mathf.Min(distance0, distance1, distance2, distance3);
        int closest = -1;
        if (distance0 == minDistance) {
            closest = 0;
        } else if (distance1 == minDistance) {
            closest = 1;
        } else if (distance2 == minDistance) {
            closest = 2;
        } else if (distance3 == minDistance) {
            closest = 3;
        } else {
            closest = Random.Range(0, 3);
        }
        endGoal = goalPoints[closest].position;
        bool isDefault = Random.Range(0f, 1f) < probabilityOfDefault;
        if (isDefault) {
            type = 0;
        } else {
            type = closest + 1;
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
    }

    private IEnumerator waitAndDespawn() {
        yield return new WaitForSeconds(despawnTime);
        Destroy(gameObject);
        yield return null;
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
