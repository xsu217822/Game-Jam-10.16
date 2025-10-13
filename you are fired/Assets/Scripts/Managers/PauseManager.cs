using MyGame.Managers;
using UnityEngine;
// Add the following using directive if AudioManager is in a different namespace
// using YourNamespace; // <-- Replace 'YourNamespace' with the actual namespace of AudioManager

public class pausemanager : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenu;

    private bool isPaused = false;
    public AudioManager audioManager;

    void Start()
    {
        audioManager = FindObjectOfType<AudioManager>();
    }

    void Update()
    {
        // 检查 ESC 键是否被按下
        if (UnityEngine.Input.GetKeyDown(KeyCode.Escape))
        {
            if (!isPaused)
                PauseGame();
            else
                ResumeGame();
        }
    }

    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f;
        if (pauseMenu != null)
            pauseMenu.SetActive(true);
        if (audioManager != null)
            audioManager.PauseBGM();
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;
        if (pauseMenu != null)
            pauseMenu.SetActive(false);
        if (audioManager != null)
            audioManager.ResumeBGM();
    }
}
