using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building_Mover : MonoBehaviour
{
    [SerializeField] public GameObject[] buildings; // Array of buildings to move
    [SerializeField] public Light[] positionMarkers; // Point lights that mark the new positions
    [SerializeField] public GameObject game;
    public Game gameScript; // Reference to the Game object to get the game timer

    [SerializeField] private float[] moveTimes = { 30f, 60f, 90f }; // Times for when the buildings should move (example)
    private Vector3[] targetPositions = new Vector3[3]; // New positions for the buildings

    private void Start()
    {
        gameScript = game.GetComponent<Game>();
        // Set up target positions based on point lights
        for (int i = 0; i < positionMarkers.Length; i++)
        {
            if (i < targetPositions.Length)
            {
                targetPositions[i] = positionMarkers[i].transform.position;
            }
        }
    }

    private void Update()
    {
        // Get the current game time (timer)
        float currentTime = gameScript.timer;

        // Check if it's time to move any buildings
        for (int i = 0; i < moveTimes.Length; i++)
        {
            // If current time matches one of the designated times, move the corresponding buildings
            if (Mathf.Ceil(currentTime) >= Mathf.Ceil(moveTimes[i]))
            {
                MoveBuildings(i);
                // Optionally, set the move time to a very large value so it doesn't move again
                moveTimes[i] = float.MaxValue;  // Ensures the building doesn't move multiple times
            }
        }
    }

    // Function to move buildings to the target positions at the given index
    private void MoveBuildings(int index)
    {
        // For simplicity, move all buildings (this can be modified to move specific buildings)
        for (int i = 0; i < buildings.Length; i++)
        {
            if (i == index) // Move the corresponding building to the target position
            {
                StartCoroutine(MoveBuildingToTarget(buildings[i], targetPositions[index]));
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
