using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    private Rigidbody rb;

    [SerializeField] private GameObject lightSource;
    private float lightInterval = 0.37f;

    [SerializeField] public GameObject game;

    private Game_2 gameScript;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        gameScript = game.GetComponent<Game_2>();
        rb.freezeRotation = true; // Prevent the ambulance from tipping over
        StartCoroutine(Lights());
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.M)) {
            SceneManager.LoadScene(0);
        }
    }

    void FixedUpdate()
    {
        if (gameScript.gameActive) {
            HandleMovement();
        }
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
            turnInput = -1f * moveInput;
        if (Input.GetKey(KeyCode.D))
            turnInput = 1f * moveInput;

        // Speed boost
        float speedFactor = (Input.GetKey(KeyCode.LeftShift)) ? speedMultiplier : 1f;

        // Accelerate/decelerate
        currentSpeed += moveInput * acceleration * Time.fixedDeltaTime;
        currentSpeed = Mathf.Clamp(currentSpeed, -maxSpeed, maxSpeed) * speedFactor;

        // Apply friction to slow down gradually
        currentSpeed *= friction;

        // Braking (reduce speed faster when pressing S)
        if (Input.GetKey(KeyCode.S) && currentSpeed > 0)
            currentSpeed -= brakeForce * Time.fixedDeltaTime;

        // Steering is based on speed (higher speed, harder to turn)
        if (currentSpeed != 0)
            currentTurnSpeed = turnInput * turnSpeed * (Mathf.Clamp01(10f / Mathf.Abs(currentSpeed)));

        // Apply movement and rotation using Rigidbody
        rb.velocity = transform.forward * currentSpeed;
        rb.MoveRotation(rb.rotation * Quaternion.Euler(0, currentTurnSpeed * speedFactor * Time.fixedDeltaTime, 0));
    }

    private IEnumerator Lights() {
        while (true)
        {
            yield return new WaitForSeconds(lightInterval);
            if (lightSource.activeSelf) {
                lightSource.SetActive(false);
            } else {
                lightSource.SetActive(true);
            }
        }
    }
}
