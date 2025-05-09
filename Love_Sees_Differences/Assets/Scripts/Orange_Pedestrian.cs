using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orange_Pedestrian : MonoBehaviour
{
    [SerializeField] public float speed;

    [SerializeField] public float despawnTime = 8f;

    [SerializeField] public GameObject player;

    private Player_Movement_Boss playerMovement;

    [SerializeField] public GameObject game;

    [SerializeField] public AudioSource sound;

    private Game_Boss gameScript;

    private Screen_Tint screenTint;

    public Vector3 startingPos;

    private bool despawning;

    public string levelName;  // Will hold the level name (e.g., "DayMode_Level1")
    private string collisionKeyPrefix; // Used to differentiate between different levels
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Truck_Thing_Boss");
        playerMovement = player.GetComponent<Player_Movement_Boss>();
        game = GameObject.Find("Game_Boss");
        gameScript = game.GetComponent<Game_Boss>();
        screenTint = game.GetComponent<Screen_Tint>();
        collisionKeyPrefix = "Collision_" + levelName + "_";
        despawning = false;
        StartCoroutine(waitAndDespawn());
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(startingPos * Time.deltaTime * speed);
    }

    private IEnumerator waitAndDespawn() {
        yield return new WaitForSeconds(despawnTime);
        Destroy(gameObject);
        yield return null;
    }

    private void OnTriggerEnter(Collider c) {
        //Debug.Log("Hit Something!");
        //Debug.Log(c.name);
        if (c.name.Contains("Wall")) {
            Destroy(gameObject);
        }
        if (c.name == "Truck_Thing_Boss" && !despawning) {
            sound.Play();
            despawning = true;
            Die();
            return;
        }
    }

    void Die() {
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
        //Debug.Log(collisionCount);

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
