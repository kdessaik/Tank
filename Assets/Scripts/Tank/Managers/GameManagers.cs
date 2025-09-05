// ============================================================
// Script written by Dessai KIBEHO
// Manages the whole game state:
// start, playing, game over, high scores, timer and audio feedback.
// ============================================================

using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem; // Support for new Input System
#endif
using TMPro;
using UnityEngine.UI;
using System.Text;
using UnityEngine.Audio;

public class GameManagers : MonoBehaviour
{
    [Header("Audio")]
    public AudioSource src;       // Background/ambient music or looping gameplay SFX
    public AudioSource srcWin;    // Dedicated audio source for victory sound
    public AudioClip sfx1, sfx2, sfx3; // sfx3 = victory sound clip

    [Header("High Scores")]
    public HighScores m_HighScores;         // HighScores manager reference

    [Header("UI Elements")]
    public TextMeshProUGUI m_MessageText;   // Message text (start/win/lose)
    public TextMeshProUGUI m_TimerText;     // Timer text display
    public GameObject m_HighScorePanel;     // Panel showing high scores
    public TextMeshProUGUI m_HighScoresText;// Text inside high score panel
    public Button m_NewGameButton;          // Button to start a new game
    public Button m_HighScoresButton;       // Button to show high scores

    [Header("Tanks")]
    public GameObject[] m_Tanks;            // Array of tanks in the game (player + enemies)

    private Vector3[] m_TankPositions;      // Initial positions of each tank
    private Quaternion[] m_TankRotations;   // Initial rotations of each tank

    private float m_gameTime = 0;           // Tracks elapsed time
    public float GameTime { get { return m_gameTime; } }

    private int lastSecond = -1;            // For updating timer display once per second

    public enum GameState { Start, Playing, GameOver };
    private GameState m_GameState;          // Current state of the game
    public GameState State { get { return m_GameState; } }

    private EnemyTankShooting[] m_AllEnemyShooters; // Cached references to enemy shooters
    private bool victorySoundPlayed = false; // Ensure victory sound plays only once per win

    private void Awake()
    {
        // Set initial state
        m_GameState = GameState.Start;
    }

    private void Start()
    {
        StartLoopingAudio();

        // Cache starting positions and rotations, and disable all tanks
        m_TankPositions = new Vector3[m_Tanks.Length];
        m_TankRotations = new Quaternion[m_Tanks.Length];

        for (int i = 0; i < m_Tanks.Length; i++)
        {
            m_TankPositions[i] = m_Tanks[i].transform.position;
            m_TankRotations[i] = m_Tanks[i].transform.rotation;
            m_Tanks[i].SetActive(false);
        }

        CacheEnemyShooters();
        SetEnemyShootingEnabled(false);

        // Hide UI elements at start
        m_TimerText.gameObject.SetActive(false);
        m_MessageText.text = "Get Ready";
        m_HighScorePanel.SetActive(false);
        m_NewGameButton.gameObject.SetActive(false);
        m_HighScoresButton.gameObject.SetActive(false);

        victorySoundPlayed = false;
    }

    // Play background/ambient loop at game start
    public void StartLoopingAudio()
    {
        if (src != null && sfx1 != null)
        {
            src.clip = sfx1;
            src.loop = true;
            src.Play();
        }
    }

    private void Update()
    {
        // Quit game if Escape pressed
        if (EscWasReleased())
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#elif UNITY_WEBGL
            // Quit not supported on WebGL.
#else
            Application.Quit();
#endif
            return;
        }

