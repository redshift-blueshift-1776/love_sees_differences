using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LyricsDisplay : MonoBehaviour
{
    [System.Serializable]
    public class LyricLine {
        public float duration; // In beats
        public string text;
    }

    [SerializeField] private string lyricsText;
    private List<LyricLine> lyrics;

    void ParseLyrics() {
        lyrics = new List<LyricLine>();
        string[] lines = lyricsText.Split('\n');

        foreach (string line in lines) {
            if (string.IsNullOrWhiteSpace(line)) continue;
            string[] parts = line.Split(':', 2);
            if (parts.Length < 2) continue;

            if (float.TryParse(parts[0], out float duration)) {
                lyrics.Add(new LyricLine { duration = duration, text = parts[1].Trim() });
            }
        }
    }

    [SerializeField] private GameObject gameAudio;
    [SerializeField] private TMPro.TextMeshProUGUI lyricsDisplay;
    [SerializeField] private float beatsPerMinute = 120f;

    private AudioSource audioSource;
    private int currentLine = 0;
    private double nextLyricTime;
    private float secondsPerBeat;


    // Start is called before the first frame update
    void Start() {
        audioSource = gameAudio.GetComponent<AudioSource>();
        secondsPerBeat = 60f / beatsPerMinute;
        ParseLyrics();
        nextLyricTime = AudioSettings.dspTime;
        //audioSource.Play();
    }

    // Update is called once per frame
    void Update()
    {
        if (currentLine < lyrics.Count && AudioSettings.dspTime >= nextLyricTime) {
            lyricsDisplay.text = lyrics[currentLine].text;
            nextLyricTime += lyrics[currentLine].duration * secondsPerBeat;
            currentLine++;
        }
    }
}
