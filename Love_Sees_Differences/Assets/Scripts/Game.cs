using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class Game : MonoBehaviour
{
    [SerializeField] public GameObject player;

    public bool gameActive;

    public float timer;
    [SerializeField] private int levelLengthInSeconds;

    [Header("Buildings")]
    [SerializeField] public GameObject BuildingW;
    [SerializeField] public GameObject BuildingA;
    [SerializeField] public GameObject BuildingS;
    [SerializeField] public GameObject BuildingD;

    [Header("Canvasses")]
    [SerializeField] public GameObject GeneratorCanvas;
    [SerializeField] public GameObject ScreenTintCanvas;
    [SerializeField] public GameObject UICanvas;
    [SerializeField] public GameObject UICanvasNew;
    [SerializeField] public GameObject EndScreenCanvas;

    [Header("Audio")]
    [SerializeField] public GameObject loadingAudio;
    [SerializeField] public GameObject gameAudio;
    [SerializeField] private AudioSource deliverSound;

    [Header("UI Canvas Old")]
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

    [Header("UI Canvas New")]
    [SerializeField] private TextMeshProUGUI scoreTextNew;
    [SerializeField] private TextMeshProUGUI timerTextNew;

    [SerializeField] private TextMeshProUGUI peopleAtWTextNew;
    [SerializeField] private Transform peopleAtWParent;
    [SerializeField] private GameObject[] peopleAtWImages;

    [SerializeField] private TextMeshProUGUI peopleAtATextNew;
    [SerializeField] private Transform peopleAtAParent;
    [SerializeField] private GameObject[] peopleAtAImages;

    [SerializeField] private TextMeshProUGUI peopleAtSTextNew;
    [SerializeField] private Transform peopleAtSParent;
    [SerializeField] private GameObject[] peopleAtSImages;

    [SerializeField] private TextMeshProUGUI peopleAtDTextNew;
    [SerializeField] private Transform peopleAtDParent;
    [SerializeField] private GameObject[] peopleAtDImages;

    [SerializeField] private TextMeshProUGUI peopleCarriedTextNew;
    [SerializeField] private Transform peopleCarriedParent;
    [SerializeField] private GameObject[] peopleCarriedImages;

    [SerializeField] private Toggle polarWToggleNew;
    [SerializeField] private Toggle polarAToggleNew;
    [SerializeField] private Toggle polarSToggleNew;
    [SerializeField] private Toggle polarDToggleNew;
    [SerializeField] private Toggle selfPolarToggleNew;
    [SerializeField] private Button boostButton;
    [SerializeField] private Button jumpButton;

    [SerializeField] public Texture W_to_S;
    [SerializeField] public Texture A_to_D;
    [SerializeField] public Texture S_to_W;
    [SerializeField] public Texture D_to_A;

    [Header("End Screen Canvas")]
    [SerializeField] private TextMeshProUGUI finalScoreText;
    [SerializeField] private TextMeshProUGUI finalDeliveriesText;
    [SerializeField] private TextMeshProUGUI finalCollisionsText;

    private int deliveries;
    private int collisions;

    [SerializeField] private int peopleAtW;
    [SerializeField] private int peopleAtA;
    [SerializeField] private int peopleAtS;
    [SerializeField] private int peopleAtD;

    [SerializeField] public int peopleCarried;
    [SerializeField] public int peopleCarriedW;
    [SerializeField] public int peopleCarriedA;
    [SerializeField] public int peopleCarriedS;
    [SerializeField] public int peopleCarriedD;

    private Player_Movement playerMovement;

    [SerializeField] public string levelName;

    public int maxCarryCapacity = 10;
    private const float pickupRadius = 30.0f;

    private const int maxPeopleAtBuilding = 10; // Maximum people a building can hold
    private const int regenerationAmount = 1;  // How many people regenerate each cycle
    
    [SerializeField] private float regenerationInterval = 3f; // Time in seconds between regenerations

    [SerializeField] public float tempo = 120f; // Tempo of the song used

    [SerializeField] public Love_Truck_Passenger.Dance danceW;
    [SerializeField] public Love_Truck_Passenger.Dance danceA;
    [SerializeField] public Love_Truck_Passenger.Dance danceS;
    [SerializeField] public Love_Truck_Passenger.Dance danceD;

    private float secondsPerBeat;

    //[SerializeField] public bool OldUIEnabled = false;
    public bool OldUIEnabled;
    // Start is called before the first frame update
    void Start()
    {
        //boostButton.onClick.RemoveAllListeners(); 
        ClearCollisionData();
        OldUIEnabled = PlayerPrefs.GetInt("UseOldUI", 1) == 1;
        gameActive = false;
        timer = 0;
        secondsPerBeat = 60f / tempo;
        
        peopleAtW = Random.Range(1, 5);
        peopleAtA = Random.Range(1, 5);
        peopleAtS = Random.Range(1, 5);
        peopleAtD = Random.Range(1, 5);
        
        peopleCarriedW = 0;
        peopleCarriedA = 0;
        peopleCarriedS = 0;
        peopleCarriedD = 0;

        peopleAtWImages = GetChildImages(peopleAtWParent);
        peopleAtAImages = GetChildImages(peopleAtAParent);
        peopleAtSImages = GetChildImages(peopleAtSParent);
        peopleAtDImages = GetChildImages(peopleAtDParent);
        peopleCarriedImages = GetChildRawImages(peopleCarriedParent);

        // GeneratorCanvas should be enabled, and UI canvas should be disabled.
        GeneratorCanvas.SetActive(true);
        ScreenTintCanvas.SetActive(false);
        UICanvas.SetActive(false);
        UICanvasNew.SetActive(false);
        EndScreenCanvas.SetActive(false);

        // Play loadingAudio
        loadingAudio.SetActive(true);
        gameAudio.SetActive(false);

        playerMovement = player.GetComponent<Player_Movement>();

        SetupUIToggles();

        // Start the people regeneration coroutine
        StartCoroutine(RegeneratePeople());
        
    }

    private GameObject[] GetChildImages(Transform parent)
    {
        if (parent == null)
        {
            Debug.LogError("Parent transform is not assigned!");
            return new GameObject[0];
        }

        // Find all images under the specified parent
        Image[] images = parent.GetComponentsInChildren<Image>();

        // Convert Image components to GameObjects
        GameObject[] imageObjects = new GameObject[images.Length];
        for (int i = 0; i < images.Length; i++)
        {
            imageObjects[i] = images[i].gameObject;
        }

        return imageObjects;
    }

    private GameObject[] GetChildRawImages(Transform parent)
    {
        if (parent == null)
        {
            Debug.LogError("Parent transform is not assigned!");
            return new GameObject[0];
        }

        // Find all images under the specified parent
        RawImage[] images = parent.GetComponentsInChildren<RawImage>();

        // Convert Image components to GameObjects
        GameObject[] imageObjects = new GameObject[images.Length];
        for (int i = 0; i < images.Length; i++)
        {
            imageObjects[i] = images[i].gameObject;
        }

        return imageObjects;
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
        ScreenTintCanvas.SetActive(true);
        

        if (OldUIEnabled) {
            UICanvas.SetActive(true);
            UICanvasNew.SetActive(false);
        } else {
            UICanvas.SetActive(false);
            UICanvasNew.SetActive(true);
        }

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
        // Additionally, if the player goes to building W, they will drop off everyone from S.
        // Same for S and W.
        // Same for A and D.
        // Same for D and A.
        // For every person dropped off, add 1 to deliveries.
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
        // If player gets within a certain radius of a building, they will pick up as many people
        // as they can, with the maximum they can carry at a time being 10.
        if (peopleToDrop > 0) {
            deliverSound.Play();
        }
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
        if (OldUIEnabled) {
            // Score = deliveries - collisions. Display on the UI.
            timerText.text = $"Time: {Mathf.Max(0, levelLengthInSeconds - (int)timer)}";
            int score = deliveries - collisions;
            scoreText.text = $"Score: {score}";
            peopleAtWText.text = $"People at W: {peopleAtW}";
            peopleAtAText.text = $"People at A: {peopleAtA}";
            peopleAtSText.text = $"People at S: {peopleAtS}";
            peopleAtDText.text = $"People at D: {peopleAtD}";
            peopleCarriedText.text = $"Carried: {peopleCarried}/{maxCarryCapacity}";
        } else {
            timerTextNew.text = $"Time: {Mathf.Max(0, levelLengthInSeconds - (int)timer)}";
            int score = deliveries - collisions;
            scoreTextNew.text = $"Score: {score}";

            //peopleCarriedTextNew.text = $"Carried: {peopleCarried}/{maxCarryCapacity}";

            for (int i = 0; i < maxPeopleAtBuilding; i++) {
                peopleAtWImages[i].SetActive(i < peopleAtW);
                peopleAtAImages[i].SetActive(i < peopleAtA);
                peopleAtSImages[i].SetActive(i < peopleAtS);
                peopleAtDImages[i].SetActive(i < peopleAtD);
            }

            for (int i = 0; i < maxCarryCapacity; i++) {
                if (i < peopleCarriedW) {
                    peopleCarriedImages[i].SetActive(true);
                    peopleCarriedImages[i].GetComponent<RawImage>().texture = W_to_S;
                } else if (i < peopleCarriedW + peopleCarriedA) {
                    peopleCarriedImages[i].SetActive(true);
                    peopleCarriedImages[i].GetComponent<RawImage>().texture = A_to_D;
                } else if (i < peopleCarriedW + peopleCarriedA + peopleCarriedS) {
                    peopleCarriedImages[i].SetActive(true);
                    peopleCarriedImages[i].GetComponent<RawImage>().texture = S_to_W;
                } else if (i < peopleCarriedW + peopleCarriedA + peopleCarriedS + peopleCarriedD) {
                    peopleCarriedImages[i].SetActive(true);
                    peopleCarriedImages[i].GetComponent<RawImage>().texture = D_to_A;
                } else {
                    peopleCarriedImages[i].SetActive(false);
                }
            }

        }
    }

    private void SetupUIToggles()
    {
        if (OldUIEnabled) {
            polarWToggle.onValueChanged.AddListener((value) => playerMovement.polarW = value);
            polarAToggle.onValueChanged.AddListener((value) => playerMovement.polarA = value);
            polarSToggle.onValueChanged.AddListener((value) => playerMovement.polarS = value);
            polarDToggle.onValueChanged.AddListener((value) => playerMovement.polarD = value);
            selfPolarToggle.onValueChanged.AddListener((value) => playerMovement.selfPolar = value);
        } else {
            polarWToggleNew.onValueChanged.AddListener((value) => playerMovement.polarW = value);
            polarAToggleNew.onValueChanged.AddListener((value) => playerMovement.polarA = value);
            polarSToggleNew.onValueChanged.AddListener((value) => playerMovement.polarS = value);
            polarDToggleNew.onValueChanged.AddListener((value) => playerMovement.polarD = value);
            selfPolarToggleNew.onValueChanged.AddListener((value) => playerMovement.selfPolar = value);
            //boostButton.onClick.AddListener((value) => playerMovement.boosted = value);
        }
        
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
        if (OldUIEnabled) {
            polarWToggle.isOn = playerMovement.polarW;
            polarAToggle.isOn = playerMovement.polarA;
            polarSToggle.isOn = playerMovement.polarS;
            polarDToggle.isOn = playerMovement.polarD;
            selfPolarToggle.isOn = playerMovement.selfPolar;
        } else {
            polarWToggleNew.isOn = playerMovement.polarW;
            polarAToggleNew.isOn = playerMovement.polarA;
            polarSToggleNew.isOn = playerMovement.polarS;
            polarDToggleNew.isOn = playerMovement.polarD;
            selfPolarToggleNew.isOn = playerMovement.selfPolar;
        }
    }

    private void EndGame()
    {
        gameActive = false;
        FreezePlayer();
        Debug.Log("Game Over! Final Score: " + (deliveries - collisions));
        ScreenTintCanvas.SetActive(false);
        UICanvas.SetActive(false);
        UICanvasNew.SetActive(false);
        EndScreenCanvas.SetActive(true);
        int score = deliveries - collisions;
        finalScoreText.text = $"Score: {score}";
        finalDeliveriesText.text = $"Deliveries: {deliveries}";
        finalCollisionsText.text = $"Collisions: {collisions}";
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

    private void ClearCollisionData()
    {
        string collisionKeyPrefix = "Collision_" + levelName + "_";

        // Clear specific collision data for the day mode level
        PlayerPrefs.DeleteKey(collisionKeyPrefix + "Count"); // Deletes the collision count
        // Optionally, clear all collision data related to the specific level
        int collisionCount = PlayerPrefs.GetInt(collisionKeyPrefix + "Count", 0);
        for (int i = 0; i < collisionCount; i++)
        {
            PlayerPrefs.DeleteKey(collisionKeyPrefix + "Time_" + i);
            PlayerPrefs.DeleteKey(collisionKeyPrefix + "PosX_" + i);
            PlayerPrefs.DeleteKey(collisionKeyPrefix + "PosY_" + i);
            PlayerPrefs.DeleteKey(collisionKeyPrefix + "PosZ_" + i);
        }
        
        PlayerPrefs.Save();  // Ensure that data is deleted immediately
    }

}
