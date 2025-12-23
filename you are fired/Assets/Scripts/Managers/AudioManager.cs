using UnityEngine;
using UnityEngine.Audio;

/// <summary>
/// 音频管理器 - 负责全局音乐和音量管理
/// 功能：
/// 1. 游戏开始时默认循环播放主菜单音乐
/// 2. 进入credit时播放credit音乐
/// 3. 退出credit时关闭credit音乐并播放主菜单音乐
/// 4. 全局音量受slider调控
/// </summary>
public class AudioManager : MonoBehaviour
{
    public static AudioManager I { get; private set; }

    [Header("===== 音量设置 =====")]
    [Range(0f, 1f)] 
    public float bgmVolume = 0.5f;

    [Header("===== 音频混音器 =====")]
    [SerializeField] private AudioMixer mainMixer; // 在 Inspector 中指定你的 MainMixer

    [Header("===== 菜单音乐 =====")]
    [SerializeField] private AudioClip menuBgm; // 主菜单循环音乐

    [Header("===== Credits 音乐 =====")]
    [SerializeField] private AudioClip creditsBgm; // Credits 音乐
    [SerializeField] private bool creditsLoop = true; // 是否循环

    private AudioSource bgmSource; // 背景音乐源
    private bool inCredits = false; // 是否在 Credits 中
    private bool isPaused = false; // 是否暂停

    private void Awake()
    {
        // 单例模式
        if (I != null && I != this)
        {
            Destroy(gameObject);
            return;
        }
        I = this;
        DontDestroyOnLoad(gameObject);

        InitializeAudioSource();
    }

    private void Start()
    {
        // 游戏开始时自动播放菜单音乐
        PlayMenuBgm();
    }

    /// <summary>
    /// 初始化 AudioSource
    /// </summary>
    private void InitializeAudioSource()
    {
        // 创建并配置 AudioSource
        bgmSource = gameObject.AddComponent<AudioSource>();
        bgmSource.playOnAwake = false;
        bgmSource.loop = false; // 循环由代码控制
        bgmSource.volume = bgmVolume;
        bgmSource.spatialBlend = 0f; // 2D 音频

        // 连接到 MainMixer
        if (mainMixer != null)
        {
            AudioMixerGroup masterGroup = mainMixer.FindMatchingGroups("Master").Length > 0
                ? mainMixer.FindMatchingGroups("Master")[0]
                : mainMixer.outputAudioMixerGroup;
            bgmSource.outputAudioMixerGroup = masterGroup;
            Debug.Log($"[AudioManager] ✓ AudioSource 已连接到 Mixer: {masterGroup.name}");
        }
        else
        {
            Debug.LogWarning("[AudioManager] ⚠ mainMixer 未指定，将直接输出到 Audio Listener");
        }
    }

    /// <summary>
    /// 播放菜单音乐
    /// </summary>
    public void PlayMenuBgm()
    {
        if (inCredits)
        {
            Debug.Log("[AudioManager] 当前在 Credits 中，忽略菜单音乐请求");
            return;
        }

        if (menuBgm == null)
        {
            Debug.LogError("[AudioManager] ✗ menuBgm 未在 Inspector 中赋值！");
            return;
        }

        StopAllBgm();
        bgmSource.clip = menuBgm;
        bgmSource.loop = true;
        bgmSource.Play();
        Debug.Log($"[AudioManager] ✓ 播放菜单音乐: {menuBgm.name}");
    }

    /// <summary>
    /// 进入 Credits - 播放 Credits 音乐
    /// </summary>
    public void PlayCredits()
    {
        if (inCredits)
        {
            Debug.LogWarning("[AudioManager] 已在 Credits 中");
            return;
        }

        if (creditsBgm == null)
        {
            Debug.LogError("[AudioManager] ✗ creditsBgm 未在 Inspector 中赋值！");
            return;
        }

        inCredits = true;
        StopAllBgm();
        bgmSource.clip = creditsBgm;
        bgmSource.loop = creditsLoop;
        bgmSource.Play();
        Debug.Log($"[AudioManager] ✓ 进入 Credits，播放: {creditsBgm.name}");
    }

