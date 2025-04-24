using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building_Mover_Night_Endless : MonoBehaviour
{
    [SerializeField] public GameObject[] buildings; // Array of buildings to move
    [SerializeField] public Light[] positionMarkers; // Point lights that mark the new positions
    [SerializeField] public GameObject game;
    public Game_2_Endless gameScript; // Reference to the Game object to get the game timer

    //[SerializeField] private float[] moveTimes = { 30f, 60f, 90f }; // Times for when the buildings should move (example)
    [SerializeField] private float moveInterval = 60;
    public Vector3[] targetPositions = new Vector3[12]; // New positions for the buildings

    [SerializeField] public int rotation;

    private void Start()
    {
        rotation = 1;
        gameScript = game.GetComponent<Game_2_Endless>();
        // Set up target positions based on point lights
        for (int i = 0; i < positionMarkers.Length; i++)
        {
            if (i < targetPositions.Length)
            {
                targetPositions[i] = positionMarkers[i].transform.position;
            }
            Debug.Log(i);
        }
    }

    private void Update()
    {
        // Get the current game time (timer)
        float currentTime = gameScript.timer;
        float nextMoveTime = rotation * moveInterval;

        if (Mathf.Ceil(currentTime) >= Mathf.Ceil(nextMoveTime))
        {
            Debug.Log("Moving a building");
            MoveBuildings(0 + 4 * rotation - 4);
            MoveBuildings(1 + 4 * rotation - 4);
            MoveBuildings(2 + 4 * rotation - 4);
            MoveBuildings(3 + 4 * rotation - 4);
            // Optionally, set the move time to a very large value so it doesn't move again
            rotation++;
        }
    }

    // Function to move buildings to the target positions at the given index
    private void MoveBuildings(int index)
    {
        // For simplicity, move all buildings (this can be modified to move specific buildings)
        for (int i = 0; i < 4; i++)
        {
            if (i == index % 4) // Move the corresponding building to the target position
            {
                Debug.Log("MoveBuildingToTarget" + i % buildings.Length + " " + index % buildings.Length);
                StartCoroutine(MoveBuildingToTarget(buildings[index % buildings.Length], targetPositions[index % targetPositions.Length]));
            }
        }
    }

    // Coroutine to move the building smoothly
    private IEnumerator MoveBuildingToTarget(GameObject building, Vector3 targetPosition)
    {
        float timeToMove = 3f; // Duration to move the building (you can adjust this)
        Vector3 startPosition = building.transform.position;
        float elapsedTime = 0f;

        while (elapsedTime < timeToMove)
        {
            building.transform.position = Vector3.Lerp(startPosition, targetPosition, (elapsedTime / timeToMove));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure the building reaches the target position exactly
        building.transform.position = targetPosition;
    }
}
