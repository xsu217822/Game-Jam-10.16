using UnityEngine;
using UnityEngine.SceneManagement;

public class FSM : MonoBehaviour
{
    public enum State
    {
        menu,
        Gamephase
    }

    [SerializeField]
    private State initialState = State.menu;

    public State CurrentState { get; private set; }

    [SerializeField]
    private string[] levelScenes = { "Level1", "Level2", "Level3", "Level4" };
    private int currentLevelIndex = 0;      

    // Assign audio clips for each scene in the Inspector
    [Header("Scene Audio Clips")]
    [SerializeField] private AudioClip menuAudio;
    [SerializeField] private AudioClip[] levelAudios; // Should match levelScenes length

    private AudioSource audioSource;

    private void Awake()
    {
        // Ensure FSM persists across scene changes
        DontDestroyOnLoad(gameObject);

        audioSource = GetComponent<AudioSource>();
        if (!audioSource)
            audioSource = gameObject.AddComponent<AudioSource>();

        SceneManager.sceneLoaded += OnSceneLoaded;

        if (SceneManager.GetActiveScene().name != initialState.ToString())
        {
            LoadSceneForState(initialState);
        }
        CurrentState = initialState;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        PlaySceneAudio(scene.name);
    }

    private void PlaySceneAudio(string sceneName)
    {
        if (sceneName == "menu" && menuAudio != null)
        {
            audioSource.clip = menuAudio;
            audioSource.Play();
        }
        else
        {
            for (int i = 0; i < levelScenes.Length; i++)
            {
                if (sceneName == levelScenes[i] && i < levelAudios.Length && levelAudios[i] != null)
                {
                    audioSource.clip = levelAudios[i];
                    audioSource.Play();
                    break;
                }
            }
        }
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
