using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Game_2 : MonoBehaviour
{
    [Header("Level")]
    [SerializeField] public GameObject player;
    [SerializeField] public GameObject destination;
    // Buildings and their assigned passengers
    [SerializeField] private GameObject buildingW;
    [SerializeField] private GameObject buildingA;
    [SerializeField] private GameObject buildingS;
    [SerializeField] private GameObject buildingD;

    public bool gameActive;
    public float timer;
    [SerializeField] private int levelLengthInSeconds;

    [Header("Canvasses")]
    [SerializeField] public GameObject GeneratorCanvas;
    [SerializeField] public GameObject UICanvas;
    [SerializeField] public GameObject EndScreenCanvas;

    [Header("Audio")]
    [SerializeField] public GameObject loadingAudio;
    [SerializeField] public GameObject gameAudio;
    [SerializeField] private AudioSource deliverSound;
    [SerializeField] private AudioSource spawnSound;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI peopleAtWText;
    [SerializeField] private TextMeshProUGUI peopleAtAText;
    [SerializeField] private TextMeshProUGUI peopleAtSText;
    [SerializeField] private TextMeshProUGUI peopleAtDText;
    [SerializeField] private TextMeshProUGUI peopleCarriedText;
    [SerializeField] private TextMeshProUGUI finalScoreText;
    [SerializeField] private TextMeshProUGUI finalDeliveriesText;
    [SerializeField] private Image fuelBarFill;

    [SerializeField] private RectTransform needleTransform;  // Assign in Inspector
    [SerializeField] private float maxSpeedDisplayed = 151f;         // Maximum speed displayed
    private float minAngle = -135f;  // Leftmost angle
    private float maxAngle = 135f;   // Rightmost angle

    [SerializeField] private GameObject screenTint;

    private Screen_Tint screenTintScript;

    private int deliveries;
    private int peopleCarried;

    private const float pickupRadius = 20.0f;
    private const float buildingPickupRadius = 30.0f;
    private const float dropoffRadius = 50.0f;
    private List<GameObject> passengers = new List<GameObject>();

    private int peopleAtW;
    private int peopleAtA;
    private int peopleAtS;
    private int peopleAtD;

    private float originalFuelBarWidth = 500f;

    private const float regenerationInterval = 5f; // Time in seconds between regenerations

    void Start()
    {
        gameActive = false;
        timer = 0;
        deliveries = 0;
        peopleCarried = 0;

        GeneratorCanvas.SetActive(true);
        UICanvas.SetActive(false);
        EndScreenCanvas.SetActive(false);

        loadingAudio.SetActive(true);
        gameAudio.SetActive(false);

        screenTintScript = screenTint.GetComponent<Screen_Tint>();

        // Initialize passenger counts at buildings
        peopleAtW = Random.Range(1, 3); // Adjust these numbers as needed
        peopleAtA = Random.Range(1, 3);
        peopleAtS = Random.Range(1, 3);
        peopleAtD = Random.Range(1, 3);
        StartCoroutine(RegeneratePeople());
    }

    void Update()
    {
        if (!gameActive)
        {
            return;
        }

        timer += Time.deltaTime;
        if (timer >= levelLengthInSeconds)
        {
            EndGame();
        }

        HandleBuildingPickup();
        HandlePassengerPickup();
        HandlePassengerDropoff();
        UpdateUI();
    }

    public void startGame()
    {
        gameActive = true;
        timer = 0;

        GeneratorCanvas.SetActive(false);
        UICanvas.SetActive(true);

        loadingAudio.SetActive(false);
        gameAudio.SetActive(true);
    }

    private void HandleBuildingPickup()
    {
        if (Vector3.Distance(player.transform.position, buildingW.transform.position) < buildingPickupRadius && peopleAtW > 0)
        {
            peopleCarried += peopleAtW;
            peopleAtW = 0;
        }
        if (Vector3.Distance(player.transform.position, buildingA.transform.position) < buildingPickupRadius && peopleAtA > 0)
        {
            peopleCarried += peopleAtA;
            peopleAtA = 0;
        }
        if (Vector3.Distance(player.transform.position, buildingS.transform.position) < buildingPickupRadius && peopleAtS > 0)
        {
            peopleCarried += peopleAtS;
            peopleAtS = 0;
        }
        if (Vector3.Distance(player.transform.position, buildingD.transform.position) < buildingPickupRadius && peopleAtD > 0)
        {
            peopleCarried += peopleAtD;
            peopleAtD = 0;
        }
    }

    private void HandlePassengerPickup()
    {
        Collider[] hitColliders = Physics.OverlapSphere(player.transform.position, pickupRadius);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Passenger"))
            {
                //passengers.Add(hitCollider.gameObject);
                //hitCollider.gameObject.SetActive(false);
                Destroy(hitCollider.gameObject);
                peopleCarried++;
            }
        }
    }

    private void HandlePassengerDropoff()
    {
        if (Vector3.Distance(player.transform.position, destination.transform.position) < dropoffRadius)
        {
            if (peopleCarried > 0)
            {
                deliverSound.Play();
                screenTintScript.TintAndFade();
                deliveries += peopleCarried;
                peopleCarried = 0;
                //passengers.Clear();
            }
        }
    }

    private void UpdateUI()
    {
        timerText.text = $"Time: {Mathf.Max(0, levelLengthInSeconds - (int)timer)}";
        scoreText.text = $"Score: {deliveries}";
        peopleAtWText.text = $"People at W: {peopleAtW}";
        peopleAtAText.text = $"People at A: {peopleAtA}";
        peopleAtSText.text = $"People at S: {peopleAtS}";
        peopleAtDText.text = $"People at D: {peopleAtD}";
        peopleCarriedText.text = $"Carried: {peopleCarried}";
        Ambulance_Movement playerScript = player.GetComponent<Ambulance_Movement>();
        //Debug.Log(playerScript.currentBoostFuel / playerScript.maxBoostFuel);
        float fuelPercentage = playerScript.currentBoostFuel / playerScript.maxBoostFuel;
        RectTransform rt = fuelBarFill.rectTransform;
        rt.sizeDelta = new Vector2(fuelPercentage * originalFuelBarWidth, rt.sizeDelta.y);
        //fuelBarFill.fillAmount = playerScript.currentBoostFuel / playerScript.maxBoostFuel;

        // Get absolute speed
        float speed = Mathf.Abs(playerScript.currentSpeed);
        
        // Map speed to needle rotation
        float needleRotation = Mathf.Lerp(minAngle, maxAngle, speed / maxSpeedDisplayed);
        
        // Rotate the needle
        needleTransform.rotation = Quaternion.Euler(0, 0, -needleRotation);
    }

    private void EndGame()
    {
        gameActive = false;
        UICanvas.SetActive(false);
        EndScreenCanvas.SetActive(true);

        finalScoreText.text = $"Score: {deliveries}";
        finalDeliveriesText.text = $"Deliveries: {deliveries}";
    }

    private IEnumerator RegeneratePeople()
    {
        // while (true)
        // {
        //     yield return new WaitForSeconds(regenerationInterval);

        //     if (gameActive)
        //     {
        //         peopleAtW += Random.Range(1, 2);
        //         peopleAtA += Random.Range(1, 2);
        //         peopleAtS += Random.Range(1, 2);
        //         peopleAtD += Random.Range(1, 2);
                
        //         UpdateUI();
        //     }
        // }
        while (true)
        {
            yield return new WaitForSeconds(regenerationInterval + Random.Range(-2f, 2f));

            if (gameActive)
            {
                List<string> availableBuildings = new List<string>();

                if (peopleAtW < 5) availableBuildings.Add("W");
                if (peopleAtA < 5) availableBuildings.Add("A");
                if (peopleAtS < 5) availableBuildings.Add("S");
                if (peopleAtD < 5) availableBuildings.Add("D");

                if (availableBuildings.Count > 0)
                {
                    string chosenBuilding = availableBuildings[Random.Range(0, availableBuildings.Count)];
                    int newPassengers = Random.Range(1, 6);

                    switch (chosenBuilding)
                    {
                        case "W": peopleAtW = Mathf.Min(5, peopleAtW + newPassengers); StartCoroutine(FlashText(peopleAtWText)); break;
                        case "A": peopleAtA = Mathf.Min(5, peopleAtA + newPassengers); StartCoroutine(FlashText(peopleAtAText)); break;
                        case "S": peopleAtS = Mathf.Min(5, peopleAtS + newPassengers); StartCoroutine(FlashText(peopleAtSText)); break;
                        case "D": peopleAtD = Mathf.Min(5, peopleAtD + newPassengers); StartCoroutine(FlashText(peopleAtDText)); break;
                    }
                    spawnSound.Play();
                }

                UpdateUI();
            }
        }
    }

    private IEnumerator FlashText(TextMeshProUGUI text)
    {
        Color originalColor = text.color;
        text.color = Color.red;
        yield return new WaitForSeconds(0.5f);
        text.color = originalColor;
    }

}
