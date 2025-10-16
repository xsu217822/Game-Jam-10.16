using UnityEngine;

/// <summary>
/// ȫ��BGM����
/// - ���˵�������ѭ��
/// - ÿ�أ��Ȳ� Intro����ѭ������ �޷�� Loop��ѭ����
/// �����㣺������ AudioSource + PlayScheduled����֤�������޷��νӡ�
/// </summary>
public class AudioManager : MonoBehaviour
{
    public static AudioManager I { get; private set; }

    [Header("����")]
    [Range(0f, 1f)] public float bgmVolume = 0.8f;

    private AudioSource srcA;   // ���� Intro �� Loop ��ͨ��A
    private AudioSource srcB;   // ����Ԥ���ų� Loop ��ͨ��B
    private bool isPaused;

    private void Awake()
    {
        if (I != null) { Destroy(gameObject); return; }
        I = this;
        DontDestroyOnLoad(gameObject);

        // ��·Source�������л�ʱ������϶
        srcA = gameObject.AddComponent<AudioSource>();
        srcB = gameObject.AddComponent<AudioSource>();

        foreach (var s in new[] { srcA, srcB })
        {
            s.playOnAwake = false;
            s.loop = false;        // �������Ŵ�ѭ����ʱ���� true
            s.volume = bgmVolume;
            s.spatialBlend = 0f;   // 2D
        }
    }

    private void Update()
    {
        // �������ȫ����������ѡ��
        if (srcA) srcA.volume = bgmVolume;
        if (srcB) srcB.volume = bgmVolume;
    }

    /// <summary>
    /// �������˵�BGM����ѭ��������ҪIntro��
    /// </summary>
    public void PlayMenuLoop(AudioClip loopClip)
    {
        if (!loopClip) { StopBGM(); return; }
        isPaused = false;

        double dsp = AudioSettings.dspTime;

        // ��A·ѭ������
        srcA.Stop(); srcB.Stop();
        srcA.clip = loopClip;
        srcA.loop = true;
        srcA.PlayScheduled(dsp);
    }

    /// <summary>
    /// ���Źؿ�BGM����Introһ�Ρ�Ȼ��Loopѭ��
    /// </summary>
    public void PlayLevelBGM(AudioClip intro, AudioClip loop)
    {
        if (!loop)
        { // û��Loop�͵����˵�ѭ���߼�������ѭ��
            PlayMenuLoop(intro);   // ���ף������и�Intro�ɲ�
            if (intro) srcA.loop = false;
            return;
        }
        isPaused = false;

        double dsp = AudioSettings.dspTime;

        // ͨ��A���Ȳ�Intro������Intro��ֱ�Ӳ�Loop��
        srcA.Stop(); srcB.Stop();

        if (intro)
        {
            srcA.clip = intro;
            srcA.loop = false;
            srcA.PlayScheduled(dsp);

            // ͨ��B���ų�Loop��Intro����ʱ�޷����
            double introEnd = dsp + (double)intro.samples / intro.frequency;
            srcB.clip = loop;
            srcB.loop = true;
            srcB.PlayScheduled(introEnd);
        }
        else
        {
            // ��Intro��ֱ��A·ѭ��Loop
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

