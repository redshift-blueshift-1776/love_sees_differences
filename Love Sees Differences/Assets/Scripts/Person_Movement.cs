using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Person_Movement : MonoBehaviour
{
    [SerializeField] public float speed;

    [SerializeField] public GameObject player;

    [SerializeField] public GameObject game;

    public Vector3 startingPos;
    // Start is called before the first frame update
    void Start()
    {
        
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
        if (c.name == "Truck_Thing") {
            // collision
            game.addCollision();
        }
    }
}
