using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class EndLevelButtons : MonoBehaviour
{
    [Header("Buttons on this prefab")]
    [SerializeField] private Button openAnotherUiButton; // Button A
    [SerializeField] private Button resumeButton;        // Button B

    [Header("Another UI to open then go to menu")]
    [SerializeField] private GameObject anotherUiPrefab;
    [SerializeField] private string menuSceneName = "MainMenu";

    private bool actionTaken;

    private void OnEnable()
    {
        WireButtons(openAnotherUiButton, resumeButton);
    }

    // Allows runtime wiring if you prefer passing the two Button objects via code
    public void WireButtons(Button firstOpenAnotherUi, Button secondResume)
    {
        if (firstOpenAnotherUi)
        {
            firstOpenAnotherUi.onClick.RemoveAllListeners();
            firstOpenAnotherUi.onClick.AddListener(OpenAnotherUiThenLoadMenu);
        }

        if (secondResume)
        {
            secondResume.onClick.RemoveAllListeners();
            secondResume.onClick.AddListener(ResumeAndClose);
        }
    }

    private void OpenAnotherUiThenLoadMenu()
    {
        if (actionTaken) return;
        actionTaken = true;

        if (anotherUiPrefab == null)
        {
            LoadMenu();
            return;
        }

        var ui = Instantiate(anotherUiPrefab);
        // Any button click inside the spawned UI -> load menu
        foreach (var b in ui.GetComponentsInChildren<Button>(true))
        {
            b.onClick.AddListener(() =>
            {
                if (ui) Destroy(ui);
                LoadMenu();
            });
        }
    }

    private void LoadMenu()
    {
        // Ensure game unpauses before switching scene
        Time.timeScale = 1f;

        // Prefer GameManager if available
        var gm = FindObjectOfType<GameManager>();
        if (gm != null)
        {
            gm.LoadMainMenu();
        }
        else if (!string.IsNullOrEmpty(menuSceneName))
        {
            SceneManager.LoadScene(menuSceneName);
        }
    }

    private void ResumeAndClose()
    {
        // Resume time and close only this end-of-level UI
        Time.timeScale = 1f;
        Destroy(gameObject);
        // NOTE: If your LevelDirector exits on final stage, this will only work
        // if the flow hasn't been terminated yet.
    }
}