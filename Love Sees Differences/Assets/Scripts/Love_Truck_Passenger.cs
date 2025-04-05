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

        // Immediately start dancing if active
        if (dance != Dance.Default && BeatManager.Instance.audioSource.isPlaying)
        {
            NextPose();
        }
    }
    private int lastPoseBeat = -1;

    void Update()
    {
        if (!BeatManager.Instance.audioSource.isPlaying) return;

        int currentBeat = BeatManager.Instance.GetCurrentBeatNumber();

        if (currentBeat != lastPoseBeat)
        {
            lastPoseBeat = currentBeat;
            NextPose();
        }

        UpdateColor();
    }


    void LateUpdate() {
        Debug.Log($"{gameObject.name} rotation in LateUpdate: {transform.rotation.eulerAngles}");
    }

    private void UpdateColor()
    {
        Material color = null;

        switch (type)
        {
            case 'W': color = wColor; break;
            case 'A': color = aColor; break;
            case 'S': color = sColor; break;
            case 'D': color = dColor; break;
        }

        if (color != null)
        {
            head.GetComponent<MeshRenderer>().material = color;
            torso.GetComponent<MeshRenderer>().material = color;
        }
    }


    private IEnumerator ChangePose(Pose pose) {
        if (currentPose == pose)
            yield break;

        float duration = secondsPerBeat / 2f;
        float elapsed = 0f;

        // Store initial rotations
        Quaternion leftArmStart = leftArmJoint.transform.localRotation;
        Quaternion leftArmLowerStart = leftArmLowerJoint.transform.localRotation;
        Quaternion rightArmStart = rightArmJoint.transform.localRotation;
        Quaternion rightArmLowerStart = rightArmLowerJoint.transform.localRotation;
        Quaternion leftLegStart = leftLegJoint.transform.localRotation;

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
            leftArmLowerTarget = defaultRotation;
            rightArmTarget = zn90;
            rightArmLowerTarget = zn90;
            currentPose = Pose.WeightsDance1;
        } else if (pose == Pose.WeightsDance2) {
            leftArmTarget = z90;
            leftArmLowerTarget = z90;
            rightArmTarget = z180;
            rightArmLowerTarget = defaultRotation;
            currentPose = Pose.WeightsDance2;
        } else if (pose == Pose.KirKan) {
            // Add KirKan targets here if needed
        }
        //Debug.Log($"Left arm: start = {leftArmStart.eulerAngles}, target = {leftArmTarget.eulerAngles}");

        // Animate over time
        while (elapsed < duration) {
            float t = elapsed / duration;

            //Debug.Log(t);

            leftArmJoint.transform.localRotation = Quaternion.Slerp(leftArmStart, leftArmTarget, t);
            leftArmLowerJoint.transform.localRotation = Quaternion.Slerp(leftArmLowerStart, leftArmLowerTarget, t);
            rightArmJoint.transform.localRotation = Quaternion.Slerp(rightArmStart, rightArmTarget, t);
            rightArmLowerJoint.transform.localRotation = Quaternion.Slerp(rightArmLowerStart, rightArmLowerTarget, t);
            leftLegJoint.transform.localRotation = Quaternion.Slerp(leftLegStart, leftLegTarget, t);

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Ensure final rotations are exactly set
        leftArmJoint.transform.localRotation = leftArmTarget;
        leftArmLowerJoint.transform.localRotation = leftArmLowerTarget;
        rightArmJoint.transform.localRotation = rightArmTarget;
        rightArmLowerJoint.transform.localRotation = rightArmLowerTarget;
        leftLegJoint.transform.localRotation = leftLegTarget;

        yield return null;
    }


    void NextPose() {
        //Debug.Log($"Starting new pose coroutine at dspTime: {AudioSettings.dspTime}");
        if (currentPoseCoroutine != null) {
            StopCoroutine(currentPoseCoroutine);
        }

        if (dance == Dance.Default) {
            //Debug.Log("Default");
            currentPoseCoroutine = StartCoroutine(ChangePose(Pose.Default));
        } else if (dance == Dance.KirKan) {
            currentPoseCoroutine = StartCoroutine(ChangePose(Pose.KirKan));
        } else if (dance == Dance.Weights) {
            //Debug.Log("Weights");
            if (currentPose == Pose.WeightsDance1) {
                currentPoseCoroutine = StartCoroutine(ChangePose(Pose.WeightsDance2));
            } else {
                currentPoseCoroutine = StartCoroutine(ChangePose(Pose.WeightsDance1));
            }
        }
    }

}
