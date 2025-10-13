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
    public AudioMixer audioMixer;  // 在 Unity Inspector 里拖入 AudioMixer

    private Resolution[] resolutions;
    private int currentResolutionIndex = 0;

    void Awake()
    {
        // 保证只存在一个 ResolutionSettings 实例
        if (FindObjectsOfType<ResolutionSettings>().Length > 1)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        // --- 初始化分辨率 ---
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
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();

        // --- 初始化全屏状态 ---
        fullscreenToggle.isOn = Screen.fullScreen;

        // --- 初始化音量 ---
        float savedVolume = PlayerPrefs.GetFloat("Volume", 0.75f);
        volumeSlider.value = savedVolume;
        SetVolume(savedVolume);

        // --- 添加事件监听 ---
        volumeSlider.onValueChanged.AddListener(SetVolume);
    }

    public void ApplySettings()
    {
        int selectedResolutionIndex = resolutionDropdown.value;
        Resolution selectedResolution = resolutions[selectedResolutionIndex];
        bool isFullscreen = fullscreenToggle.isOn;

        Screen.SetResolution(selectedResolution.width, selectedResolution.height, isFullscreen);

        // 保存设置
        PlayerPrefs.SetInt("resolutionIndex", selectedResolutionIndex);
        PlayerPrefs.SetInt("fullscreen", isFullscreen ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void SetVolume(float value)
    {
        // 将滑条值（0~1）转换为分贝 (-80 ~ 20dB)
        float volumeInDb = Mathf.Lerp(-80f, 20f, Mathf.Clamp01(value));
        audioMixer.SetFloat("MasterVolume", volumeInDb);
        PlayerPrefs.SetFloat("volume", value);
    }

    void OnEnable()
    {
        // 读取分辨率 & 全屏
        if (PlayerPrefs.HasKey("resolutionIndex"))
        {
            int savedIndex = PlayerPrefs.GetInt("resolutionIndex");
            if (savedIndex < resolutionDropdown.options.Count)
            {
                resolutionDropdown.value = savedIndex;
                resolutionDropdown.RefreshShownValue();
            }
        }

        fullscreenToggle.isOn = PlayerPrefs.GetInt("fullscreen", 1) == 1;
    }
}
