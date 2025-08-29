using UnityEngine;
using System.IO;

public class HighScores : MonoBehaviour
{
    public int[] scores = new int[10];

    string currentDirectory;

    public string scoreFileName = "highscores.txt";

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Printing the current directory to the console
        currentDirectory = Application.dataPath;
        Debug.Log("Our current directory is: " + currentDirectory);
        // Load the scores by default
        LoadScoresFromFile();
    }

    public void LoadScoresFromFile()
    {
        // Before we try to read a file, we should check that it exists. If it doesn't exist, we'll log a message and abort.
        bool fileExists = File.Exists(currentDirectory + "\\" + scoreFileName);
        if (fileExists == true)
        {
            Debug.Log("Found high score file " + scoreFileName);
        }
        else
        {
            Debug.Log("The file " + scoreFileName + " does not exist. No scores will be loaded.", this);
            return;
        }

        // Make a new array of default values
        scores = new int[scores.Length];

        // Now we read the file in
        StreamReader fileReader = new StreamReader(currentDirectory + "\\" + scoreFileName);

        int scoreCount = 0;

        // A while loop to read scores
        while (fileReader.Peek() != -1 && scoreCount < scores.Length)
        {
            string fileLine = fileReader.ReadLine();

            int readScore = -1;
            bool didParse = int.TryParse(fileLine, out readScore);

            if (didParse)
            {
                scores[scoreCount] = readScore;
            }
            else
            {
                Debug.Log("Invalid line in scores file at " + scoreCount + ", using default value.", this);
                scores[scoreCount] = 0;
            }

            scoreCount++;
        }

        // Close the stream
        fileReader.Close();
        Debug.Log("High scores read from " + scoreFileName);
    }

    public void SaveScoresToFile()
    {
        // Create a StreamWriter for our file path
        StreamWriter fileWriter = new StreamWriter(currentDirectory + "\\" + scoreFileName);

        // Write the lines to the file
        for (int i = 0; i < scores.Length; i++)
        {
            fileWriter.WriteLine(scores[i]);
        }

        // Close the stream
        fileWriter.Close();

        Debug.Log("High scores written to " + scoreFileName);
    }

    public void AddScore(int newScore)
    {
        int desiredIndex = -1;
        for (int i = 0; i < scores.Length; i++)
        {
            if (scores[i] < newScore || scores[i] == 0)
            {
                desiredIndex = i;
                break;
            }
        }

        if (desiredIndex < 0)
        {
            Debug.Log("Score of " + newScore + " not high enough for high scores list.", this);
            return;
        }

        for (int i = scores.Length - 1; i > desiredIndex; i--)
        {
            scores[i] = scores[i - 1];
        }

        scores[desiredIndex] = newScore;
        Debug.Log("Score of " + newScore + " entered into high scores at position " + desiredIndex, this);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
