using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Person_NFS : MonoBehaviour
{
    [SerializeField] public float speed = 20f;
    [SerializeField] public float despawnRadius = 20f; // Distance at which pedestrian despawns

    [SerializeField] public GameObject player;
    private Player_Movement playerMovement;

    [SerializeField] public GameObject game;
    [SerializeField] public AudioSource sound;

    private Game gameScript;
    private Screen_Tint screenTint;


    [Header("Goal Points")]
    [SerializeField] public Transform[] goalPoints;  // Assign in Unity Inspector

    public Vector3 endGoal;

    private Vector3 lastPosition;
    private float stuckTime = 0f;

    void Start()
    {
        player = GameObject.Find("Truck_Thing");
        playerMovement = player.GetComponent<Player_Movement>();
        game = GameObject.Find("Game");
        gameScript = game.GetComponent<Game>();
        screenTint = game.GetComponent<Screen_Tint>();
        endGoal = goalPoints[Random.Range(0, goalPoints.Length)].position;
    }

    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, endGoal, speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, endGoal) <= despawnRadius)
        {
            Destroy(gameObject); // Despawn pedestrian
        }
        if (Vector3.Distance(transform.position, lastPosition) < 0.1f)
        {
            stuckTime += Time.deltaTime;
            if (stuckTime > 1f)
            {
                Debug.Log("Pedestrian is stuck, forcing movement!");
                Vector3 goABit = new Vector3(5, 0, 5);
                transform.Translate(goABit * Time.deltaTime * Random.Range(-2, 2));
                stuckTime = 0;
            }
        }
        else
        {
            stuckTime = 0;
        }

        lastPosition = transform.position;
    }

    private void OnTriggerEnter(Collider c)
    {
        //Debug.Log(c.name);
        if (c.name == "Truck_Thing")
        {
            sound.Play();
            Die();
            return;
        }
    }

    void Die()
    {
        StartCoroutine(DieCoroutine());
    }

    private IEnumerator DieCoroutine()
    {
        screenTint.TintAndFade();
        gameScript.addCollision();
        yield return new WaitForSeconds(0.5f);
        Destroy(gameObject);
        yield return null;
    }
}
