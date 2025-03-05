using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionLoader : MonoBehaviour
{
    public GameObject personPrefab; // The person prefab to spawn
    public string levelName; // The level name ("NightMode_Level1", etc.)

    [SerializeField] public GameObject game;
    public Game_2 gameScript; // Reference to the Game_2 script with the timer

    private void Start()
    {
        // Load and check collision data when the level starts
        gameScript = game.GetComponent<Game_2>();
        LoadCollisionData();
    }

    private void LoadCollisionData()
    {
        string collisionKeyPrefix = "Collision_" + levelName + "_";

        // Check if collision data exists for this level
        int collisionCount = PlayerPrefs.GetInt(collisionKeyPrefix + "Count", 0);
        Debug.Log(collisionCount);

        // Loop through each stored collision and check if it should be spawned
        for (int i = 0; i < collisionCount; i++)
        {
            float collisionTime = PlayerPrefs.GetFloat(collisionKeyPrefix + "Time_" + i);
            Debug.Log("Found at:");
            Debug.Log(collisionTime);
            float x = PlayerPrefs.GetFloat(collisionKeyPrefix + "PosX_" + i);
            float y = PlayerPrefs.GetFloat(collisionKeyPrefix + "PosY_" + i);
            float z = PlayerPrefs.GetFloat(collisionKeyPrefix + "PosZ_" + i);

            Vector3 collisionPosition = new Vector3(x, y, z);

            // Start a coroutine that checks the timer and spawns the person at the right time
            StartCoroutine(WaitForCollisionTime(collisionTime, collisionPosition));
        }
    }

    private IEnumerator WaitForCollisionTime(float collisionTime, Vector3 position)
    {
        // Wait until the game timer reaches or exceeds the collision time
        while (Mathf.Ceil(gameScript.timer) < Mathf.Ceil(10f * collisionTime) / 10f)
        {
            yield return null; // Wait for the next frame
        }

        // Spawn the person at the saved position
        Instantiate(personPrefab, position, Quaternion.identity);
    }
}
