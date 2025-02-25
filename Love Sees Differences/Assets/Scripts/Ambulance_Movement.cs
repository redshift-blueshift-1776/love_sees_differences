using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ambulance_Movement : MonoBehaviour
{
    [SerializeField] private float acceleration = 10f; // Acceleration rate
    [SerializeField] private float maxSpeed = 50f; // Maximum speed
    [SerializeField] private float turnSpeed = 100f; // Steering sensitivity
    [SerializeField] private float brakeForce = 20f; // Braking power
    [SerializeField] private float friction = 0.98f; // Simulated drag
    [SerializeField] private float speedMultiplier = 2f; // Speed boost with Shift

    private float currentSpeed = 0f;
    private float currentTurnSpeed = 0f;
    private Vector3 velocity;

    void Update()
    {
        HandleMovement();
    }

    void HandleMovement()
    {
        float moveInput = 0f;
        float turnInput = 0f;

        // Forward and backward acceleration
        if (Input.GetKey(KeyCode.W))
            moveInput = 1f;
        if (Input.GetKey(KeyCode.S))
            moveInput = -1f;

        // Steering left and right
        if (Input.GetKey(KeyCode.A))
            turnInput = -1f;
        if (Input.GetKey(KeyCode.D))
            turnInput = 1f;

        // Speed boost
        float speedFactor = (Input.GetKey(KeyCode.LeftShift)) ? speedMultiplier : 1f;

        // Accelerate/decelerate
        currentSpeed += moveInput * acceleration * speedFactor * Time.deltaTime;
        currentSpeed = Mathf.Clamp(currentSpeed, -maxSpeed, maxSpeed);

        // Apply friction to slow down gradually
        currentSpeed *= friction;

        // Braking (reduce speed faster when pressing S)
        if (Input.GetKey(KeyCode.S) && currentSpeed > 0)
            currentSpeed -= brakeForce * Time.deltaTime;

        // Steering is based on speed (higher speed, harder to turn)
        if (currentSpeed != 0)
            currentTurnSpeed = turnInput * turnSpeed * (Mathf.Clamp01(10f / Mathf.Abs(currentSpeed)));

        // Apply movement
        transform.Rotate(0, currentTurnSpeed * Time.deltaTime, 0);
        transform.Translate(Vector3.forward * currentSpeed * Time.deltaTime);
    }
}
