using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player_Movement_Boss : MonoBehaviour
{
    private CharacterController controller;

    private Vector3 playerVelocity = new Vector3(0,0,0);
    private bool groundedPlayer;
    [SerializeField] private float playerSpeed = 5.0f;
    private float jumpHeight = 25.0f;
    private float gravityValue = -64f;

    private float jumpVelocity;

    //private float playerMass = 120;

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

    public bool boosted;

    public bool jumping;

    [SerializeField] public GameObject game;

    private Game_Boss gameScript;

    private int peopleCarriedW;
    private int peopleCarriedA;
    private int peopleCarriedS;
    private int peopleCarriedD;
    private int peopleCarried;

    [SerializeField] public GameObject mainCamera;
    [SerializeField] public GameObject alternateCamera;

    [SerializeField] private GameObject[] passengers;

    [SerializeField] public GameObject arrow;

    [SerializeField] public GameObject orangeGlow;

    public bool isInvincible;

    private void Start()
    {
        gameScript = game.GetComponent<Game_Boss>();
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
        boosted = false;
        isInvincible = false;

        mainCamera.SetActive(true);
        alternateCamera.SetActive(false);
        orangeGlow.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M)) {
            SceneManager.LoadScene(0);
        }

        if (gameScript.gameActive && Input.GetKeyDown(KeyCode.G)) {
            if (gameScript.fireArrow()) {
                var projectile = Instantiate(arrow, transform);
                projectile.SetActive(true);
                var projectileScript = projectile.GetComponent<Arrow>();
                projectileScript.targetPosition = gameScript.boss.transform.position;
                projectileScript.startPosition = transform.position;
            }
        }

        if (gameScript.gameActive && Input.GetKeyDown(KeyCode.C)) {
            mainCamera.SetActive(!mainCamera.activeSelf);
            alternateCamera.SetActive(!alternateCamera.activeSelf);
        }
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
        if (Input.GetKey(KeyCode.LeftShift) || boosted)
            move *= speedUp;
        
        //groundedPlayer = (transform.position.y < 6);
        groundedPlayer = controller.isGrounded;
        // if (groundedPlayer && playerVelocity.y < 0)
        // {
        //     playerVelocity.y = 0f;
        // }
        // If B pressed, jump
        if ((Input.GetKeyDown(KeyCode.B) || jumping) && groundedPlayer)
        {
            playerVelocity.y = jumpVelocity;
        }

        playerVelocity.y += gravityValue * Time.deltaTime;
        if (transform.position.y > 100) {
            playerVelocity.y = gravityValue * speedUp;
        }
        if (gameScript.gameActive) {
            controller.Move(move * Time.deltaTime);
            controller.Move(playerVelocity * Time.deltaTime);
        }

        peopleCarriedW = gameScript.peopleCarriedW;
        peopleCarriedA = gameScript.peopleCarriedA;
        peopleCarriedS = gameScript.peopleCarriedS;
        peopleCarriedD = gameScript.peopleCarriedD;

        for (int i = 0; i < passengers.Length; i++) {
            if (i < peopleCarriedW) {
                passengers[i].SetActive(true);
                var individualPassenger = passengers[i].GetComponent<Love_Truck_Passenger>();
                individualPassenger.type = 'W';
                individualPassenger.dance = gameScript.danceW;
            } else if (i < peopleCarriedW + peopleCarriedA) {
                passengers[i].SetActive(true);
                var individualPassenger = passengers[i].GetComponent<Love_Truck_Passenger>();
                individualPassenger.type = 'A';
                individualPassenger.dance = gameScript.danceA;
            } else if (i < peopleCarriedW + peopleCarriedA + peopleCarriedS) {
                passengers[i].SetActive(true);
                var individualPassenger = passengers[i].GetComponent<Love_Truck_Passenger>();
                individualPassenger.type = 'S';
                individualPassenger.dance = gameScript.danceS;
            } else if (i < peopleCarriedW + peopleCarriedA + peopleCarriedS + peopleCarriedD) {
                passengers[i].SetActive(true);
                var individualPassenger = passengers[i].GetComponent<Love_Truck_Passenger>();
                individualPassenger.type = 'D';
                individualPassenger.dance = gameScript.danceD;
            } else {
                passengers[i].SetActive(false);
            }
        }

    }

    public void addCollision() {
        gameScript.addCollision();
    }

    public void hit() {
        StartCoroutine(iframe());
    }

    public IEnumerator iframe()
    { 
        isInvincible = true;
        orangeGlow.SetActive(true);
        // for (int i = 1; i <= 20; i++) {
        //     orangeGlow.transform.localScale = new Vector3(i, i, i);
        //     yield return new WaitForSeconds(0.05f);
        // }
        float iframeLength = 0.5f; // Duration to move the building (you can adjust this)
        Vector3 startPosition = new Vector3(0, 0, 0);
        Vector3 targetPosition = new Vector3(20, 20, 20);
        float elapsedTime = 0f;

        while (elapsedTime < iframeLength)
        {
            orangeGlow.transform.localScale = Vector3.Lerp(startPosition, targetPosition, (elapsedTime / iframeLength));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        isInvincible = false;
        orangeGlow.SetActive(false);
        yield return null;
    }

    // void OnControllerColliderHit(ControllerColliderHit hit) {
    //     if (hit.rigidbody != null) {
    //         Vector3 horizontalDir = new Vector3(hit.moveDirection.x, 0, hit.moveDirection.z);
    //         hit.rigidbody.AddForce(horizontalDir * 1000);
    //     }
    // }
}