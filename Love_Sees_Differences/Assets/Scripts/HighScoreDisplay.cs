using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class HighScoreDisplay : MonoBehaviour
{
    public Transform highScoreContainer; // Content of ScrollView
    public GameObject highScorePrefab;   // A prefab with a Text component
    [SerializeField] public bool isDayMode = true;

    void Start()
    {
        List<string> dayLevelNames = new List<string>
        {
            "Feelings in the Heart", "Champion", "Love Under The Stars",
            "Our Youthful Blossoming Moments", "Lots of Fun",
            "For the Past", "Yellow Clock", "EAT DH", "Legends of the Red and Blue", "Eclipse"
        };
        if (isDayMode)
        {
            foreach (string name in dayLevelNames)
            {
                string key = name + "_day";
                int score = PlayerPrefs.GetInt(key, -1);
                Debug.Log(key + score);
                if (score >= 0)
                    AddHighScoreText($"{name}: {score}");
            }
            int bossScore = (int) PlayerPrefs.GetFloat("Boss_High_Score", -1);
            if (bossScore >= 0)
                AddHighScoreText($"Boss Time: {bossScore}");
        }
        else
        {
            for (int i = 0; i < UnityEngine.SceneManagement.SceneManager.sceneCountInBuildSettings; i++)
            {
                string path = UnityEngine.SceneManagement.SceneUtility.GetScenePathByBuildIndex(i);
                string sceneName = System.IO.Path.GetFileNameWithoutExtension(path);
                if (sceneName.Contains("Night"))
                {
                    string key = sceneName + "_night";
                    int score = PlayerPrefs.GetInt(key, -1);
                    Debug.Log(key + score);
                    if (score >= 0)
                        AddHighScoreText($"{sceneName}: {score}");
                }
            }

            // Add endless
            int endlessScore = PlayerPrefs.GetInt("Endless_High_Score", -1);
            if (endlessScore >= 0)
                AddHighScoreText($"Endless: {endlessScore}");
        }
    }

    void AddHighScoreText(string text)
    {
        GameObject entry = Instantiate(highScorePrefab, highScoreContainer);
        entry.GetComponent<TextMeshProUGUI>().text = text;
    }

    public void ResetEverything() {
        PlayerPrefs.DeleteAll(); // or use DeleteKey("key") for specific ones
        PlayerPrefs.Save();
    }
}