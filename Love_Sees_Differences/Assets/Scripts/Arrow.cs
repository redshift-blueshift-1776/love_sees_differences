using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    public Vector3 startPosition;
    public Vector3 targetPosition;
    // Start is called before the first frame update
    void Start()
    {
        // GameObject boss = GameObject.Find("The Adversary");
        // targetPosition = boss.transform.position;
        transform.position = startPosition;
        transform.LookAt(targetPosition);
        StartCoroutine(shootArrow());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider c) {
        Debug.Log(c.name);
        if (c.name.Contains("Wall")) {
            Destroy(gameObject);
        }
    }

    private IEnumerator shootArrow() {
        float duration = 2f;
        float elapsed = 0f;
        Vector3 oldPosition = startPosition;
        while (elapsed < duration) {
            float t = elapsed / duration;

            transform.position = Vector3.Lerp(oldPosition, targetPosition, t);

            elapsed += Time.deltaTime;
            yield return null;
        }
        Destroy(gameObject);
    }
}
