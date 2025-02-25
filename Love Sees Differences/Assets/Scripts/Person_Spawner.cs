using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Person_Spawner : MonoBehaviour
{
    [SerializeField] private float xSpeed = 1.0f;
    [SerializeField] private float zSpeed = 1.0f;

    [SerializeField] public float spawnInterval = 5f;

    [SerializeField] public GameObject game;

    private Game gameScript;

    private GameObject newPerson;

    private Vector3 direction;

    [SerializeField] private GameObject person;
    // Start is called before the first frame update
    void Start()
    {
        direction = new Vector3(xSpeed, 0, zSpeed);
        gameScript = game.GetComponent<Game>();
        StartCoroutine(RegeneratePeople());
    }

    // Update is called once per frame
    void Update()
    {

    }

    void spawnPerson(Vector3 size, Vector3 walkDirection, float speed) {

        newPerson = Instantiate(person, transform.position, transform.rotation);
        newPerson.transform.localScale = size;
        //newPerson.GetComponent<Person_Movement>().setSource(transform);
        newPerson.GetComponent<Person_Movement>().startingPos = walkDirection;
        newPerson.GetComponent<Person_Movement>().speed = speed;
    }

    private IEnumerator RegeneratePeople()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);
            if (gameScript.gameActive)
            {
                Vector3 vec = new Vector3(1, 1, 1);
                spawnPerson(vec, direction, 2);
            }

            
        }
    }
}
