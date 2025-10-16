using UnityEngine;

/// <summary>
/// 全局BGM管理：
/// - 主菜单：单曲循环
/// - 每关：先播 Intro（不循环）→ 无缝接 Loop（循环）
/// 技术点：用两个 AudioSource + PlayScheduled，保证采样级无缝衔接。
/// </summary>
public class AudioManager : MonoBehaviour
{
    public static AudioManager I { get; private set; }

    [Header("音量")]
    [Range(0f, 1f)] public float bgmVolume = 0.8f;

    private AudioSource srcA;   // 播放 Intro 或 Loop 的通道A
    private AudioSource srcB;   // 用于预先排程 Loop 的通道B
    private bool isPaused;

    private void Awake()
    {
        if (I != null) { Destroy(gameObject); return; }
        I = this;
        DontDestroyOnLoad(gameObject);

        // 两路Source，避免切换时产生间隙
        srcA = gameObject.AddComponent<AudioSource>();
        srcB = gameObject.AddComponent<AudioSource>();

        foreach (var s in new[] { srcA, srcB })
        {
            s.playOnAwake = false;
            s.loop = false;        // 仅当播放纯循环曲时会置 true
            s.volume = bgmVolume;
            s.spatialBlend = 0f;   // 2D
        }
    }

    private void Update()
    {
        // 跟随调节全局音量（可选）
        if (srcA) srcA.volume = bgmVolume;
        if (srcB) srcB.volume = bgmVolume;
    }

    /// <summary>
    /// 播放主菜单BGM（纯循环，不需要Intro）
    /// </summary>
    public void PlayMenuLoop(AudioClip loopClip)
    {
        if (!loopClip) { StopBGM(); return; }
        isPaused = false;

        double dsp = AudioSettings.dspTime;

        // 用A路循环播放
        srcA.Stop(); srcB.Stop();
        srcA.clip = loopClip;
        srcA.loop = true;
        srcA.PlayScheduled(dsp);
    }

    /// <summary>
    /// 播放关卡BGM：先Intro一次、然后Loop循环
    /// </summary>
    public void PlayLevelBGM(AudioClip intro, AudioClip loop)
    {
        if (!loop)
        { // 没有Loop就当主菜单循环逻辑，但不循环
            PlayMenuLoop(intro);   // 兜底：至少有个Intro可播
            if (intro) srcA.loop = false;
            return;
        }
        isPaused = false;

        double dsp = AudioSettings.dspTime;

        // 通道A：先播Intro（若无Intro则直接播Loop）
        srcA.Stop(); srcB.Stop();

        if (intro)
        {
            srcA.clip = intro;
            srcA.loop = false;
            srcA.PlayScheduled(dsp);

            // 通道B：排程Loop在Intro结束时无缝接入
            double introEnd = dsp + (double)intro.samples / intro.frequency;
            srcB.clip = loop;
            srcB.loop = true;
            srcB.PlayScheduled(introEnd);
        }
        else
        {
            // 无Intro：直接A路循环Loop
            srcA.clip = loop;
            srcA.loop = true;
            srcA.PlayScheduled(dsp);
        }
    }

    public void StopBGM()
    {
        srcA.Stop();
        srcB.Stop();
        isPaused = false;
    }

    public void PauseBGM()
    {
        if (isPaused) return;
        srcA.Pause();
        srcB.Pause();
        isPaused = true;
    }

    public void ResumeBGM()
    {
        if (!isPaused) return;
        srcA.UnPause();
        srcB.UnPause();
        isPaused = false;
    }
}

