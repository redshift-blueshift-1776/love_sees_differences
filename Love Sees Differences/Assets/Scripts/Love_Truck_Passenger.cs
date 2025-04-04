using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Love_Truck_Passenger : MonoBehaviour
{

    [SerializeField] public GameObject game;

    public Game gameScript;

    public AudioSource audioSource;

    public char type; // char for which building they go to

    [SerializeField] public Color wColor;
    [SerializeField] public Color aColor;
    [SerializeField] public Color sColor;
    [SerializeField] public Color dColor;

    [SerializeField] public GameObject torso;

    // Start is called before the first frame update
    void Start()
    {
        game = GameObject.Find("Game");
        gameScript = game.GetComponent<Game>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
