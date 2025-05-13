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


    // Start is called before the first frame update
    void Start() {
        secondsPerBeat = 60f / beatsPerMinute;
        ParseLyrics();
        
        // Don't start the lyrics until the audio actually starts playing
        nextLyricTime = 0;
        currentLine = 0;
        lyricsDisplay.text = ""; // Start with an empty display
        canvas.SetActive(false);
    }

    // Update is called once per frame
    void Update() {

    }

    public void showLyrics() {
        nextLyricTime = 0;
        currentLine = 0;
        lyricsDisplay.text = "";
        canvas.SetActive(true);
        StartCoroutine(doLyrics());
    }

    public IEnumerator doLyrics() {
        if (nextLyricTime == 0) {
            // Sync with the exact DSP time when the audio starts playing
            nextLyricTime = AudioSettings.dspTime;
        }
        
        while (currentLine < lyrics.Count) {
            double currentTime = AudioSettings.dspTime;
            double waitTime = nextLyricTime - currentTime;

            if (waitTime > 0)
                yield return new WaitForSecondsRealtime((float)waitTime);

            lyricsDisplay.text = lyrics[currentLine].text;
            nextLyricTime += lyrics[currentLine].duration * secondsPerBeat;
            currentLine++;
        }
        lyricsDisplay.text = "";
        canvas.SetActive(false);
    }

}
