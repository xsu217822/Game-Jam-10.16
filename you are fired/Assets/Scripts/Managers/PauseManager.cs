using UnityEngine;

// Pause manager that selects level-specific pause menu
public class pausemanager : MonoBehaviour
{
    [System.Serializable]
    public class LevelPauseMapping
    {
        public LevelConfig level;
        public GameObject pauseMenu;
    }

    [Header("Default / Fallback UI")]
    [SerializeField] private GameObject pauseMenu;       // Fallback if no mapping
    [SerializeField] private GameObject otherUI;
    [SerializeField] private Player player;

    [Header("Per-Level Pause Menus")]
    [SerializeField] private LevelPauseMapping[] levelPauseMenus;

    [Header("Refs")]
    [SerializeField] private LevelDirector levelDirector;

    private GameObject activePauseMenu;
    private bool isPaused = false;
    public AudioManager audioManager;

    private void Awake()
    {
        if (!levelDirector)
            levelDirector = FindObjectOfType<LevelDirector>();
        if (!player)
            player = FindObjectOfType<Player>();
    }

    void OnEnable()
    {
        if (levelDirector != null)
            levelDirector.OnLevelChanged += HandleLevelChanged;
    }

    void OnDisable()
    {
        if (levelDirector != null)
            levelDirector.OnLevelChanged -= HandleLevelChanged;
    }

    void Start()
    {
        audioManager = FindObjectOfType<AudioManager>();
        // Initialize current mapping if level already set
        if (levelDirector && levelDirector.CurrentLevel)
            HandleLevelChanged(levelDirector.CurrentLevel, levelDirector.CurrentLevelIndex);
        // Ensure all mapped menus start inactive
        if (levelPauseMenus != null)
        {
            foreach (var m in levelPauseMenus)
                if (m != null && m.pauseMenu != null)
                    m.pauseMenu.SetActive(false);
        }
        if (pauseMenu) pauseMenu.SetActive(false);
    }

    private void HandleLevelChanged(LevelConfig cfg, int index)
    {
        // Disable previous active menu if any
        if (activePauseMenu && activePauseMenu != pauseMenu)
            activePauseMenu.SetActive(false);

        activePauseMenu = null;
        if (cfg != null && levelPauseMenus != null)
        {
            foreach (var m in levelPauseMenus)
            {
                if (m != null && m.level == cfg)
                {
                    activePauseMenu = m.pauseMenu;
                    break;
                }
            }
        }
        if (activePauseMenu == null)
            activePauseMenu = pauseMenu; // fallback
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!isPaused) PauseGame();
            else ResumeGame();
        }
    }

    public void PauseGame()
    {
        if (isPaused) return;
        isPaused = true;
        if (GameManager.I != null)
            GameManager.I.Pause(true); // sync state if GameManager exists
        else
            Time.timeScale = 0f;

        if (activePauseMenu != null)
            activePauseMenu.SetActive(true);
        if (audioManager != null)
            audioManager.PauseBGM();

        if (player && player.TryGetComponent<SpriteRenderer>(out var sr))
            sr.enabled = false;

        if (otherUI) otherUI.SetActive(false);
    }

    public void ResumeGame()
    {
        if (!isPaused) return;
        isPaused = false;
        if (GameManager.I != null)
            GameManager.I.Pause(false);
        else
            Time.timeScale = 1f;

        if (activePauseMenu != null)
            activePauseMenu.SetActive(false);
        if (audioManager != null)
            audioManager.ResumeBGM();

        if (player && player.TryGetComponent<SpriteRenderer>(out var sr))
            sr.enabled = true;

        if (otherUI) otherUI.SetActive(true);
    }
}
