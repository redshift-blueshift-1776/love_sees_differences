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

    [SerializeField] public GameObject ScreenTintCanvas;
    [SerializeField] public GameObject UICanvas;

    [SerializeField] public GameObject UICanvasNew;
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

    [SerializeField] private TextMeshProUGUI finalFailuresText;
    [SerializeField] private Image fuelBarFill;

    [SerializeField] private RectTransform needleTransform;  // Assign in Inspector
    [SerializeField] private float maxSpeedDisplayed = 151f;         // Maximum speed displayed
    private float minAngle = -135f;  // Leftmost angle
    private float maxAngle = 135f;   // Rightmost angle

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

    [SerializeField] private Image fuelBarFillNew;

    [SerializeField] private RectTransform needleTransformNew;  // Assign in Inspector

    [SerializeField] public Texture passengerTexture;
    [SerializeField] public Texture criticalPassengerTexture;

    [SerializeField] private GameObject screenTint;

    private Screen_Tint screenTintScript;

    private int deliveries;
    private int failures;
    public int peopleCarried;

    public int peopleCarriedCritical;

    private const float pickupRadius = 20.0f;
    private const float buildingPickupRadius = 30.0f;
    private const float dropoffRadius = 50.0f;
    private List<GameObject> passengers = new List<GameObject>();

    [Header("Game Numbers")]
    public int maxCarryCapacity = 30;

    private int peopleAtW;
    private int peopleAtA;
    private int peopleAtS;
    private int peopleAtD;

    private int peopleAtWCritical;
    private int peopleAtACritical;
    private int peopleAtSCritical;
    private int peopleAtDCritical;

    private float originalFuelBarWidth = 500f;

    [SerializeField] private float regenerationInterval = 5f; // Time in seconds between regenerations

    [SerializeField] private float criticalTime = 60f;

    [SerializeField] private float probabilityOfCritical = 0.2f;

    public bool OldUIEnabled;

    void Start()
    {
        OldUIEnabled = PlayerPrefs.GetInt("UseOldUI", 1) == 1;
        gameActive = false;
        timer = 0;
        deliveries = 0;
        peopleCarried = 0;

        peopleAtWImages = GetChildRawImages(peopleAtWParent);
        peopleAtAImages = GetChildRawImages(peopleAtAParent);
        peopleAtSImages = GetChildRawImages(peopleAtSParent);
        peopleAtDImages = GetChildRawImages(peopleAtDParent);
        peopleCarriedImages = GetChildRawImages(peopleCarriedParent);

        GeneratorCanvas.SetActive(true);
        ScreenTintCanvas.SetActive(false);
        UICanvas.SetActive(false);
        UICanvasNew.SetActive(false);
        EndScreenCanvas.SetActive(false);

        loadingAudio.SetActive(true);
        gameAudio.SetActive(false);

        screenTintScript = screenTint.GetComponent<Screen_Tint>();

        // Initialize passenger counts at buildings
        peopleAtW = Random.Range(1, 3); // Adjust these numbers as needed
        peopleAtA = Random.Range(1, 3);
        peopleAtS = Random.Range(1, 3);
        peopleAtD = Random.Range(1, 3);
        peopleAtWCritical = 0;
        peopleAtACritical = 0;
        peopleAtSCritical = 0;
        peopleAtDCritical = 0;
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

    // private GameObject[] GetChildImages(Transform parent)
    // {
    //     if (parent == null)
    //     {
    //         Debug.LogError("Parent transform is not assigned!");
    //         return new GameObject[0];
    //     }

    //     // Find all images under the specified parent
    //     Image[] images = parent.GetComponentsInChildren<Image>();

    //     // Convert Image components to GameObjects
    //     GameObject[] imageObjects = new GameObject[images.Length];
    //     for (int i = 0; i < images.Length; i++)
    //     {
    //         imageObjects[i] = images[i].gameObject;
    //     }

    //     return imageObjects;
    // }

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

    public void startGame()
    {
        gameActive = true;
        timer = 0;

        GeneratorCanvas.SetActive(false);
        ScreenTintCanvas.SetActive(true);
        
        if (OldUIEnabled) {
            UICanvas.SetActive(true);
            UICanvasNew.SetActive(false);
        } else {
            UICanvas.SetActive(false);
            UICanvasNew.SetActive(true);
        }

        loadingAudio.SetActive(false);
        gameAudio.SetActive(true);
    }

    // private void HandleBuildingPickup()
    // {
    //     if (Vector3.Distance(player.transform.position, buildingW.transform.position) < buildingPickupRadius && peopleAtW > 0)
    //     {
    //         peopleCarried += peopleAtW;
    //         peopleAtW = 0;
    //     }
    //     if (Vector3.Distance(player.transform.position, buildingA.transform.position) < buildingPickupRadius && peopleAtA > 0)
    //     {
    //         peopleCarried += peopleAtA;
    //         peopleAtA = 0;
    //     }
    //     if (Vector3.Distance(player.transform.position, buildingS.transform.position) < buildingPickupRadius && peopleAtS > 0)
    //     {
    //         peopleCarried += peopleAtS;
    //         peopleAtS = 0;
    //     }
    //     if (Vector3.Distance(player.transform.position, buildingD.transform.position) < buildingPickupRadius && peopleAtD > 0)
    //     {
    //         peopleCarried += peopleAtD;
    //         peopleAtD = 0;
    //     }
    // }

    // private void HandlePassengerPickup()
    // {
    //     Collider[] hitColliders = Physics.OverlapSphere(player.transform.position, pickupRadius);
    //     foreach (var hitCollider in hitColliders)
    //     {
    //         if (hitCollider.CompareTag("Passenger"))
    //         {
    //             //passengers.Add(hitCollider.gameObject);
    //             //hitCollider.gameObject.SetActive(false);
    //             Destroy(hitCollider.gameObject);
    //             peopleCarried++;
    //         }
    //     }
    // }
    private void HandleBuildingPickup()
    {
        HandleSingleBuildingPickup(ref peopleAtW, ref peopleAtWCritical, buildingW);
        HandleSingleBuildingPickup(ref peopleAtA, ref peopleAtACritical, buildingA);
        HandleSingleBuildingPickup(ref peopleAtS, ref peopleAtSCritical, buildingS);
        HandleSingleBuildingPickup(ref peopleAtD, ref peopleAtDCritical, buildingD);
    }

    private void HandleSingleBuildingPickup(ref int peopleAtBuilding, ref int criticalPeopleAtBuilding, GameObject building)
    {
        if (Vector3.Distance(player.transform.position, building.transform.position) < buildingPickupRadius && peopleAtBuilding > 0)
        {
            int spaceAvailable = maxCarryCapacity - peopleCarried;
            int peopleToPickup = Mathf.Min(peopleAtBuilding, spaceAvailable);
            int criticalPeopleToPickup = Mathf.Min(criticalPeopleAtBuilding, spaceAvailable);

            peopleCarried += peopleToPickup;
            peopleCarriedCritical += criticalPeopleToPickup;
            peopleAtBuilding -= peopleToPickup;
            criticalPeopleAtBuilding -= criticalPeopleToPickup;
        }
    }

    private void HandlePassengerPickup()
    {
        Collider[] hitColliders = Physics.OverlapSphere(player.transform.position, pickupRadius);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Passenger") && peopleCarried < maxCarryCapacity)
            {
                Destroy(hitCollider.gameObject);
                peopleCarried++;
                
                if (peopleCarried >= maxCarryCapacity)
                    break; // Stop picking up once full
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
                peopleCarriedCritical = 0;
                //passengers.Clear();
            }
        }
    }

    public void addFailure()
    {
        failures++;
    }

    private void UpdateUI()
    {
        if (OldUIEnabled) {
            timerText.text = $"Time: {Mathf.Max(0, levelLengthInSeconds - (int)timer)}";
            int score = deliveries - failures;
            scoreText.text = $"Score: {score}";
            peopleAtWText.text = $"People at W: {peopleAtW} ({peopleAtWCritical})";
            peopleAtAText.text = $"People at A: {peopleAtA} ({peopleAtACritical})";
            peopleAtSText.text = $"People at S: {peopleAtS} ({peopleAtSCritical})";
            peopleAtDText.text = $"People at D: {peopleAtD} ({peopleAtDCritical})";
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
        } else {
            timerTextNew.text = $"Time: {Mathf.Max(0, levelLengthInSeconds - (int)timer)}";
            int score = deliveries - failures;
            scoreTextNew.text = $"Score: {score}";

            //peopleCarriedTextNew.text = $"Carried: {peopleCarried}/{maxCarryCapacity}";

            for (int i = 0; i < 5; i++) {
                peopleAtWImages[i].SetActive(i < peopleAtW);
                peopleAtAImages[i].SetActive(i < peopleAtA);
                peopleAtSImages[i].SetActive(i < peopleAtS);
                peopleAtDImages[i].SetActive(i < peopleAtD);

                peopleAtWImages[i].GetComponent<RawImage>().texture = (i < peopleAtWCritical) ? criticalPassengerTexture : passengerTexture;
                peopleAtAImages[i].GetComponent<RawImage>().texture = (i < peopleAtACritical) ? criticalPassengerTexture : passengerTexture;
                peopleAtSImages[i].GetComponent<RawImage>().texture = (i < peopleAtSCritical) ? criticalPassengerTexture : passengerTexture;
                peopleAtDImages[i].GetComponent<RawImage>().texture = (i < peopleAtDCritical) ? criticalPassengerTexture : passengerTexture;
            }

            for (int i = 0; i < maxCarryCapacity; i++) {
                if (i < peopleCarriedCritical) {
                    peopleCarriedImages[i].SetActive(true);
                    peopleCarriedImages[i].GetComponent<RawImage>().texture = criticalPassengerTexture;
                } else if (i < peopleCarried) {
                    peopleCarriedImages[i].SetActive(true);
                    peopleCarriedImages[i].GetComponent<RawImage>().texture = passengerTexture;
                } else {
                    peopleCarriedImages[i].SetActive(false);
                }
            }

            Ambulance_Movement playerScript = player.GetComponent<Ambulance_Movement>();
            //Debug.Log(playerScript.currentBoostFuel / playerScript.maxBoostFuel);
            float fuelPercentage = playerScript.currentBoostFuel / playerScript.maxBoostFuel;
            RectTransform rt = fuelBarFillNew.rectTransform;
            rt.sizeDelta = new Vector2(fuelPercentage * originalFuelBarWidth, rt.sizeDelta.y);
            //fuelBarFill.fillAmount = playerScript.currentBoostFuel / playerScript.maxBoostFuel;

            // Get absolute speed
            float speed = Mathf.Abs(playerScript.currentSpeed);
            
            // Map speed to needle rotation
            float needleRotation = Mathf.Lerp(minAngle, maxAngle, speed / maxSpeedDisplayed);
            
            // Rotate the needle
            needleTransformNew.rotation = Quaternion.Euler(0, 0, -needleRotation);
        }
    }

    private void EndGame()
    {
        gameActive = false;
        UICanvas.SetActive(false);
        ScreenTintCanvas.SetActive(false);
        UICanvasNew.SetActive(false);
        EndScreenCanvas.SetActive(true);

        int score = deliveries - failures;

        finalScoreText.text = $"Score: {score}";
        finalDeliveriesText.text = $"Deliveries: {deliveries}";
        finalFailuresText.text = $"Failures: {failures}";
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
                    int criticalPassengers = 0;

                    if (Random.value < probabilityOfCritical) {
                        criticalPassengers = Mathf.Min(newPassengers, Random.Range(1, 3));
                    }

                    switch (chosenBuilding)
                    {
                        case "W":
                            peopleAtW = Mathf.Min(5, peopleAtW + newPassengers);
                            peopleAtWCritical = Mathf.Min(peopleAtW, peopleAtWCritical + criticalPassengers);
                            if (OldUIEnabled) {
                                StartCoroutine(FlashText(peopleAtWText)); break;
                            } else {
                                StartCoroutine(FlashText(peopleAtWTextNew));
                                for (int i = 0; i < peopleAtWCritical; i++)
                                {
                                    StartCoroutine(CriticalPassengerTimer(criticalTime, peopleAtWImages[i].GetComponent<RawImage>()));
                                }
                                break;
                            }
                        case "A":
                            peopleAtA = Mathf.Min(5, peopleAtA + newPassengers);
                            peopleAtACritical = Mathf.Min(peopleAtA, peopleAtACritical + criticalPassengers);
                            if (OldUIEnabled) {
                                StartCoroutine(FlashText(peopleAtAText)); break;
                            } else {
                                StartCoroutine(FlashText(peopleAtATextNew));
                                for (int i = 0; i < peopleAtACritical; i++)
                                {
                                    StartCoroutine(CriticalPassengerTimer(criticalTime, peopleAtAImages[i].GetComponent<RawImage>()));
                                }
                                break;
                            }
                        case "S":
                            peopleAtS = Mathf.Min(5, peopleAtS + newPassengers);
                            peopleAtSCritical = Mathf.Min(peopleAtS, peopleAtSCritical + criticalPassengers);
                            if (OldUIEnabled) {
                                StartCoroutine(FlashText(peopleAtSText)); break;
                            } else {
                                StartCoroutine(FlashText(peopleAtSTextNew));
                                for (int i = 0; i < peopleAtWCritical; i++)
                                {
                                    StartCoroutine(CriticalPassengerTimer(criticalTime, peopleAtSImages[i].GetComponent<RawImage>()));
                                }
                                break;
                            }
                        case "D":
                            peopleAtD = Mathf.Min(5, peopleAtD + newPassengers);
                            peopleAtDCritical = Mathf.Min(peopleAtD, peopleAtDCritical + criticalPassengers);
                            if (OldUIEnabled) {
                                StartCoroutine(FlashText(peopleAtDText)); break;
                            } else {
                                StartCoroutine(FlashText(peopleAtDTextNew));
                                for (int i = 0; i < peopleAtDCritical; i++)
                                {
                                    StartCoroutine(CriticalPassengerTimer(criticalTime, peopleAtDImages[i].GetComponent<RawImage>()));
                                }
                                break;
                            }
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

    private IEnumerator CriticalPassengerTimer(float time, RawImage passengerImage)
    {
        Debug.Log("New Critical Passenger");
        float elapsed = 0;
        Color originalColor = passengerImage.color;

        while (elapsed < time)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0.2f, elapsed / time);
            passengerImage.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return null;
        }

        addFailure();
        peopleCarriedCritical = Mathf.Max(0, peopleCarriedCritical - 1);
        peopleCarried = Mathf.Max(0, peopleCarried - 1);
        UpdateUI();
    }

}