        // Handle game states
        switch (m_GameState)
        {
            case GameState.Start:
                if (EnterWasReleased()) OnNewGame();
                break;

            case GameState.Playing:
                GameStatePlaying();
                break;

            case GameState.GameOver:
                if (EnterWasReleased()) m_GameState = GameState.Start;
                break;
        }
    }

    // Cross-platform Escape key check
    private bool EscWasReleased()
    {
#if ENABLE_INPUT_SYSTEM
        return Keyboard.current != null && Keyboard.current.escapeKey.wasReleasedThisFrame;
#else
        return Input.GetKeyUp(KeyCode.Escape);
#endif
    }

    // Cross-platform Enter key check
    private bool EnterWasReleased()
    {
#if ENABLE_INPUT_SYSTEM
        return Keyboard.current != null && Keyboard.current.enterKey.wasReleasedThisFrame;
#else
        return Input.GetKeyUp(KeyCode.Return);
#endif
    }

    // Check if only one tank left (win condition)
    private bool OneTankLeft()
    {
        int numTanksLeft = 0;
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            if (m_Tanks[i].activeSelf) numTanksLeft++;
        }
        return numTanksLeft <= 1;
    }

    // Check if player tank is dead
    private bool IsPlayerDead()
    {
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            if (m_Tanks[i].CompareTag("Player") && m_Tanks[i].activeSelf) return false;
        }
        return true;
    }

    // Handle logic while playing
    private void GameStatePlaying()
    {
        bool isGameOver = false;
        m_gameTime += Time.deltaTime;

        // Update timer display once per second
        int seconds = Mathf.RoundToInt(m_gameTime);
        if (seconds != lastSecond)
        {
            lastSecond = seconds;
            m_TimerText.text = string.Format("{0:D2}:{1:D2}", (seconds / 60), (seconds % 60));
        }

        if (IsPlayerDead())
        {
            m_MessageText.text = "TRY AGAIN";
            isGameOver = true;
        }
        else if (OneTankLeft())
        {
            m_MessageText.text = "WINNER!";
            isGameOver = true;

            // Play victory sound once
            if (!victorySoundPlayed && sfx3 != null)
            {
                if (srcWin != null) srcWin.PlayOneShot(sfx3);
                else if (src != null) src.PlayOneShot(sfx3);
                victorySoundPlayed = true;
            }

            // Save high score on win
            if (m_HighScores != null)
            {
                m_HighScores.AddScore(Mathf.RoundToInt(m_gameTime));
                m_HighScores.SaveScoresToFile();
            }
        }

        if (isGameOver)
        {
            SetEnemyShootingEnabled(false); // Stop enemy fire immediately
            m_GameState = GameState.GameOver;
            m_NewGameButton.gameObject.SetActive(true);
            m_HighScoresButton.gameObject.SetActive(true);
        }
    }

    // Start a new game: reset UI, tanks and enemy shooting
    public void OnNewGame()
    {
        m_NewGameButton.gameObject.SetActive(false);
        m_HighScoresButton.gameObject.SetActive(false);
        m_HighScorePanel.SetActive(false);

        m_TimerText.gameObject.SetActive(true);
        m_MessageText.text = "";

        m_gameTime = 0;
        m_GameState = GameState.Playing;

        // Reset all tanks to their starting positions and rotations
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            m_Tanks[i].transform.position = m_TankPositions[i];
            m_Tanks[i].transform.rotation = m_TankRotations[i];
            m_Tanks[i].SetActive(true);
        }

        SetEnemyShootingEnabled(true); // Allow enemies to shoot now
        victorySoundPlayed = false;    // Reset victory flag
    }

    // Show high scores panel with formatted times
    public void OnHighScores()
    {
        m_MessageText.text = "";
        m_HighScorePanel.SetActive(true);

        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < m_HighScores.scores.Length; i++)
        {
            int seconds = m_HighScores.scores[i];
            sb.AppendFormat("{0:D2}:{1:D2}\n", (seconds / 60), (seconds % 60));
        }
        m_HighScoresText.text = sb.ToString();
    }

    // Cache enemy shooter references, including inactive ones
    private void CacheEnemyShooters()
    {
        m_AllEnemyShooters = FindObjectsOfType<EnemyTankShooting>(true);
    }

    // Enable or disable enemy shooting
    private void SetEnemyShootingEnabled(bool enabled)
    {
        if (m_AllEnemyShooters == null) return;

        foreach (var shooter in m_AllEnemyShooters)
        {
            if (shooter == null) continue;

            shooter.ResetShooting();
            shooter.SetShootingEnabled(enabled);
        }
    }
}
