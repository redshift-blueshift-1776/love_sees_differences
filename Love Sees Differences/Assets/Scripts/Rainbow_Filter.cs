using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Rainbow_Filter : MonoBehaviour
{
    [SerializeField] public Image rainbow1; // Drag the ScreenTint Image here in the Inspector
    [SerializeField] public Image rainbow2; // Drag the ScreenTint Image here in the Inspector
    [SerializeField] public Image rainbow3; // Drag the ScreenTint Image here in the Inspector
    [SerializeField] public Image rainbow4; // Drag the ScreenTint Image here in the Inspector
    public float fadeDuration1 = 7.0f; // Duration for fade in and out
    public float fadeDuration2 = 2.0f; // Duration for fade in and out
    public float fadeDuration3 = 3.0f; // Duration for fade in and out
    public float fadeDuration4 = 5.0f; // Duration for fade in and out

    //public bool isTintEnabled = true;

    [SerializeField] public GameObject game;

    private Game gameScript;

    private void Start()
    {
        //isTintEnabled = PlayerPrefs.GetInt("ScreenTintEnabled", 1) == 1;
        if (rainbow1 != null && rainbow2 != null && rainbow3 != null && rainbow4 != null)
        {
            // Ensure the tints are initially invisible
            Color color1 = rainbow1.color;
            color1.a = 0;
            rainbow1.color = color1;
            Color color2 = rainbow2.color;
            color2.a = 0;
            rainbow2.color = color2;
            Color color3 = rainbow3.color;
            color3.a = 0;
            rainbow3.color = color3;
            Color color4 = rainbow4.color;
            color4.a = 0;
            rainbow4.color = color4;
        }
        StartCoroutine(FadeTint1());
        StartCoroutine(FadeTint2());
        StartCoroutine(FadeTint3());
        StartCoroutine(FadeTint4());
    }

    private IEnumerator FadeTint1()
    {
        while (true) {
            // Step 1: Fade In (make the screen green)
            float timer = 0;
            Color color = rainbow1.color;
            color.a = 0.3f;
            rainbow1.color = color;

            // Step 2: Fade Out (restore normal color)
            timer = 0;
            while (timer <= fadeDuration1)
            {
                timer += Time.deltaTime;
                color.a = Mathf.Lerp(0.3f, 0, timer / fadeDuration1);
                rainbow1.color = color;
                yield return null;
            }
        }
    }

    private IEnumerator FadeTint2()
    {
        while (true) {
            // Step 1: Fade In (make the screen green)
            float timer = 0;
            Color color = rainbow2.color;
            color.a = 0.5f;
            rainbow2.color = color;

            // Step 2: Fade Out (restore normal color)
            timer = 0;
            while (timer <= fadeDuration2)
            {
                timer += Time.deltaTime;
                color.a = Mathf.Lerp(0.5f, 0, timer / fadeDuration2);
                rainbow2.color = color;
                yield return null;
            }
        }
    }

    private IEnumerator FadeTint3()
    {
        while (true) {
            // Step 1: Fade In (make the screen green)
            float timer = 0;
            Color color = rainbow3.color;
            color.a = 0.5f;
            rainbow3.color = color;

            // Step 2: Fade Out (restore normal color)
            timer = 0;
            while (timer <= fadeDuration3)
            {
                timer += Time.deltaTime;
                color.a = Mathf.Lerp(0.5f, 0, timer / fadeDuration3);
                rainbow3.color = color;
                yield return null;
            }
        }
    }

    private IEnumerator FadeTint4()
    {
        while (true) {
            // Step 1: Fade In (make the screen green)
            float timer = 0;
            Color color = rainbow4.color;
            color.a = 0.5f;
            rainbow4.color = color;

            // Step 2: Fade Out (restore normal color)
            timer = 0;
            while (timer <= fadeDuration4)
            {
                timer += Time.deltaTime;
                color.a = Mathf.Lerp(0.5f, 0, timer / fadeDuration4);
                rainbow4.color = color;
                yield return null;
            }
        }
    }

    // public void SetTintEnabled(bool isEnabled) {
    //     isTintEnabled = isEnabled;
    //     PlayerPrefs.SetInt("ScreenTintEnabled", isEnabled ? 1 : 0);
    // }
}