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
        groundedPlayer = controller.isGrounded;
        if (groundedPlayer && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f;
        }

        // Changes the height position of the player..
        if (Input.GetButtonDown("Jump") && groundedPlayer)
        {
            playerVelocity.y += jumpVelocity;
        }
        playerVelocity.y += gravityValue * Time.deltaTime;
        playerVelocity.x = 0;
        playerVelocity.z = 0;
        if (Input.GetKey(KeyCode.LeftShift)) {
            playerSpeed = 5f * speedUp;
        } else {
            playerSpeed = 5f;
        }
        if (Input.GetKey(KeyCode.F)) {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            int sceneID = SceneManager.GetActiveScene().buildIndex;
            SceneManager.LoadScene(sceneID + 1);
        }
        if (Input.GetKey(KeyCode.P)) {
            //Cursor.visible = true;
            //Cursor.lockState = CursorLockMode.None;
            int sceneID = SceneManager.GetActiveScene().buildIndex;
            if (sceneID == 17) {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }
            SceneManager.LoadScene(sceneID + 2);
        }
        playerVelocity += (gameObject.transform.right * Input.GetAxis("Horizontal") + gameObject.transform.forward * Input.GetAxis("Vertical")) * playerSpeed;
        controller.Move(playerVelocity * Time.deltaTime);


        // Rotates the camera
        float rotX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float rotY = Math.Clamp(-Input.GetAxis("Mouse Y") * mouseSensitivity, -90, 90);
        //Camera.main.transform.Rotate(rotY, 0, 0);
        gameObject.transform.Rotate(0, rotX, 0);

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