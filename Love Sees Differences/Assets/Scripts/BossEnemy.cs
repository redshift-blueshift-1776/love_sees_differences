using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum EnemyState
{
    Wander,
    Follow,
    Die,
    Attack1,
    Attack2,
    Attack3
};

public class BossEnemy : MonoBehaviour
{
    [SerializeField] GameObject weapon;
    [SerializeField] int enemyType;
    // 0: pepperoni
    // 1: mushroom

    public EnemyState currState;
    public float range;
    public float moveSpeed;

    Rigidbody myRigidbody;

    public int HP;
    public int maxHP;
    Color og;
    Color transparent;

    public Transform player;

    private bool gotHit = false;
    GameObject newBullet;

    public int phase;

    [SerializeField] public GameObject Game;

    public Game_Boss gameScript;

    private Game_Boss game;

    public int xBound;
    bool direction;

    public bool isInvincible = false;

    bool startedPhase2 = false;
    bool startedPhase3 = false;
    bool startedPhase4 = false;

    [SerializeField] GameObject orangePassenger;

    [SerializeField] GameObject sceneSwitcher;

    // Start is called before the first frame update
    void Start()
    {
        //phase = 1;
        //maxHP = 4000;
        //HP = 4000;
        //moveSpeed = 5;
        //currState = EnemyState.Attack1;
        HP = 100;
        maxHP = 100;
        og = GetComponent<Renderer>().material.color;
        player = GameObject.Find("Player").transform;
        myRigidbody = GetComponent<Rigidbody>();
        transparent = new Color(og.r, og.g, og.b, 0.5f);
    }

    // Update is called once per frame
    void Update()
    {
        switch (currState)
        {
            case (EnemyState.Attack1):
                Attack1();
                break;
            case (EnemyState.Attack2):
                Attack2();
                break;
            case (EnemyState.Die):
                Die();
                break;
        }    

        if (HP <= 0) {
            currState = EnemyState.Die;
        }

        if ((float)HP / (float)maxHP <= 0.75f && !startedPhase2) {
            phase = 2;
            Debug.Log("phase 2 entered");
            StartCoroutine(changeAttackPhase());
            startedPhase2 = true;
        }

        if ((float)HP / (float)maxHP <= 0.50f && !startedPhase3) {
            phase = 3;
            Debug.Log("phase 3 entered");
            startedPhase3 = true;
        }

        if ((float)HP / (float)maxHP <= 0.25f && !startedPhase4) {
            phase = 4;
            Debug.Log("phase 4 entered");
            startedPhase4 = true;
        }

        if (phase == 2) {
            
        }

        if (gotHit) {
            
        }
    }

    private void OnTriggerEnter(Collider c)
    {
        if (c.name.Contains("Arrow") && currState != EnemyState.Die && !isInvincible) {
            StartCoroutine(Hit());
            HP -= 1;
        } else if (c.name == "Truck_Thing_Boss") {
            if (!player.GetComponent<Player_Movement_Boss>().isInvincible) {
                gameScript.addCollision();
            }
        }
    }

    IEnumerator Hit()
    { 
        gotHit = true;
        GetComponent<Renderer>().material.color = Color.red;
        yield return new WaitForSeconds(.1f);
        GetComponent<Renderer>().material.color = og;
        gotHit = false;
        yield return null;
    }

    void changePhase(int i) {
        phase = i;
    }

    private IEnumerator changeAttackPhase()
    {
        isInvincible = true;
        // Will do a cutscene?
        yield return new WaitForSeconds(.5f);
        isInvincible = false;
        yield return null;
    }

    private bool IsPlayerInRange(float range)
    {
        return Vector3.Distance(transform.position, player.transform.position) <= range;
    }

    void Attack1()
    {
        if (phase == 1) {
            
        } 

        if (phase == 2) {
            
        }

        if (phase == 3) {
            
        }

        if (phase == 4) {
            
        }

        // Change attack
    }

    void changeAttack (int i) {
        switch (i)
        {
            case (1):
                currState = EnemyState.Attack1;
                break;
            case (2):
                currState = EnemyState.Attack2;
                break;
        }    
    }

    void spawnOrangePassenger(Vector3 size, Vector3 passengerDirection, float speed) {
        
    }

    void Attack2()
    {
        if (phase == 1) {
            
        }

        if (phase == 2) {

        }

        if (phase == 3) {
            
        }

        if (phase == 4) {
            
        }

        // Change attack
    }

    void Die() {
        StartCoroutine(DieCoroutine());
    }

    private IEnumerator DieCoroutine()
    {
        Debug.Log("test");
        player.GetComponent<Player_Movement_Boss>().isInvincible = true;
        for (int i = 0; i < 5; i++) {
            GetComponent<Renderer>().material.color = transparent;
            yield return new WaitForSeconds(0.1f);
            GetComponent<Renderer>().material.color = og;
            yield return new WaitForSeconds(0.1f);
        }
        GetComponent<Renderer>().material.color = Color.red;
        yield return new WaitForSeconds(3f);
        //SceneManager.LoadScene(2); //Replace with actual scene when we make it
        Destroy(gameObject);
        yield return null;
    }


}

