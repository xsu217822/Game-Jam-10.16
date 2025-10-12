using UnityEngine;
using UnityEngine.SceneManagement;

public class FSM : MonoBehaviour
{
    public enum State
    {
        menu,
        Gamephase
    }

    public static FSM Instance { get; private set; }

    [SerializeField]
    private State initialState = State.menu;

    public State CurrentState { get; private set; }

    [SerializeField]
    private string[] levelScenes = { "Level1", "Level2", "Level3", "Level4" };
    private int currentLevelIndex = 0;

    private void Awake()
    {
        // Singleton pattern ¡ª keep one FSM alive
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Ensure we start from correct state
        if (SceneManager.GetActiveScene().name != initialState.ToString())
        {
            LoadSceneForState(initialState);
        }
        CurrentState = initialState;
    }

    public void NextState()
    {
        if (CurrentState == State.Gamephase)
        {
            if (currentLevelIndex < levelScenes.Length - 1)
            {
                currentLevelIndex++;
                LoadLevelScene(currentLevelIndex);
            }
            else
            {
                JumpToMenu();
            }
        }
        else
        {
            State nextState = GetNextState(CurrentState);
            if (nextState != CurrentState)
            {
                CurrentState = nextState;
                if (nextState == State.Gamephase)
                {
                    currentLevelIndex = 0;
                    LoadLevelScene(currentLevelIndex);
                }
                else
                {
                    LoadSceneForState(CurrentState);
                }
            }
        }
    }

    private State GetNextState(State state)
    {
        switch (state)
        {
            case State.menu:
                return State.Gamephase;
            case State.Gamephase:
                return State.menu;
            default:
                return state;
        }
    }

    private void LoadSceneForState(State state)
    {
        switch (state)
        {
            case State.menu:
                SceneManager.LoadScene("menu");
                break;
            case State.Gamephase:
                currentLevelIndex = 0;
                LoadLevelScene(currentLevelIndex);
                break;
        }
    }

    private void LoadLevelScene(int index)
    {
        if (index >= 0 && index < levelScenes.Length)
        {
            SceneManager.LoadScene(levelScenes[index]);
        }
    }

    public int CurrentLevelIndex
    {
        get => currentLevelIndex;
        set
        {
            if (value >= 0 && value < levelScenes.Length)
            {
                currentLevelIndex = value;
                LoadLevelScene(currentLevelIndex);
            }
        }
    }

    public void JumpToMenu()
    {
        CurrentState = State.menu;
        LoadSceneForState(CurrentState);
    }
}
