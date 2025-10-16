using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager I { get; private set; }

    [Header("Volume")]
    [Range(0f, 1f)] public float bgmVolume = 0.8f;

    [Header("Credits")]
    [SerializeField] private AudioClip creditsBgm;
    [SerializeField] private bool creditsLoop = true;
    [SerializeField] private AudioClip menuLoopAfterCredits; // resume menu loop after credits

    private AudioSource srcA;
    private AudioSource srcB;
    private bool isPaused;
    private bool inCredits;

    private void Awake()
    {
        if (I != null) { Destroy(gameObject); return; }
        I = this;
        DontDestroyOnLoad(gameObject);

        srcA = gameObject.AddComponent<AudioSource>();
        srcB = gameObject.AddComponent<AudioSource>();
        foreach (var s in new[] { srcA, srcB })
        {
            s.playOnAwake = false;
            s.loop = false;
            s.volume = bgmVolume;
            s.spatialBlend = 0f;
        }
    }

    private void Update()
    {
        if (srcA) srcA.volume = bgmVolume;
        if (srcB) srcB.volume = bgmVolume;
    }

    public void PlayMenuLoop(AudioClip loopClip)
    {
        if (!loopClip) { StopBGM(); return; }
        isPaused = false;
        double dsp = AudioSettings.dspTime;
        srcA.Stop(); srcB.Stop();
        srcA.clip = loopClip;
        srcA.loop = true;
        srcA.PlayScheduled(dsp);
    }

    public void PlayLevelBGM(AudioClip intro, AudioClip loop)
    {
        if (!loop) { PlayMenuLoop(intro); if (intro) srcA.loop = false; return; }
        isPaused = false;
        double dsp = AudioSettings.dspTime;
        srcA.Stop(); srcB.Stop();
        if (intro)
        {
            srcA.clip = intro; srcA.loop = false; srcA.PlayScheduled(dsp);
            double introEnd = dsp + (double)intro.samples / intro.frequency;
            srcB.clip = loop; srcB.loop = true; srcB.PlayScheduled(introEnd);
        }
        else
        {
            srcA.clip = loop; srcA.loop = true; srcA.PlayScheduled(dsp);
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
        srcA.Pause(); srcB.Pause(); isPaused = true;
    }

    public void ResumeBGM()
    {
        if (!isPaused) return;
        srcA.UnPause(); srcB.UnPause(); isPaused = false;
    }

    // Credits control with menu pause/resume
    public void StartCredits()
    {
        if (inCredits) return;
        inCredits = true;

        GameManager.I?.Pause(true);       // pause main menu
        if (!creditsBgm) { StopBGM(); return; }

        double dsp = AudioSettings.dspTime;
        srcA.Stop(); srcB.Stop();
        srcA.clip = creditsBgm;
        srcA.loop = creditsLoop;
        srcA.PlayScheduled(dsp);
    }

    public void EndCredits()
    {
        if (!inCredits) return;
        inCredits = false;

        GameManager.I?.Pause(false);      // resume main menu
        if (menuLoopAfterCredits)
            PlayMenuLoop(menuLoopAfterCredits);
        else
            StopBGM();
    }
}

