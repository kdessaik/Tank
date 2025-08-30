using UnityEngine;
using System.IO;

public class HighScores : MonoBehaviour
{
    public int[] scores = new int[10];

    private string scoreFilePath;

    private void Awake()
    {
        // Use persistent path for proper saving on all platforms
        scoreFilePath = Path.Combine(Application.persistentDataPath, "highscores.txt");
        LoadScoresFromFile();
    }

    public void LoadScoresFromFile()
    {
        if (!File.Exists(scoreFilePath))
        {
            Debug.Log("High score file not found, creating default file.");
            InitializeDefaultScores();
            SaveScoresToFile();
            return;
        }

        try
        {
            string[] lines = File.ReadAllLines(scoreFilePath);
            for (int i = 0; i < scores.Length && i < lines.Length; i++)
            {
                if (!int.TryParse(lines[i], out scores[i]))
                {
                    scores[i] = 0; // fallback for invalid data
                }
            }
            Debug.Log("High scores loaded successfully.");
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error loading scores: " + e.Message);
        }
    }

    public void SaveScoresToFile()
    {
        try
        {
            using (StreamWriter writer = new StreamWriter(scoreFilePath))
            {
                for (int i = 0; i < scores.Length; i++)
                {
                    writer.WriteLine(scores[i]);
                }
            }
            Debug.Log("High scores saved to file.");
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error saving scores: " + e.Message);
        }
    }

    public void AddScore(int newScore)
    {
        // Insert the new score in the correct position
        for (int i = 0; i < scores.Length; i++)
        {
            if (newScore > scores[i])
            {
                for (int j = scores.Length - 1; j > i; j--)
                {
                    scores[j] = scores[j - 1];
                }
                scores[i] = newScore;
                Debug.Log("Score " + newScore + " added at position " + i);
                return;
            }
        }
    }

    private void InitializeDefaultScores()
    {
        for (int i = 0; i < scores.Length; i++)
        {
            scores[i] = 0;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F9))
        {
            LoadScoresFromFile();
            Debug.Log("Scores after loading:");
            for (int i = 0; i < scores.Length; i++)
            {
                Debug.Log("Score " + (i + 1) + ": " + scores[i]);
            }
        }

        if (Input.GetKeyDown(KeyCode.F10))
        {
            SaveScoresToFile();
            Debug.Log("Scores saved to file.");
        }
    }
}
