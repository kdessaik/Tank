using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem; // New Input System
#endif
using TMPro;

public class GameManagers : MonoBehaviour
{
    public HighScores m_HighScores;
    //Reference to the overlay Text to display winning text , etc
    public TextMeshProUGUI m_MessageText;
    public TextMeshProUGUI m_TimerText;
      
 


    public GameObject[] m_Tanks;

    private Vector3[] m_TankPositions;
    private Quaternion[] m_TankRotations;




    private float m_gameTime = 0;
    public float GameTime { get { return m_gameTime; } }

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
    }
    private void Update()
    {
        // ESC to quit game
        if (EscWasReleased())
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false; // Stop Play mode in Editor
#elif UNITY_WEBGL
            Debug.Log("Quit not supported on WebGL.");
#else
            Application.Quit(); // Quit in standalone builds
#endif
            return; // Stop further logic this frame
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

    // ? Helper Methods for input
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
            if (m_Tanks[i].activeSelf == true)
            {
                numTanksLeft++;
            }
        }
        return numTanksLeft <= 1;
    }

    private bool IsPlayerDead()
    {
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            if (m_Tanks[i].tag == "Player" && m_Tanks[i].activeSelf)
            {
                return false; // Player still alive
            }
        }
        return true; // Player dead
    }

    private void GameStatePlaying()
    {
        bool isGameOver = false;
        m_gameTime += Time.deltaTime;

        int seconds = Mathf.RoundToInt(m_gameTime);

        m_TimerText.text = string.Format("{0:D2}:{1:D2}", (seconds / 60), (seconds % 60));

        if (IsPlayerDead())
        {
            //Debug.Log("You Lose");
            m_MessageText.text = "TRY AGAIN";
            isGameOver = true;
        }
        else if (OneTankLeft())
        {
            //Debug.Log("You Win");
            m_MessageText.text = "WINNER!";
            isGameOver = true;
            m_HighScores.AddScore(Mathf.RoundToInt(m_gameTime));
            m_HighScores.SaveScoresToFile();

        }

        if (isGameOver)
        {
            m_GameState = GameState.GameOver;
        }
    }

    private void OnNewGame()
    {
        m_TimerText.gameObject.SetActive(true);
        m_MessageText.text = "";

        m_gameTime = 0;
        m_GameState = GameState.Playing;

        for (int i = 0; i < m_Tanks.Length; i++)
        {
            m_Tanks[i].transform.position = m_TankPositions[i];
            m_Tanks[i].transform.rotation = m_TankRotations[i];
            m_Tanks[i].SetActive(true); // triggers OnEnable() in TankHealth
        }
    }
}
