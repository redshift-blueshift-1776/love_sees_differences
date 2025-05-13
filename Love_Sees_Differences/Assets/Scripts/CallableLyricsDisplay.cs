using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CallableLyricsDisplay : MonoBehaviour
{
    [System.Serializable]
    public class LyricLine {
        public float duration; // In beats
        public string text;
    }

    [SerializeField] public string[] lyricsText;
    private List<LyricLine> lyrics;

    void ParseLyrics() {
        lyrics = new List<LyricLine>();
        string[] lines = lyricsText;

        foreach (string line in lines) {
            if (string.IsNullOrWhiteSpace(line)) continue;
            string[] parts = line.Split(':', 2);
            if (parts.Length < 2) continue;

            if (float.TryParse(parts[0], out float duration)) {
                lyrics.Add(new LyricLine { duration = duration, text = parts[1].Trim() });
            }
        }
    }

    [SerializeField] private TMPro.TextMeshProUGUI lyricsDisplay;
    [SerializeField] private float beatsPerMinute = 120f;

    [SerializeField] private GameObject canvas;

    private int currentLine = 0;
    private double nextLyricTime;
    private float secondsPerBeat;

    private Coroutine doLyricsCoroutine;
    // private Coroutine doProgressCoroutine;
    private bool isAutoPlaying = true;

    [SerializeField] private RectTransform lyricsProgressBarFill;
    private float originalLyricsBarWidth;


    // Start is called before the first frame update
    void Start() {
        secondsPerBeat = 60f / beatsPerMinute;
        ParseLyrics();
        
        // Don't start the lyrics until the audio actually starts playing
        nextLyricTime = 0;
        currentLine = 0;
        lyricsDisplay.text = ""; // Start with an empty display
        canvas.SetActive(false);
        originalLyricsBarWidth = lyricsProgressBarFill.sizeDelta.x;
    }

    // Update is called once per frame
    void Update() {

    }

    public void StopAutoPlay() {
        isAutoPlaying = false;

        if (doLyricsCoroutine != null) {
            StopCoroutine(doLyricsCoroutine);
            doLyricsCoroutine = null;
        }
    }

    public void OnNextPressed() {
        if (currentLine < lyrics.Count - 1) {
            if (!isAutoPlaying) {
                currentLine++;
            }
            StopAutoPlay();
            UpdateLyricsDisplay();
        } else {
            // Reached the end manually – treat like skip
            StopAutoPlay();
            lyricsDisplay.text = "";
            canvas.SetActive(false);
            doLyricsCoroutine = null;
        }
        if (currentLine >= lyrics.Count - 1) {
            lyricsDisplay.text = "";
            canvas.SetActive(false);
            doLyricsCoroutine = null;
        }
    }

    public void OnBackPressed() {
        if (currentLine > 0) {
            currentLine--;
            if (isAutoPlaying) {
                currentLine--;
            }
            StopAutoPlay();
            UpdateLyricsDisplay();
        }
    }

    public void OnSkipPressed() {
        //StopCoroutine(doLyricsCoroutine);
        lyricsDisplay.text = "";
        canvas.SetActive(false);
    }

    public void showLyrics() {
        nextLyricTime = 0;
        currentLine = 0;
        lyricsDisplay.text = "";
        canvas.SetActive(true);
        isAutoPlaying = true;

        ResetLyricsProgressBar();

        if (doLyricsCoroutine != null)
            StopCoroutine(doLyricsCoroutine);

        doLyricsCoroutine = StartCoroutine(doLyrics());
        // doProgressCoroutine = StartCoroutine(progressBar());
    }

    // public IEnumerator progressBar() {
    //     while (isAutoPlaying) {
    //         UpdateLyricsProgressBar();
    //         yield return null;
    //     }
    //     yield return null;
    // }

    public IEnumerator doLyrics() {
        if (nextLyricTime == 0) {
            nextLyricTime = AudioSettings.dspTime;
        }

        while (currentLine < lyrics.Count && isAutoPlaying) {
            double currentTime = AudioSettings.dspTime;
            double waitTime = nextLyricTime - currentTime;

            if (waitTime > 0)
                yield return new WaitForSecondsRealtime((float)waitTime);

            UpdateLyricsDisplay();
            nextLyricTime += lyrics[currentLine].duration * secondsPerBeat;
            currentLine++;
        }

        if (isAutoPlaying) {
            lyricsDisplay.text = "";
            canvas.SetActive(false);
        }

        doLyricsCoroutine = null;
    }

    private void UpdateLyricsDisplay() {
        if (currentLine >= 0 && currentLine < lyrics.Count) {
            lyricsDisplay.text = lyrics[currentLine].text;
        }
        UpdateLyricsProgressBar();
    }

    private void UpdateLyricsProgressBar() {
        float percentage = (float)currentLine / (lyrics.Count - 1);

        //Debug.Log(percentage);

        lyricsProgressBarFill.sizeDelta = new Vector2(
            percentage * originalLyricsBarWidth,
            lyricsProgressBarFill.sizeDelta.y
        );
    }

    private void ResetLyricsProgressBar() {
        lyricsProgressBarFill.sizeDelta = new Vector2(0, lyricsProgressBarFill.sizeDelta.y);
    }

}
