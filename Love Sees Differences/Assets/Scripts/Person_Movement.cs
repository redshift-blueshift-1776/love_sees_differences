using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Person_Movement : MonoBehaviour
{
    [SerializeField] public float speed;

    [SerializeField] public GameObject player;

    private Player_Movement playerMovement;

    [SerializeField] public GameObject game;

    [SerializeField] public AudioSource sound;

    private Game gameScript;

    private Screen_Tint screenTint;

    public Vector3 startingPos;

    private bool despawning;

    public string levelName;  // Will hold the level name (e.g., "DayMode_Level1")
    private string collisionKeyPrefix; // Used to differentiate between different levels
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Truck_Thing");
        playerMovement = player.GetComponent<Player_Movement>();
        game = GameObject.Find("Game");
        gameScript = game.GetComponent<Game>();
        screenTint = game.GetComponent<Screen_Tint>();
        collisionKeyPrefix = "Collision_" + levelName + "_";
        despawning = false;
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(startingPos * Time.deltaTime * speed);
        StartCoroutine(waitAndDespawn());
    }

    private IEnumerator waitAndDespawn() {
        yield return new WaitForSeconds(10f);
        Destroy(gameObject);
        yield return null;
    }

    private void OnTriggerEnter(Collider c) {
        //Debug.Log("Hit Something!");
        //Debug.Log(c.name);
        if (c.name == "Truck_Thing" && !despawning) {
            despawning = true;
            sound.Play();
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

}
