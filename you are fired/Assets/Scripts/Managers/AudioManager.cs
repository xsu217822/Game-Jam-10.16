using UnityEngine;
using UnityEngine.Audio;

// Add a namespace to avoid CS0101 conflicts
namespace MyGame.Managers
{
    public class AudioManager : MonoBehaviour
    {
        [SerializeField] private AudioMixer mixer;        // ����� Mixer
        [SerializeField] private AudioSource bgmSource;   // �ϵ�һ��ѭ�����ŵ� Source
        [SerializeField] private AudioSource sfxSource;   // �ϵ�һ��һ���� Source

        public void PlayBGM(AudioClip clip, bool loop = true)
        {
            if (!clip) return;
            bgmSource.clip = clip;
            bgmSource.loop = loop;
            bgmSource.Play();
        }

        public void PlaySFX(AudioClip clip)
        {
            if (!clip) return;
            sfxSource.PlayOneShot(clip);
        }

        public void SetMasterVolume(float linear01)
        {
            float db = Mathf.Log10(Mathf.Clamp(linear01, 0.0001f, 1f)) * 20f;
            mixer.SetFloat("MasterVolume", db);  // ȷ�� Mixer ���¶�������������
        }
        public void PauseBGM()
        {
            if (bgmSource != null)
                bgmSource.Pause();
        }

        public void ResumeBGM()
        {
            if (bgmSource != null)
                bgmSource.UnPause();
        }
    }
}

