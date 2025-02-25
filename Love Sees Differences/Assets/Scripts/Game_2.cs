using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Game_2 : MonoBehaviour
{
    [SerializeField] public GameObject player;
    [SerializeField] public GameObject destination;

    public bool gameActive;
    private float timer;
    [SerializeField] private int levelLengthInSeconds;

    [SerializeField] public GameObject GeneratorCanvas;
    [SerializeField] public GameObject UICanvas;
    [SerializeField] public GameObject EndScreenCanvas;

    [SerializeField] public GameObject loadingAudio;
    [SerializeField] public GameObject gameAudio;

    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI peopleAtWText;
    [SerializeField] private TextMeshProUGUI peopleAtAText;
    [SerializeField] private TextMeshProUGUI peopleAtSText;
    [SerializeField] private TextMeshProUGUI peopleAtDText;
    [SerializeField] private TextMeshProUGUI peopleCarriedText;
    [SerializeField] private TextMeshProUGUI finalScoreText;
    [SerializeField] private TextMeshProUGUI finalDeliveriesText;

    [SerializeField] private AudioSource deliverSound;

    [SerializeField] private GameObject screenTint;

    private Screen_Tint screenTintScript;

    private int deliveries;
    private int peopleCarried;

    private const float pickupRadius = 30.0f;
    private const float dropoffRadius = 50.0f;
    private List<GameObject> passengers = new List<GameObject>();

    // Buildings and their assigned passengers
    [SerializeField] private GameObject buildingW;
    [SerializeField] private GameObject buildingA;
    [SerializeField] private GameObject buildingS;
    [SerializeField] private GameObject buildingD;

    private int peopleAtW;
    private int peopleAtA;
    private int peopleAtS;
    private int peopleAtD;

    private const float regenerationInterval = 10f; // Time in seconds between regenerations

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
        peopleAtW = Random.Range(3, 10); // Adjust these numbers as needed
        peopleAtA = Random.Range(3, 10);
        peopleAtS = Random.Range(3, 10);
        peopleAtD = Random.Range(3, 10);
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
        if (Vector3.Distance(player.transform.position, buildingW.transform.position) < pickupRadius && peopleAtW > 0)
        {
            peopleCarried += peopleAtW;
            peopleAtW = 0;
        }
        if (Vector3.Distance(player.transform.position, buildingA.transform.position) < pickupRadius && peopleAtA > 0)
        {
            peopleCarried += peopleAtA;
            peopleAtA = 0;
        }
        if (Vector3.Distance(player.transform.position, buildingS.transform.position) < pickupRadius && peopleAtS > 0)
        {
            peopleCarried += peopleAtS;
            peopleAtS = 0;
        }
        if (Vector3.Distance(player.transform.position, buildingD.transform.position) < pickupRadius && peopleAtD > 0)
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
                passengers.Add(hitCollider.gameObject);
                hitCollider.gameObject.SetActive(false);
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
                passengers.Clear();
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
        while (true)
        {
            yield return new WaitForSeconds(regenerationInterval);

            if (gameActive)
            {
                peopleAtW += Random.Range(1, 5);
                peopleAtA += Random.Range(1, 5);
                peopleAtS += Random.Range(1, 5);
                peopleAtD += Random.Range(1, 5);
                
                UpdateUI();
            }
        }
    }
}
