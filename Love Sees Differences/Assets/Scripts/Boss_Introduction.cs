using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss_Introduction : MonoBehaviour
{
    [SerializeField] public GameObject boss;
    [SerializeField] public GameObject mainCamera;
    [SerializeField] public GameObject mainCameraReal;

    public Transform originalCameraPosition;
    // Start is called before the first frame update
    void Start()
    {
        mainCamera.SetActive(true);
        mainCameraReal.SetActive(false);
        originalCameraPosition = mainCamera.transform;
        mainCamera.transform.position = boss.transform.position;
        mainCamera.transform.eulerAngles = new Vector3(0,0,0);
        StartCoroutine(CameraMovement());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private IEnumerator CameraMovement()
    {
        float duration = 2f;
        float elapsed = 0f;
        Vector3 targetPosition = new Vector3(boss.transform.position.x, boss.transform.position.y + 10, boss.transform.position.z - 20);
        Vector3 oldPosition = new Vector3(mainCamera.transform.position.x, mainCamera.transform.position.y, mainCamera.transform.position.z);

        while (elapsed < duration) {
            float t = elapsed / duration;

            mainCamera.transform.position = Vector3.Lerp(oldPosition, targetPosition, t);

            elapsed += Time.deltaTime;
            yield return null;
        }

        duration = 2f;
        elapsed = 0f;
        targetPosition = new Vector3(boss.transform.position.x - 20, boss.transform.position.y + 10, boss.transform.position.z - 20);
        oldPosition = new Vector3(mainCamera.transform.position.x, mainCamera.transform.position.y, mainCamera.transform.position.z);
        Quaternion y45 = Quaternion.Euler(0, 45, 0);
        Quaternion targetRotation = y45;
        Quaternion oldRotation = mainCamera.transform.rotation;

        while (elapsed < duration) {
            float t = elapsed / duration;

            mainCamera.transform.position = Vector3.Lerp(oldPosition, targetPosition, t);
            mainCamera.transform.rotation = Quaternion.Slerp(oldRotation, targetRotation, t);

            elapsed += Time.deltaTime;
            yield return null;
        }

        Quaternion yn45 = Quaternion.Euler(0, -45, 0);

        for (int i = 0; i < 5; i++) {
            duration = 0.5f;
            elapsed = 0f;
            targetPosition = new Vector3(boss.transform.position.x + 7, boss.transform.position.y + 10, boss.transform.position.z - 7);
            oldPosition = new Vector3(mainCamera.transform.position.x, mainCamera.transform.position.y, mainCamera.transform.position.z);
            targetRotation = yn45;
            oldRotation = mainCamera.transform.rotation;

            while (elapsed < duration) {
                float t = elapsed / duration;

                mainCamera.transform.position = Vector3.Lerp(oldPosition, targetPosition, t);
                mainCamera.transform.rotation = Quaternion.Slerp(oldRotation, targetRotation, t);

                elapsed += Time.deltaTime;
                yield return null;
            }
            mainCamera.transform.position = oldPosition;
            mainCamera.transform.rotation = oldRotation;
        }
        duration = 3f;
        elapsed = 0f;
        targetPosition = new Vector3(boss.transform.position.x + 20, boss.transform.position.y + 10, boss.transform.position.z - 20);
        oldPosition = new Vector3(mainCamera.transform.position.x, mainCamera.transform.position.y, mainCamera.transform.position.z);
        targetRotation = yn45;
        oldRotation = mainCamera.transform.rotation;

        while (elapsed < duration) {
            float t = elapsed / duration;

            mainCamera.transform.position = Vector3.Lerp(oldPosition, targetPosition, t);
            mainCamera.transform.rotation = Quaternion.Slerp(oldRotation, targetRotation, t);

            elapsed += Time.deltaTime;
            yield return null;
        }

        Quaternion defaultRotation = Quaternion.Euler(0, 0, 0);
        duration = 1f;
        elapsed = 0f;
        targetPosition = new Vector3(boss.transform.position.x, boss.transform.position.y + 10, boss.transform.position.z - 20);
        oldPosition = new Vector3(mainCamera.transform.position.x, mainCamera.transform.position.y, mainCamera.transform.position.z);
        targetRotation = defaultRotation;
        oldRotation = mainCamera.transform.rotation;

        while (elapsed < duration) {
            float t = elapsed / duration;

            mainCamera.transform.position = Vector3.Lerp(oldPosition, targetPosition, t);
            mainCamera.transform.rotation = Quaternion.Slerp(oldRotation, targetRotation, t);

            elapsed += Time.deltaTime;
            yield return null;
        }

        for (int i = 0; i < 10; i++) {
            duration = 0.2f;
            elapsed = 0f;
            targetPosition = new Vector3(boss.transform.position.x, boss.transform.position.y + 7, boss.transform.position.z - 5);
            oldPosition = new Vector3(mainCamera.transform.position.x, mainCamera.transform.position.y, mainCamera.transform.position.z);
            targetRotation = defaultRotation;
            oldRotation = mainCamera.transform.rotation;

            while (elapsed < duration) {
                float t = elapsed / duration;

                mainCamera.transform.position = Vector3.Lerp(oldPosition, targetPosition, t);
                mainCamera.transform.rotation = Quaternion.Slerp(oldRotation, targetRotation, t);

                elapsed += Time.deltaTime;
                yield return null;
            }
            mainCamera.transform.position = oldPosition;
            mainCamera.transform.rotation = oldRotation;
        }

        duration = 2f;
        elapsed = 0f;
        targetPosition = originalCameraPosition.position;
        targetRotation = originalCameraPosition.rotation;
        oldPosition = new Vector3(mainCamera.transform.position.x, mainCamera.transform.position.y, mainCamera.transform.position.z);
        oldRotation = mainCamera.transform.rotation;
        while (elapsed < duration) {
            float t = elapsed / duration;

            mainCamera.transform.position = Vector3.Lerp(oldPosition, targetPosition, t);
            mainCamera.transform.rotation = Quaternion.Slerp(oldRotation, targetRotation, t);

            elapsed += Time.deltaTime;
            yield return null;
        }

        mainCamera.transform.position = originalCameraPosition.position;
        mainCamera.transform.rotation = originalCameraPosition.rotation;
        mainCamera.SetActive(false);
        mainCameraReal.SetActive(true);
    }
}
