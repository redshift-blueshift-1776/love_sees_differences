using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Love_Truck_Passenger : MonoBehaviour
{
    public enum Pose {
        Default,
        KirKan,
        WeightsDance1,
        WeightsDance2,
    }

    public enum Dance {
        Default,
        KirKan,
        Weights,
        Champion,
        Aluminum,
        Floss,
        Floss24
    }

    [SerializeField] public GameObject game;

    [SerializeField] public Dance dance;

    [SerializeField] private Pose currentPose;

    public Game gameScript;

    private GameObject gameAudio;

    public AudioSource audioSource;

    public char type; // char for which building they go to

    [Header("Materials")]

    [SerializeField] public Material wColor;
    [SerializeField] public Material aColor;
    [SerializeField] public Material sColor;
    [SerializeField] public Material dColor;

    [Header("Body Parts")]
    [SerializeField] public GameObject torso;
    [SerializeField] public GameObject leftLeg;
    [SerializeField] public GameObject rightLeg;

    [SerializeField] public GameObject leftLegLower;
    [SerializeField] public GameObject rightLegLower;

    [SerializeField] public GameObject leftArm;
    [SerializeField] public GameObject rightArm;

    [SerializeField] public GameObject leftArmLower;
    [SerializeField] public GameObject rightArmLower;

    [SerializeField] public GameObject head;

    [Header("Joints")]
    [SerializeField] public GameObject leftLegJoint;
    [SerializeField] public GameObject rightLegJoint;

    [SerializeField] public GameObject leftLegLowerJoint;
    [SerializeField] public GameObject rightLegLowerJoint;

    [SerializeField] public GameObject leftArmJoint;
    [SerializeField] public GameObject rightArmJoint;

    [SerializeField] public GameObject leftArmLowerJoint;
    [SerializeField] public GameObject rightArmLowerJoint;

    private float tempo;

    private float secondsPerBeat;

    private double nextChangeTime;

    // Start is called before the first frame update
    void Start()
    {
        game = GameObject.Find("Game");
        gameScript = game.GetComponent<Game>();
        gameAudio = gameScript.gameAudio;
        audioSource = gameAudio.GetComponent<AudioSource>();
        tempo = gameScript.tempo;
        secondsPerBeat = 60f / tempo;

        nextChangeTime = 0f;
        currentPose = Pose.Default;

    }

    // Update is called once per frame
    void Update()
    {
        if (!audioSource.isPlaying) {
            // If the audio isn't playing, reset
            nextChangeTime = 0;
            return;
        }
        if (nextChangeTime == 0) {
            // Sync with the exact DSP time when the audio starts playing
            nextChangeTime = AudioSettings.dspTime;
        }
        if (AudioSettings.dspTime >= nextChangeTime) {
            nextChangeTime += secondsPerBeat;
            NextPose();
        }
        if (type == 'W') {
            head.GetComponent<MeshRenderer>().material = wColor;
            torso.GetComponent<MeshRenderer>().material = wColor;
        } else if (type == 'A') {
            head.GetComponent<MeshRenderer>().material = aColor;
            torso.GetComponent<MeshRenderer>().material = aColor;
        } else if (type == 'S') {
            head.GetComponent<MeshRenderer>().material = sColor;
            torso.GetComponent<MeshRenderer>().material = sColor;
        } else if (type == 'D') {
            head.GetComponent<MeshRenderer>().material = dColor;
            torso.GetComponent<MeshRenderer>().material = dColor;
        }
    }

    private IEnumerator ChangePose(Pose pose) {
        float poseChangeTime = 0;
        while (poseChangeTime < secondsPerBeat / 2f) {
            Quaternion defaultRotation = Quaternion.Euler(0, 0, 0);
            Quaternion z90 = Quaternion.Euler(0, 0, 90);
            Quaternion z180 = Quaternion.Euler(0, 0, 180);
            if (pose == Pose.Default) {
                leftLegJoint.transform.rotation = Quaternion.Slerp(leftLegJoint.transform.rotation, defaultRotation, Time.deltaTime * 5f);
                currentPose = Pose.Default;
            }
            if (pose == Pose.WeightsDance1) {
                Debug.Log("WeightsDance1");
                leftArmJoint.transform.rotation = Quaternion.Slerp(leftArmJoint.transform.rotation, z180, Time.deltaTime * 5f);
                leftArmLowerJoint.transform.rotation = Quaternion.Slerp(leftArmLowerJoint.transform.rotation, defaultRotation, Time.deltaTime * 5f);
                rightArmJoint.transform.rotation = Quaternion.Slerp(rightArmJoint.transform.rotation, z90, Time.deltaTime * 5f);
                rightArmLowerJoint.transform.rotation = Quaternion.Slerp(rightArmLowerJoint.transform.rotation, z90, Time.deltaTime * 5f);
                currentPose = Pose.WeightsDance1;
            }
            if (pose == Pose.WeightsDance2) {
                Debug.Log("WeightsDance2");
                leftArmJoint.transform.rotation = Quaternion.Slerp(leftArmJoint.transform.rotation, z90, Time.deltaTime * 5f);
                leftArmLowerJoint.transform.rotation = Quaternion.Slerp(leftArmLowerJoint.transform.rotation, z90, Time.deltaTime * 5f);
                rightArmJoint.transform.rotation = Quaternion.Slerp(rightArmJoint.transform.rotation, z180, Time.deltaTime * 5f);
                rightArmLowerJoint.transform.rotation = Quaternion.Slerp(rightArmLowerJoint.transform.rotation, defaultRotation, Time.deltaTime * 5f);
                currentPose = Pose.WeightsDance2;
            } else {
                return null;
            }
        }
        return null;
    }

    void NextPose() {
        if (dance == Dance.Default) {
            Debug.Log("Default");
            StartCoroutine(ChangePose(Pose.Default));
        } else if (dance == Dance.KirKan) {
            ChangePose(Pose.KirKan);
        } else if (dance == Dance.Weights) {
            Debug.Log("Weights");
            if (currentPose == Pose.WeightsDance1) {
                ChangePose(Pose.WeightsDance2);
            } else {
                ChangePose(Pose.WeightsDance1);
            }
        }
    }
}
