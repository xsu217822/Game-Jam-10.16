using UnityEngine;
using UnityEngine.SceneManagement;

public class FSM : MonoBehaviour
{
    public enum State
    {
        menu,
        VNphase,
        Gamephase,
        endGame
    }

    [SerializeField]
    private State initialState = State.menu;

    public State CurrentState { get; private set; }

    private void Awake()
    {
        // Only load the scene if it's not already loaded
        if (SceneManager.GetActiveScene().name != initialState.ToString())
        {
            LoadSceneForState(initialState);
        }
        CurrentState = initialState;
    }

    // Call this from a button's OnClick event in the Inspector
    public void NextState()
    {
        State nextState = GetNextState(CurrentState);
        if (nextState != CurrentState)
        {
            CurrentState = nextState;
            LoadSceneForState(CurrentState);
        }
    }

    private State GetNextState(State state)
    {
        switch (state)
        {
            case State.menu:
                return State.VNphase;
            case State.VNphase:
                return State.Gamephase;
            case State.Gamephase:
                return State.endGame;
            case State.endGame:
                return State.endGame; // No further state
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
            case State.VNphase:
                SceneManager.LoadScene("VNphase");
                break;
            case State.Gamephase:
                SceneManager.LoadScene("Gamephase");
                break;
            case State.endGame:
                SceneManager.LoadScene("endGame");
                break;
        }
    }
}