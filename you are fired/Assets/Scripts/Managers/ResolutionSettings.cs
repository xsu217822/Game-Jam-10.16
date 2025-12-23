//Robin Wang
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Audio;
using System.Linq;

public class ResolutionSettings : MonoBehaviour
{
    [Header("UI Components")]
    public TMP_Dropdown resolutionDropdown;
    public Toggle fullscreenToggle;
    public Slider volumeSlider;

    private static ResolutionSettings instance;
    private Resolution[] resolutions;
    private int currentResolutionIndex = 0;
    private bool isInitialized = false;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        FindUIComponents();
        InitializeSettings();
    }

    void OnEnable()
    {
        if (resolutionDropdown == null || volumeSlider == null)
        {
            FindUIComponents();
        }
        
        if (isInitialized)
        {
            RefreshUIState();
            RemoveListeners();
            AddListeners();
            
            float savedVolume = PlayerPrefs.GetFloat("volume", 0.5f);
            SetVolume(savedVolume);
        }
    }

    private void FindUIComponents()
    {
        if (resolutionDropdown == null)
        {
            GameObject dropdownObj = GameObject.Find("Canvas/option/Dropdown");
            if (dropdownObj != null)
                resolutionDropdown = dropdownObj.GetComponent<TMP_Dropdown>();
        }
        
        if (fullscreenToggle == null)
        {
            GameObject toggleObj = GameObject.Find("Canvas/option/Toggle");
            if (toggleObj != null)
                fullscreenToggle = toggleObj.GetComponent<Toggle>();
        }
        
        if (volumeSlider == null)
        {
            GameObject sliderObj = GameObject.Find("Canvas/option/Slider");
            if (sliderObj != null)
                volumeSlider = sliderObj.GetComponent<Slider>();
        }

        Canvas canvas = FindAnyObjectByType<Canvas>();
        
        if (canvas != null && (resolutionDropdown == null || fullscreenToggle == null || volumeSlider == null))
        {
            if (resolutionDropdown == null)
            {
                TMP_Dropdown[] dropdowns = canvas.GetComponentsInChildren<TMP_Dropdown>(true);
                if (dropdowns.Length > 0)
                    resolutionDropdown = dropdowns[0];
            }
            
            if (fullscreenToggle == null)
            {
                Toggle[] toggles = canvas.GetComponentsInChildren<Toggle>(true);
                if (toggles.Length > 0)
                    fullscreenToggle = toggles[0];
            }
            
            if (volumeSlider == null)
            {
                Slider[] sliders = canvas.GetComponentsInChildren<Slider>(true);
                if (sliders.Length > 0)
                    volumeSlider = sliders[0];
            }
        }
    }

    private void InitializeSettings()
    {
        resolutions = Screen.resolutions.Distinct().ToArray();
        
        if (resolutionDropdown != null)
        {
            resolutionDropdown.ClearOptions();
            var options = new System.Collections.Generic.List<string>();
            
            for (int i = 0; i < resolutions.Length; i++)
            {
                string option = resolutions[i].width + " x " + resolutions[i].height;
                options.Add(option);

                if (resolutions[i].width == Screen.currentResolution.width &&
                    resolutions[i].height == Screen.currentResolution.height)
                {
                    currentResolutionIndex = i;
                }
            }

            resolutionDropdown.AddOptions(options);
            
            if (PlayerPrefs.HasKey("resolutionIndex"))
            {
                int savedIndex = PlayerPrefs.GetInt("resolutionIndex");
                if (savedIndex < resolutions.Length)
                    currentResolutionIndex = savedIndex;
            }
            
            resolutionDropdown.value = currentResolutionIndex;
            resolutionDropdown.RefreshShownValue();
        }

        if (fullscreenToggle != null)
        {
            bool savedFullscreen = PlayerPrefs.GetInt("fullscreen", 1) == 1;
            fullscreenToggle.isOn = savedFullscreen;
        }

        if (volumeSlider != null)
        {
            float savedVolume = PlayerPrefs.GetFloat("volume", 0.5f);
            volumeSlider.value = savedVolume;
            SetVolume(savedVolume);
        }

        RemoveListeners();
        AddListeners();
        
        isInitialized = true;
        ApplySettings();
    }

    private void AddListeners()
    {
        if (resolutionDropdown != null)
            resolutionDropdown.onValueChanged.AddListener(delegate { ApplySettings(); });
        
        if (fullscreenToggle != null)
            fullscreenToggle.onValueChanged.AddListener(delegate { ApplySettings(); });
        
        if (volumeSlider != null)
            volumeSlider.onValueChanged.AddListener(SetVolume);
    }

    private void RemoveListeners()
    {
        if (resolutionDropdown != null)
            resolutionDropdown.onValueChanged.RemoveAllListeners();
        
        if (fullscreenToggle != null)
            fullscreenToggle.onValueChanged.RemoveAllListeners();
        
        if (volumeSlider != null)
            volumeSlider.onValueChanged.RemoveAllListeners();
    }

    private void RefreshUIState()
    {
        if (resolutionDropdown != null)
        {
            int savedIndex = PlayerPrefs.GetInt("resolutionIndex", currentResolutionIndex);
            if (savedIndex < resolutions.Length)
            {
                resolutionDropdown.value = savedIndex;
                resolutionDropdown.RefreshShownValue();
            }
        }

        if (fullscreenToggle != null)
            fullscreenToggle.isOn = PlayerPrefs.GetInt("fullscreen", 1) == 1;

        if (volumeSlider != null)
        {
            float savedVolume = PlayerPrefs.GetFloat("volume", 0.5f);
            volumeSlider.value = savedVolume;
        }
    }

    public void ApplySettings()
    {
        if (!isInitialized) return;
        
        if (resolutionDropdown == null || resolutions == null)
            return;

        int selectedResolutionIndex = resolutionDropdown.value;
        if (selectedResolutionIndex < 0 || selectedResolutionIndex >= resolutions.Length)
            return;
            
        Resolution selectedResolution = resolutions[selectedResolutionIndex];
        bool isFullscreen = fullscreenToggle != null ? fullscreenToggle.isOn : Screen.fullScreen;

        Screen.SetResolution(selectedResolution.width, selectedResolution.height, isFullscreen);

        PlayerPrefs.SetInt("resolutionIndex", selectedResolutionIndex);
        PlayerPrefs.SetInt("fullscreen", isFullscreen ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void SetVolume(float value)
    {
        if (AudioManager.I != null)
            AudioManager.I.SetVolume(value);

        float clampedValue = Mathf.Clamp01(value);
        PlayerPrefs.SetFloat("volume", clampedValue);
        PlayerPrefs.Save();
    }

    void OnDisable()
    {
        if (isInitialized)
            ApplySettings();
    }
}
