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
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();

        // --- ��ʼ��ȫ��״̬ ---
        fullscreenToggle.isOn = Screen.fullScreen;

        // --- ��ʼ������ ---
        float savedVolume = PlayerPrefs.GetFloat("volume", 0.75f);
        volumeSlider.value = savedVolume;
        SetVolume(savedVolume);

        // --- ����¼����� ---
        volumeSlider.onValueChanged.AddListener(SetVolume);
    }

    public void ApplySettings()
    {
        int selectedResolutionIndex = resolutionDropdown.value;
        Resolution selectedResolution = resolutions[selectedResolutionIndex];
        bool isFullscreen = fullscreenToggle.isOn;

        Screen.SetResolution(selectedResolution.width, selectedResolution.height, isFullscreen);

        // ��������
        PlayerPrefs.SetInt("resolutionIndex", selectedResolutionIndex);
        PlayerPrefs.SetInt("fullscreen", isFullscreen ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void SetVolume(float value)
    {
        // ������ֵ��0~1��ת��Ϊ�ֱ� (-80 ~ 0dB)
        float volumeInDb = Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1)) * 20f;
        audioMixer.SetFloat("MasterVolume", volumeInDb);
        PlayerPrefs.SetFloat("volume", value);
    }

    void OnEnable()
    {
        // ��ȡ�ֱ��� & ȫ��
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
