using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Person_Movement : MonoBehaviour
{
    [SerializeField] public float speed;

    [SerializeField] public GameObject player;

    private Player_Movement playerMovement;

    [SerializeField] public GameObject game;

    private Game gameScript;

    public Vector3 startingPos;
    // Start is called before the first frame update
    void Start()
    {
        playerMovement = player.GetComponent<Player_Movement>();
        gameScript = game.GetComponent<Game>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(startingPos * Time.deltaTime * speed);
        StartCoroutine(waitAndDespawn());
    }

    private IEnumerator waitAndDespawn() {
        yield return new WaitForSeconds(10f);
        Destroy(gameObject);
        yield return null;
    }

    private void OnTriggerEnter(Collider c) {
        Debug.Log("Hit Something!");
        Debug.Log(c.name);
        if (c.name == "Truck_Thing") {
            // collision
            gameScript.addCollision();
            Destroy(gameObject);
            return;
        }
    }
}
