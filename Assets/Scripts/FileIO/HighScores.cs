// ============================================================
// Script written by Dessai KIBEHO
// Handles saving, loading, and managing high scores.
// Automatically stores scores in a text file inside
// Application.persistentDataPath for cross-platform persistence.
// ============================================================

using UnityEngine;
using System.IO;

public class HighScores : MonoBehaviour
{
    [Header("High Score Settings")]
    public int[] scores = new int[10]; // Store up to 10 high scores

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
            // File not found: create default scores then save
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
        }
        catch (System.Exception)
        {
            // Silently ignore errors; keeps scores as-is
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
        }
        catch (System.Exception)
        {
            // Silently ignore save errors
        }
    }

    public void AddScore(int newScore)
    {
        // Insert the new score in the correct position (descending order)
        for (int i = 0; i < scores.Length; i++)
        {
            if (newScore > scores[i])
            {
                // Shift lower scores down
                for (int j = scores.Length - 1; j > i; j--)
                {
                    scores[j] = scores[j - 1];
                }
                scores[i] = newScore;
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
        // Optional hotkeys for testing saving/loading:
        if (Input.GetKeyDown(KeyCode.F9))
        {
            LoadScoresFromFile();
        }

        if (Input.GetKeyDown(KeyCode.F10))
        {
            SaveScoresToFile();
        }
    }
}
