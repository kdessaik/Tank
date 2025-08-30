using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem; // New Input System
#endif
using TMPro;
using UnityEngine.UI;
using System.Text;

public class GameManagers : MonoBehaviour
{
    public HighScores m_HighScores;

    public TextMeshProUGUI m_MessageText;
    public TextMeshProUGUI m_TimerText;

    public GameObject[] m_Tanks;
    private Vector3[] m_TankPositions;
    private Quaternion[] m_TankRotations;

    public GameObject m_HighScorePanel;
    public TextMeshProUGUI m_HighScoresText;

    public Button m_NewGameButton;
    public Button m_HighScoresButton;

    private float m_gameTime = 0;
    public float GameTime { get { return m_gameTime; } }

    private int lastSecond = -1;

    public enum GameState
    {
        Start,
        Playing,
        GameOver
    };

    private GameState m_GameState;
    public GameState State { get { return m_GameState; } }

    private void Awake()
    {
        m_GameState = GameState.Start;
    }

    private void Start()
    {
        m_TankPositions = new Vector3[m_Tanks.Length];
        m_TankRotations = new Quaternion[m_Tanks.Length];

        for (int i = 0; i < m_Tanks.Length; i++)
        {
            m_TankPositions[i] = m_Tanks[i].transform.position;
            m_TankRotations[i] = m_Tanks[i].transform.rotation;
            m_Tanks[i].SetActive(false);
        }

        m_TimerText.gameObject.SetActive(false);
        m_MessageText.text = "Get Ready";

        m_HighScorePanel.SetActive(false);
        m_NewGameButton.gameObject.SetActive(false);
        m_HighScoresButton.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (EscWasReleased())
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#elif UNITY_WEBGL
            Debug.Log("Quit not supported on WebGL.");
#else
            Application.Quit();
#endif
            return;
        }

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

    private bool EscWasReleased()
    {
#if ENABLE_INPUT_SYSTEM
        return Keyboard.current != null && Keyboard.current.escapeKey.wasReleasedThisFrame;
#else
        return Input.GetKeyUp(KeyCode.Escape);
#endif
    }

    private bool EnterWasReleased()
    {
#if ENABLE_INPUT_SYSTEM
        return Keyboard.current != null && Keyboard.current.enterKey.wasReleasedThisFrame;
#else
        return Input.GetKeyUp(KeyCode.Return);
#endif
    }

    private bool OneTankLeft()
    {
        int numTanksLeft = 0;
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            if (m_Tanks[i].activeSelf) numTanksLeft++;
        }
        return numTanksLeft <= 1;
    }

    private bool IsPlayerDead()
    {
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            if (m_Tanks[i].CompareTag("Player") && m_Tanks[i].activeSelf) return false;
        }
        return true;
    }

    private void GameStatePlaying()
    {
        bool isGameOver = false;
        m_gameTime += Time.deltaTime;

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
            m_HighScores.AddScore(Mathf.RoundToInt(m_gameTime));
            m_HighScores.SaveScoresToFile();
        }

        if (isGameOver)
        {
            m_GameState = GameState.GameOver;
            m_NewGameButton.gameObject.SetActive(true);
            m_HighScoresButton.gameObject.SetActive(true);
        }
    }

    public void OnNewGame()
    {
        m_NewGameButton.gameObject.SetActive(false);
        m_HighScoresButton.gameObject.SetActive(false);
        m_HighScorePanel.SetActive(false);

        m_TimerText.gameObject.SetActive(true);
        m_MessageText.text = "";

        m_gameTime = 0;
        m_GameState = GameState.Playing;

        for (int i = 0; i < m_Tanks.Length; i++)
        {
            m_Tanks[i].transform.position = m_TankPositions[i];
            m_Tanks[i].transform.rotation = m_TankRotations[i];
            m_Tanks[i].SetActive(true);
        }
    }

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
}
