using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Game_2_Endless : MonoBehaviour
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

    [SerializeField] private AudioSource failSound;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI finalScoreText;
    [SerializeField] private TextMeshProUGUI finalDeliveriesText;

    [SerializeField] private TextMeshProUGUI finalFailuresText;
    // [SerializeField] private Image fuelBarFill;

    // [SerializeField] private RectTransform needleTransform;  // Assign in Inspector
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
    private ConcurrentDictionary<RawImage, string> fadingPassengers = new ConcurrentDictionary<RawImage, string>();

    [Header("Game Numbers")]
    public int maxCarryCapacity = 30;

    [SerializeField] private int peopleAtW;
    [SerializeField] private int peopleAtA;
    [SerializeField] private int peopleAtS;
    [SerializeField] private int peopleAtD;

    [SerializeField] private int peopleAtWCritical;
    [SerializeField] private int peopleAtACritical;
    [SerializeField] private int peopleAtSCritical;
    [SerializeField] private int peopleAtDCritical;

    private float originalFuelBarWidth = 500f;

    [SerializeField] private float regenerationInterval = 5f; // Time in seconds between regenerations

    [SerializeField] private float criticalTime = 60f;

    [SerializeField] private float probabilityOfCritical = 0.2f;

    public bool OldUIEnabled;

    void Start()
    {
        //OldUIEnabled = PlayerPrefs.GetInt("UseOldUI", 1) == 1;
        OldUIEnabled = false;
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
        if (failures >= 1)
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
        HandleSingleBuildingPickup(ref peopleAtW, ref peopleAtWCritical, buildingW, "W");
        HandleSingleBuildingPickup(ref peopleAtA, ref peopleAtACritical, buildingA, "A");
        HandleSingleBuildingPickup(ref peopleAtS, ref peopleAtSCritical, buildingS, "S");
        HandleSingleBuildingPickup(ref peopleAtD, ref peopleAtDCritical, buildingD, "D");
    }

    private void HandleSingleBuildingPickup(ref int peopleAtBuilding, ref int criticalPeopleAtBuilding, GameObject building, string buildingName)
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

            if (!OldUIEnabled) {
                // Remove from fading list when picked up
                foreach (var img in fadingPassengers.Keys)
                {
                    if (fadingPassengers[img] == buildingName) {
                        img.color = new Color(img.color.r, img.color.g, img.color.b, 1f); // Reset opacity
                        fadingPassengers.TryRemove(img, out buildingName);
                    }
                }
            }

            UpdateUI();
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
        timerTextNew.text = $"Time: {Mathf.Max(0, (int)timer)}";
        int score = deliveries - failures;
        scoreTextNew.text = $"Score: {score}";

        //peopleCarriedTextNew.text = $"Carried: {peopleCarried}/{maxCarryCapacity}";

        for (int i = 0; i < 5; i++) {
            RawImage imgW = peopleAtWImages[i].GetComponent<RawImage>();
            RawImage imgA = peopleAtAImages[i].GetComponent<RawImage>();
            RawImage imgS = peopleAtSImages[i].GetComponent<RawImage>();
            RawImage imgD = peopleAtDImages[i].GetComponent<RawImage>();

            // Reset alpha to full before setting textures
            imgW.color = new Color(imgW.color.r, imgW.color.g, imgW.color.b, 1f);
            imgA.color = new Color(imgA.color.r, imgA.color.g, imgA.color.b, 1f);
            imgS.color = new Color(imgS.color.r, imgS.color.g, imgS.color.b, 1f);
            imgD.color = new Color(imgD.color.r, imgD.color.g, imgD.color.b, 1f);

            // Assign textures
            imgW.texture = (i < peopleAtWCritical) ? criticalPassengerTexture : passengerTexture;
            imgA.texture = (i < peopleAtACritical) ? criticalPassengerTexture : passengerTexture;
            imgS.texture = (i < peopleAtSCritical) ? criticalPassengerTexture : passengerTexture;
            imgD.texture = (i < peopleAtDCritical) ? criticalPassengerTexture : passengerTexture;

            // Activate UI elements
            peopleAtWImages[i].SetActive(i < peopleAtW);
            peopleAtAImages[i].SetActive(i < peopleAtA);
            peopleAtSImages[i].SetActive(i < peopleAtS);
            peopleAtDImages[i].SetActive(i < peopleAtD);
        }

        for (int i = 0; i < maxCarryCapacity; i++) {
            if (i < peopleCarriedCritical) {
                peopleCarriedImages[i].SetActive(true);
                peopleCarriedImages[i].GetComponent<RawImage>().texture = criticalPassengerTexture;
            } 
            else if (i < peopleCarried) {
                peopleCarriedImages[i].SetActive(true);
                peopleCarriedImages[i].GetComponent<RawImage>().texture = passengerTexture;
            } 
            else {
                peopleCarriedImages[i].SetActive(false);
            }
        }


        Ambulance_Movement_Endless playerScript = player.GetComponent<Ambulance_Movement_Endless>();
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
        while (true)
        {
            yield return new WaitForSeconds(regenerationInterval + Random.Range(-2f, 2f));

            if (gameActive)
            {
                List<string> availableBuildings = new List<string>();

                if (peopleAtW < 5 && peopleAtWCritical == 0) availableBuildings.Add("W");
                if (peopleAtA < 5 && peopleAtACritical == 0) availableBuildings.Add("A");
                if (peopleAtS < 5 && peopleAtSCritical == 0) availableBuildings.Add("S");
                if (peopleAtD < 5 && peopleAtDCritical == 0) availableBuildings.Add("D");

                if (availableBuildings.Count > 0)
                {
                    string chosenBuilding = availableBuildings[Random.Range(0, availableBuildings.Count)];
                    int newPassengers = Random.Range(1, 6);
                    int criticalPassengers = 0;

                    int criticalPassengersAtBuilding = 10;
                    int passengersAtBuilding = 10;

                    switch (chosenBuilding) {
                        case "W": criticalPassengersAtBuilding = peopleAtWCritical; passengersAtBuilding = peopleAtW; break;
                        case "A": criticalPassengersAtBuilding = peopleAtACritical; passengersAtBuilding = peopleAtA; break;
                        case "S": criticalPassengersAtBuilding = peopleAtSCritical; passengersAtBuilding = peopleAtS; break;
                        case "D": criticalPassengersAtBuilding = peopleAtDCritical; passengersAtBuilding = peopleAtD; break;
                    }

                    if (Random.value < probabilityOfCritical
                        && criticalPassengersAtBuilding == 0
                        && passengersAtBuilding + newPassengers >= 5) {
                        criticalPassengers = Random.Range(1, newPassengers + 1);
                    }

                    switch (chosenBuilding)
                    {
                        case "W":
                            peopleAtW = Mathf.Min(5, peopleAtW + newPassengers);
                            peopleAtWCritical = Mathf.Min(peopleAtW, peopleAtWCritical + criticalPassengers);
                            StartCoroutine(FlashText(peopleAtWTextNew));
                            for (int i = 0; i < peopleAtWCritical; i++)
                            {
                                StartCoroutine(CriticalPassengerTimer(criticalTime, peopleAtWImages[i].GetComponent<RawImage>(), "W"));
                            }
                            break;
                        case "A":
                            peopleAtA = Mathf.Min(5, peopleAtA + newPassengers);
                            peopleAtACritical = Mathf.Min(peopleAtA, peopleAtACritical + criticalPassengers);
                            StartCoroutine(FlashText(peopleAtATextNew));
                            for (int i = 0; i < peopleAtACritical; i++)
                            {
                                StartCoroutine(CriticalPassengerTimer(criticalTime, peopleAtAImages[i].GetComponent<RawImage>(), "A"));
                            }
                            break;
                        case "S":
                            peopleAtS = Mathf.Min(5, peopleAtS + newPassengers);
                            peopleAtSCritical = Mathf.Min(peopleAtS, peopleAtSCritical + criticalPassengers);
                            StartCoroutine(FlashText(peopleAtSTextNew));
                            for (int i = 0; i < peopleAtSCritical; i++)
                            {
                                StartCoroutine(CriticalPassengerTimer(criticalTime, peopleAtSImages[i].GetComponent<RawImage>(), "S"));
                            }
                            break;
                        case "D":
                            peopleAtD = Mathf.Min(5, peopleAtD + newPassengers);
                            peopleAtDCritical = Mathf.Min(peopleAtD, peopleAtDCritical + criticalPassengers);
                            StartCoroutine(FlashText(peopleAtDTextNew));
                            for (int i = 0; i < peopleAtDCritical; i++)
                            {
                                StartCoroutine(CriticalPassengerTimer(criticalTime, peopleAtDImages[i].GetComponent<RawImage>(), "D"));
                            }
                            break;
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

    private IEnumerator CriticalPassengerTimer(float time, RawImage passengerImage, string building)
    {
        //Debug.Log("New Critical Passenger at " + building);
        float elapsed = 0;
        Color originalColor = passengerImage.color;

        fadingPassengers.TryAdd(passengerImage, building);

        while (elapsed < time)
        {
            if (!fadingPassengers.ContainsKey(passengerImage)) 
            {
                Debug.Log("Passenger picked up. Stopping fade.");
                passengerImage.color = originalColor; // Reset opacity
                yield break; // Exit the coroutine
            }
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0.1f, elapsed / time);
            passengerImage.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return null;
        }

        // Reset the color to avoid permanently faded images
        passengerImage.color = originalColor;

        // Add failure and remove passenger from the building
        addFailure();
        TriggerScreenShake();
        failSound.Play();

        switch (building)
        {
            case "W":
                peopleAtWCritical = Mathf.Max(0, peopleAtWCritical - 1);
                peopleAtW = Mathf.Max(0, peopleAtW - 1);
                break;
            case "A":
                peopleAtACritical = Mathf.Max(0, peopleAtACritical - 1);
                peopleAtA = Mathf.Max(0, peopleAtA - 1);
                break;
            case "S":
                peopleAtSCritical = Mathf.Max(0, peopleAtSCritical - 1);
                peopleAtS = Mathf.Max(0, peopleAtS - 1);
                break;
            case "D":
                peopleAtDCritical = Mathf.Max(0, peopleAtDCritical - 1);
                peopleAtD = Mathf.Max(0, peopleAtD - 1);
                break;
        }

        UpdateUI();
    }

    void TriggerScreenShake()
    {
        CameraShake.Instance.Shake(0.15f, 6.9f); // Duration, magnitude
    }


}
