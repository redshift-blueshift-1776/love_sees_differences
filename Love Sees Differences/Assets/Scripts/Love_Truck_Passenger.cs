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

    private Coroutine currentPoseCoroutine;


    // Start is called before the first frame update
    void Start()
    {
        game = GameObject.Find("Game");
        gameScript = game.GetComponent<Game>();
        gameAudio = gameScript.gameAudio;
        audioSource = gameAudio.GetComponent<AudioSource>();
        tempo = gameScript.tempo;
        secondsPerBeat = 60f / tempo;

        nextChangeTime = BeatManager.Instance.GetNextBeatTime();
        currentPose = Pose.Default;

    }

    // Update is called once per frame
    void Update()
    {
        if (!BeatManager.Instance.audioSource.isPlaying) {
            return;
        }

        if (AudioSettings.dspTime >= nextChangeTime) {
            nextChangeTime += BeatManager.Instance.secondsPerBeat;
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
        float duration = secondsPerBeat / 2f;
        float elapsed = 0f;

        // Store initial rotations
        Quaternion leftArmStart = leftArmJoint.transform.rotation;
        Quaternion leftArmLowerStart = leftArmLowerJoint.transform.rotation;
        Quaternion rightArmStart = rightArmJoint.transform.rotation;
        Quaternion rightArmLowerStart = rightArmLowerJoint.transform.rotation;
        Quaternion leftLegStart = leftLegJoint.transform.rotation;

        // Target rotations based on pose
        Quaternion defaultRotation = Quaternion.Euler(0, 0, 0);
        Quaternion z90 = Quaternion.Euler(0, 0, 90);
        Quaternion z180 = Quaternion.Euler(0, 0, 180);
        Quaternion zn90 = Quaternion.Euler(0, 0, -90);

        Quaternion leftArmTarget = leftArmStart;
        Quaternion leftArmLowerTarget = leftArmLowerStart;
        Quaternion rightArmTarget = rightArmStart;
        Quaternion rightArmLowerTarget = rightArmLowerStart;
        Quaternion leftLegTarget = leftLegStart;

        // Set target rotations
        if (pose == Pose.Default) {
            leftLegTarget = defaultRotation;
            currentPose = Pose.Default;
        } else if (pose == Pose.WeightsDance1) {
            leftArmTarget = z180;
            leftArmLowerTarget = z180;
            rightArmTarget = zn90;
            rightArmLowerTarget = z180;
            currentPose = Pose.WeightsDance1;
        } else if (pose == Pose.WeightsDance2) {
            leftArmTarget = z90;
            leftArmLowerTarget = z180;
            rightArmTarget = z180;
            rightArmLowerTarget = z180;
            currentPose = Pose.WeightsDance2;
        } else if (pose == Pose.KirKan) {
            // Add KirKan targets here if needed
        }

        // Animate over time
        while (elapsed < duration) {
            float t = elapsed / duration;

            leftArmJoint.transform.rotation = Quaternion.Slerp(leftArmStart, leftArmTarget, t);
            leftArmLowerJoint.transform.rotation = Quaternion.Slerp(leftArmLowerStart, leftArmLowerTarget, t);
            rightArmJoint.transform.rotation = Quaternion.Slerp(rightArmStart, rightArmTarget, t);
            rightArmLowerJoint.transform.rotation = Quaternion.Slerp(rightArmLowerStart, rightArmLowerTarget, t);
            leftLegJoint.transform.rotation = Quaternion.Slerp(leftLegStart, leftLegTarget, t);

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Ensure final rotations are exactly set
        leftArmJoint.transform.rotation = leftArmTarget;
        leftArmLowerJoint.transform.rotation = leftArmLowerTarget;
        rightArmJoint.transform.rotation = rightArmTarget;
        rightArmLowerJoint.transform.rotation = rightArmLowerTarget;
        leftLegJoint.transform.rotation = leftLegTarget;
    }


    void NextPose() {
        if (currentPoseCoroutine != null) {
            StopCoroutine(currentPoseCoroutine);
        }

        if (dance == Dance.Default) {
            Debug.Log("Default");
            currentPoseCoroutine = StartCoroutine(ChangePose(Pose.Default));
        } else if (dance == Dance.KirKan) {
            currentPoseCoroutine = StartCoroutine(ChangePose(Pose.KirKan));
        } else if (dance == Dance.Weights) {
            Debug.Log("Weights");
            if (currentPose == Pose.WeightsDance1) {
                currentPoseCoroutine = StartCoroutine(ChangePose(Pose.WeightsDance2));
            } else {
                currentPoseCoroutine = StartCoroutine(ChangePose(Pose.WeightsDance1));
            }
        }
    }

}
