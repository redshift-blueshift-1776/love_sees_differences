using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Person_Spawner_NFS : MonoBehaviour
{
    // [SerializeField] private float xSpeed = 1.0f;
    // [SerializeField] private float zSpeed = 1.0f;

    [SerializeField] public float speed = 20f;
    [SerializeField] private float despawnRadius = 20f; // Distance at which pedestrian despawns

    [SerializeField] public GameObject player;
    private Player_Movement playerMovement;

    [SerializeField] public GameObject game;
    //[SerializeField] public AudioSource sound;

    private Game gameScript;


    [Header("Goal Points")]
    [SerializeField] private Transform[] goalPoints;  // Assign in Unity Inspector

    [Header("Map Settings")]
    [SerializeField] private float topLeftX = -100f;  // Distance between grid points in Unity world units
    [SerializeField] private float topLeftZ = 100f;  // Distance between grid points in Unity world units
    [SerializeField] private float bottomRightX = -100f;  // Distance between grid points in Unity world units
    [SerializeField] private float bottomRightZ = 100f;  // Distance between grid points in Unity world units

    [SerializeField] public float spawnInterval = 5f;

    private GameObject newPerson;

    private Vector3 direction;

    [SerializeField] private GameObject person;
    // Start is called before the first frame update
    void Start()
    {
        //direction = new Vector3(xSpeed, 0, zSpeed);
        gameScript = game.GetComponent<Game>();
        StartCoroutine(RegeneratePeople());
    }

    // Update is called once per frame
    void Update()
    {

    }

    void spawnPerson(Vector3 size, Vector3 walkDirection, float speed) {
        Vector3 spawnLocation = new Vector3(Random.Range(topLeftX, bottomRightX), 0, Random.Range(bottomRightZ, topLeftZ));
        newPerson = Instantiate(person, spawnLocation, transform.rotation);
        newPerson.SetActive(true);  // Ensure it is active

        newPerson.transform.localScale = size;
        newPerson.GetComponent<Person_NFS>().speed = speed;
        newPerson.GetComponent<Person_NFS>().despawnRadius = despawnRadius;
        newPerson.GetComponent<Person_NFS>().goalPoints = goalPoints;
        
    }

    private IEnumerator RegeneratePeople()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);
            if (gameScript.gameActive)
            {
                Vector3 vec = new Vector3(1, 1, 1);
                spawnPerson(vec, direction, speed);
            }

            
        }
    }
}
