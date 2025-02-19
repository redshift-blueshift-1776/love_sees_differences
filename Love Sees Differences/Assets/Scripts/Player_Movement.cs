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
    [SerializeField] private float playerSpeed = 5.0f;
    private float jumpHeight = 10.0f;
    private float gravityValue = -20f;

    private float jumpVelocity;

    private float playerMass = 120;

    private float mouseSensitivity = 1;

    [SerializeField] private float speedUp = 5f;

    [SerializeField] public GameObject BuildingW;
    [SerializeField] public GameObject BuildingA;
    [SerializeField] public GameObject BuildingS;
    [SerializeField] public GameObject BuildingD;

    public bool polarW;
    public bool polarA;
    public bool polarS;
    public bool polarD;

    public bool selfPolar;

    private void Start()
    {
        jumpVelocity = Mathf.Sqrt(-2 * gravityValue * jumpHeight);
        print(jumpVelocity);
        controller = gameObject.GetComponent<CharacterController>();
        // set the skin width appropriately according to Unity documentation: https://docs.unity3d.com/Manual/class-CharacterController.html
        controller.skinWidth = 0.1f * controller.radius;
        //Cursor.visible = false;
        //Cursor.lockState = CursorLockMode.Locked;
        mouseSensitivity = PlayerPrefs.GetFloat("MouseSensitivity", 1.0f);
        polarW = false;
        polarA = false;
        polarS = false;
        polarD = false;
        selfPolar = false;
    }

    void Update()
    {
        // Controls here
        // If W, A, S, or D pressed, swap the value of the bool for polar of that letter
        if (Input.GetKeyDown(KeyCode.W))
            polarW = !polarW;
        if (Input.GetKeyDown(KeyCode.A))
            polarA = !polarA;
        if (Input.GetKeyDown(KeyCode.S))
            polarS = !polarS;
        if (Input.GetKeyDown(KeyCode.D))
            polarD = !polarD;
        // If space pressed, swap the value of selfPolar
        if (Input.GetKeyDown(KeyCode.Space))
            selfPolar = !selfPolar;
        // Actual Movement here:
        // Get vectors towards Buildings W, A, S, and D.
        Vector3 vecW = BuildingW.transform.position - transform.position;
        Vector3 vecA = BuildingA.transform.position - transform.position;
        Vector3 vecS = BuildingS.transform.position - transform.position;
        Vector3 vecD = BuildingD.transform.position - transform.position;
        // Normalize the four vectors.
        vecW.Normalize();
        vecA.Normalize();
        vecS.Normalize();
        vecD.Normalize();
        // If polarW is the same as selfPolar, reverse the direction for the vector towards W.
        // Same for A, S, and D.
        if (polarW == selfPolar)
            vecW = -vecW;
        if (polarA == selfPolar)
            vecA = -vecA;
        if (polarS == selfPolar)
            vecS = -vecS;
        if (polarD == selfPolar)
            vecD = -vecD;
        // Add the four vectors, and move the character in that direction
        Vector3 move = vecW + vecA + vecS + vecD;

        move *= playerSpeed;
        // If left shift pressed, multiply the speed by speedUp.
        if (Input.GetKey(KeyCode.LeftShift))
            move *= speedUp;
        
        groundedPlayer = (transform.position.y < 6);
        if (groundedPlayer && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f;
        }

        controller.Move(move * Time.deltaTime);

        // If B pressed, jump
        if (Input.GetKeyDown(KeyCode.B) && groundedPlayer)
        {
            playerVelocity.y = jumpVelocity;
        }

        playerVelocity.y += gravityValue * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);

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