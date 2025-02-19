using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player_Movement : MonoBehaviour
{
    private CharacterController controller;

    private Vector3 playerVelocity = new Vector3(0,0,0);
    private bool groundedPlayer;
    private float playerSpeed = 5.0f;
    private float jumpHeight = 10.0f;
    private float gravityValue = -20f;

    private float jumpVelocity;

    private float playerMass = 120;

    private float mouseSensitivity = 1;

    private float speedUp = 5f;

    [SerializeField] public GameObject BuildingW;
    [SerializeField] public GameObject BuildingA;
    [SerializeField] public GameObject BuildingS;
    [SerializeField] public GameObject BuildingD;

    private bool polarW;
    private bool polarA;
    private bool polarS;
    private bool polarD;

    private bool selfPolar;

    private void Start()
    {
        jumpVelocity = Mathf.Sqrt(-2 * gravityValue * jumpHeight);
        print(jumpVelocity);
        controller = gameObject.GetComponent<CharacterController>();
        // set the skin width appropriately according to Unity documentation: https://docs.unity3d.com/Manual/class-CharacterController.html
        controller.skinWidth = 0.1f * controller.radius;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        mouseSensitivity = PlayerPrefs.GetFloat("MouseSensitivity", 1.0f);
    }

    void Update()
    {
        // Controls here
        // If W, A, S, or D pressed, swap the value of the bool for polar of that letter

        // If space pressed, swap the value of selfPolar
        
        // Actual Movement here:
        // Get vectors towards Buildings W, A, S, and D.

        // Normalize the four vectors.

        // If polarW is the same as selfPolar, reverse the direction for the vector towards W.
        // Same for A, S, and D.

        // Add the four vectors, and move the character in that direction
        // If left shift pressed, multiply the speed by speedUp.

        // If B pressed, jump

    }

    void OnControllerColliderHit(ControllerColliderHit hit) {
        if (hit.rigidbody != null) {
            Vector3 horizontalDir = new Vector3(hit.moveDirection.x, 0, hit.moveDirection.z);
            hit.rigidbody.AddForce(horizontalDir * 10);
        }

        if (hit.gameObject.name == "Death Zone") {
            int sceneID = SceneManager.GetActiveScene().buildIndex;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            if (sceneID == 1) {
                SceneManager.LoadScene(2);
            }
            if (sceneID == 3) {
                SceneManager.LoadScene(4);
            }
            if (sceneID == 5) {
                SceneManager.LoadScene(6);
            }
            if (sceneID == 7) {
                SceneManager.LoadScene(8);
            }
            if (sceneID == 9) {
                SceneManager.LoadScene(10);
            }
            if (sceneID == 11) {
                SceneManager.LoadScene(12);
            }
            if (sceneID == 13) {
                SceneManager.LoadScene(14);
            }
            if (sceneID == 15) {
                SceneManager.LoadScene(16);
            }
            if (sceneID == 17) {
                SceneManager.LoadScene(18);
            }
            
        }
    }
}