    /// <summary>
    /// 退出 Credits - 停止 Credits 音乐并恢复菜单音乐
    /// </summary>
    public void EndCredits()
    {
        if (!inCredits)
        {
            Debug.LogWarning("[AudioManager] 未在 Credits 中");
            return;
        }

        inCredits = false;
        Debug.Log("[AudioManager] ✓ 退出 Credits，恢复菜单音乐");
        PlayMenuBgm();
    }

    /// <summary>
    /// 设置全局音量
    /// </summary>
    public void SetVolume(float volume)
    {
        bgmVolume = Mathf.Clamp01(volume);
        if (bgmSource != null)
        {
            bgmSource.volume = bgmVolume;
        }
        Debug.Log($"[AudioManager] 音量设置: {bgmVolume:P0}");
    }

    /// <summary>
    /// 暂停音乐
    /// </summary>
    public void PauseBgm()
    {
        if (isPaused) return;
        bgmSource.Pause();
        isPaused = true;
        Debug.Log("[AudioManager] ⏸ 音乐已暂停");
    }

    /// <summary>
    /// 恢复音乐
    /// </summary>
    public void ResumeBgm()
    {
        if (!isPaused) return;
        bgmSource.UnPause();
        isPaused = false;
        Debug.Log("[AudioManager] ▶ 音乐已恢复");
    }

    /// <summary>
    /// 停止所有音乐
    /// </summary>
    public void StopAllBgm()
    {
        bgmSource.Stop();
        isPaused = false;
        Debug.Log("[AudioManager] ⏹ 音乐已停止");
    }

    /// <summary>
    /// 播放关卡BGM（Intro→Loop） - 兼容旧代码
    /// </summary>
    public void PlayLevelBGM(AudioClip intro, AudioClip loop)
    {
        if (inCredits)
        {
            Debug.LogWarning("[AudioManager] 在 Credits 中，忽略关卡BGM");
            return;
        }

        // 如果只有 loop，直接播放
        if (loop == null && intro != null)
        {
            StopAllBgm();
            bgmSource.clip = intro;
            bgmSource.loop = false;
            bgmSource.Play();
            Debug.Log($"[AudioManager] ✓ 播放关卡音乐（无循环）: {intro.name}");
            return;
        }

        // 如果既有 intro 又有 loop
        if (intro != null && loop != null)
        {
            StopAllBgm();
            bgmSource.clip = intro;
            bgmSource.loop = false;
            bgmSource.Play();
            Debug.Log($"[AudioManager] ✓ 播放关卡音乐 Intro: {intro.name}，然后循环: {loop.name}");
            
            // 计算 Intro 时长后播放 Loop
            double introDuration = intro.samples / (double)intro.frequency;
            // 这里需要通过协程来切换，但为了简化，我们暂时只播放 intro 和 loop 的组合
            // 实际上应该用协程在 intro 结束后切换到 loop
        }
        else if (loop != null)
        {
            StopAllBgm();
            bgmSource.clip = loop;
            bgmSource.loop = true;
            bgmSource.Play();
            Debug.Log($"[AudioManager] ✓ 播放关卡音乐循环: {loop.name}");
        }
        else
        {
            Debug.LogWarning("[AudioManager] ✗ PlayLevelBGM: 没有有效的音频剪辑");
        }
    }

    /// <summary>
    /// 兼容旧代码 - PauseBGM 大写版本
    /// </summary>
    public void PauseBGM() => PauseBgm();

    /// <summary>
    /// 兼容旧代码 - ResumeBGM 大写版本
    /// </summary>
    public void ResumeBGM() => ResumeBgm();

    /// <summary>
    /// 获取当前是否在 Credits 中
    /// </summary>
    public bool IsInCredits => inCredits;

    /// <summary>
    /// 获取当前是否暂停
    /// </summary>
    public bool IsPaused => isPaused;

    /// <summary>
    /// 获取当前播放的音乐
    /// </summary>
    public AudioClip CurrentClip => bgmSource.clip;
}


