using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class Game : MonoBehaviour
{
    [SerializeField] public GameObject player;

    public bool gameActive;

    private float timer;
    [SerializeField] private int levelLengthInSeconds;

    [SerializeField] public GameObject BuildingW;
    [SerializeField] public GameObject BuildingA;
    [SerializeField] public GameObject BuildingS;
    [SerializeField] public GameObject BuildingD;

    [SerializeField] public GameObject GeneratorCanvas;
    [SerializeField] public GameObject UICanvas;

    [SerializeField] public GameObject loadingAudio;
    [SerializeField] public GameObject gameAudio;

    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI peopleAtWText;
    [SerializeField] private TextMeshProUGUI peopleAtAText;
    [SerializeField] private TextMeshProUGUI peopleAtSText;
    [SerializeField] private TextMeshProUGUI peopleAtDText;
    [SerializeField] private TextMeshProUGUI peopleCarriedText;
    [SerializeField] private Toggle polarWToggle;
    [SerializeField] private Toggle polarAToggle;
    [SerializeField] private Toggle polarSToggle;
    [SerializeField] private Toggle polarDToggle;
    [SerializeField] private Toggle selfPolarToggle;

    private int deliveries;
    private int collisions;

    private int peopleAtW;
    private int peopleAtA;
    private int peopleAtS;
    private int peopleAtD;

    private int peopleCarried;
    private int peopleCarriedW;
    private int peopleCarriedA;
    private int peopleCarriedS;
    private int peopleCarriedD;

    private Player_Movement playerMovement;

    private const int maxCarryCapacity = 10;
    private const float pickupRadius = 30.0f;

    private const int maxPeopleAtBuilding = 20; // Maximum people a building can hold
    private const int regenerationAmount = 2;  // How many people regenerate each cycle
    private const float regenerationInterval = 5f; // Time in seconds between regenerations
    // Start is called before the first frame update
    void Start()
    {
        gameActive = false;
        timer = 0;
        
        peopleAtW = 5;
        peopleAtA = 5;
        peopleAtS = 5;
        peopleAtD = 5;
        
        peopleCarriedW = 0;
        peopleCarriedA = 0;
        peopleCarriedS = 0;
        peopleCarriedD = 0;

        // GeneratorCanvas should be enabled, and UI canvas should be disabled.
        GeneratorCanvas.SetActive(true);
        UICanvas.SetActive(false);

        // Play loadingAudio
        loadingAudio.SetActive(true);
        gameAudio.SetActive(false);

        playerMovement = player.GetComponent<Player_Movement>();

        SetupUIToggles();

        // Start the people regeneration coroutine
        StartCoroutine(RegeneratePeople());
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!gameActive)
        {
            FreezePlayer();
            return;
        }

        // When gameActive is true, start the timer and let it go for levelLengthInSeconds seconds.
        timer += Time.deltaTime;
        if (timer >= levelLengthInSeconds)
        {
            EndGame();
        }

        HandleBuildingInteraction();

        UpdateScore();
        UpdateUIToggles();
    }

    public void startGame() {
        // Generator Canvas will call this when the start game button is pressed.
        gameActive = true;
        timer = 0;
        UnfreezePlayer();

        // Also, hide the GeneratorCanvas and show the UI canvas.
        GeneratorCanvas.SetActive(false);
        UICanvas.SetActive(true);

        // Also, stop loadingAudio and play gameAudio.
        loadingAudio.SetActive(false);
        gameAudio.SetActive(true);
    }

    private void FreezePlayer()
    {
        // While gameActive is false, freeze the player's movements
        player.GetComponent<CharacterController>().enabled = false;
    }

    private void UnfreezePlayer()
    {
        player.GetComponent<CharacterController>().enabled = true;
    }

    private void HandleBuildingInteraction()
    {
        // If player gets within a certain radius of a building, they will pick up as many people
        // as they can, with the maximum they can carry at a time being 10.
        float distanceW = Vector3.Distance(player.transform.position, BuildingW.transform.position);
        float distanceA = Vector3.Distance(player.transform.position, BuildingA.transform.position);
        float distanceS = Vector3.Distance(player.transform.position, BuildingS.transform.position);
        float distanceD = Vector3.Distance(player.transform.position, BuildingD.transform.position);

        if (distanceW < pickupRadius) HandlePickupAndDropoff(ref peopleAtW, ref peopleCarriedS, ref peopleCarriedW);
        if (distanceA < pickupRadius) HandlePickupAndDropoff(ref peopleAtA, ref peopleCarriedD, ref peopleCarriedA);
        if (distanceS < pickupRadius) HandlePickupAndDropoff(ref peopleAtS, ref peopleCarriedW, ref peopleCarriedS);
        if (distanceD < pickupRadius) HandlePickupAndDropoff(ref peopleAtD, ref peopleCarriedA, ref peopleCarriedD);

        peopleCarried = peopleCarriedW + peopleCarriedA + peopleCarriedS + peopleCarriedD;
    }

    private void HandlePickupAndDropoff(ref int peopleAtBuilding, ref int peopleToDrop, ref int peopleToPickup)
    {
        // Additionally, if the player goes to building W, they will drop off everyone from S.
        // Same for S and W.
        // Same for A and D.
        // Same for D and A.
        // For every person dropped off, add 1 to deliveries.
        deliveries += peopleToDrop;
        peopleToDrop = 0;

        int availableSpace = maxCarryCapacity - peopleCarried;
        int peopleToTake = Mathf.Min(peopleAtBuilding, availableSpace);

        peopleToPickup += peopleToTake;
        peopleAtBuilding -= peopleToTake;
    }

    public void addCollision()
    {
        collisions++;
    }

    private void UpdateScore()
    {
        // Score = deliveries - collisions. Display on the UI.
        timerText.text = $"Time: {Mathf.Max(0, levelLengthInSeconds - (int)timer)}";
        int score = deliveries - collisions;
        scoreText.text = $"Score: {score}";
        peopleAtWText.text = $"People at W: {peopleAtW}";
        peopleAtAText.text = $"People at A: {peopleAtA}";
        peopleAtSText.text = $"People at S: {peopleAtS}";
        peopleAtDText.text = $"People at D: {peopleAtD}";
        peopleCarriedText.text = $"Carried: {peopleCarried}/{maxCarryCapacity}";
    }

    private void SetupUIToggles()
    {
        polarWToggle.onValueChanged.AddListener((value) => playerMovement.polarW = value);
        polarAToggle.onValueChanged.AddListener((value) => playerMovement.polarA = value);
        polarSToggle.onValueChanged.AddListener((value) => playerMovement.polarS = value);
        polarDToggle.onValueChanged.AddListener((value) => playerMovement.polarD = value);
        selfPolarToggle.onValueChanged.AddListener((value) => playerMovement.selfPolar = value);
    }

    private void UpdateUIToggles()
    {
        // Also, the UI has toggles for variables in Player_Movement:
        // public bool polarW;
        // public bool polarA;
        // public bool polarS;
        // public bool polarD;
        // public bool selfPolar;
        // Adjust these if clicked.
        polarWToggle.isOn = playerMovement.polarW;
        polarAToggle.isOn = playerMovement.polarA;
        polarSToggle.isOn = playerMovement.polarS;
        polarDToggle.isOn = playerMovement.polarD;
        selfPolarToggle.isOn = playerMovement.selfPolar;
    }

    private void EndGame()
    {
        gameActive = false;
        FreezePlayer();
        Debug.Log("Game Over! Final Score: " + (deliveries - collisions));
    }

    private IEnumerator RegeneratePeople()
    {
        while (true)
        {
            yield return new WaitForSeconds(regenerationInterval);

            if (gameActive)
            {
                peopleAtW = Mathf.Min(peopleAtW + regenerationAmount, maxPeopleAtBuilding);
                peopleAtA = Mathf.Min(peopleAtA + regenerationAmount, maxPeopleAtBuilding);
                peopleAtS = Mathf.Min(peopleAtS + regenerationAmount, maxPeopleAtBuilding);
                peopleAtD = Mathf.Min(peopleAtD + regenerationAmount, maxPeopleAtBuilding);
                
                UpdateScore();
            }
        }
    }
}
