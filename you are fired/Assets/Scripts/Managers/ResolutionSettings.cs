//Robin Wang
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Audio;

public class ResolutionSettings : MonoBehaviour
{
    [Header("UI Components")]
    public TMP_Dropdown resolutionDropdown;
    public Toggle fullscreenToggle;
    public Slider volumeSlider;

    [Header("Audio Settings")]
    public AudioMixer audioMixer;  // �� Unity Inspector ������ AudioMixer

    private Resolution[] resolutions;
    private int currentResolutionIndex = 0;
    private bool isInitialized = false;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        // --- ��ʼ���ֱ��� ---
        resolutions = Screen.resolutions;
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
        
        // --- 从 PlayerPrefs 加载保存的设置 ---
        if (PlayerPrefs.HasKey("resolutionIndex"))
        {
            int savedIndex = PlayerPrefs.GetInt("resolutionIndex");
            if (savedIndex < resolutions.Length)
            {
                currentResolutionIndex = savedIndex;
            }
        }
        
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();

        // --- 初始化全屏状态（从 PlayerPrefs 加载）---
        bool savedFullscreen = PlayerPrefs.GetInt("fullscreen", 1) == 1;
        fullscreenToggle.isOn = savedFullscreen;

        // --- 初始化音量 ---
        float savedVolume = PlayerPrefs.GetFloat("volume", 0.75f);
        volumeSlider.value = savedVolume;
        SetVolume(savedVolume);

        // --- 添加监听器（初始化完成后） ---
        resolutionDropdown.onValueChanged.AddListener(delegate { ApplySettings(); });
        fullscreenToggle.onValueChanged.AddListener(delegate { ApplySettings(); });
        volumeSlider.onValueChanged.AddListener(SetVolume);
        
        isInitialized = true;
        
        // --- 应用保存的设置 ---
        ApplySettings();
    }

    public void ApplySettings()
    {
        if (!isInitialized) return;
        
        int selectedResolutionIndex = resolutionDropdown.value;
        if (selectedResolutionIndex < 0 || selectedResolutionIndex >= resolutions.Length)
            return;
            
        Resolution selectedResolution = resolutions[selectedResolutionIndex];
        bool isFullscreen = fullscreenToggle.isOn;

        Debug.Log($"Applying resolution: {selectedResolution.width} x {selectedResolution.height}, Fullscreen: {isFullscreen}");
        Screen.SetResolution(selectedResolution.width, selectedResolution.height, isFullscreen);

        // 保存设置
        PlayerPrefs.SetInt("resolutionIndex", selectedResolutionIndex);
        PlayerPrefs.SetInt("fullscreen", isFullscreen ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void SetVolume(float value)
    {
        // 将滑块值 0~1 转换为分贝 (-40 ~ 0dB，更合理的听感范围)
        float clampedValue = Mathf.Clamp01(value);
        float volumeInDb = Mathf.Lerp(-40f, 0f, clampedValue);
        audioMixer.SetFloat("MasterVolume", volumeInDb);
        PlayerPrefs.SetFloat("volume", clampedValue);
    }

    // OnEnable 已移除以避免与 Start 冲突
}
