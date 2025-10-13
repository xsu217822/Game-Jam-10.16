// Assets/Scripts/CutscenePlayer.cs
using System;                     // Action
using UnityEngine;
using UnityEngine.Playables;      // �����Timeline
using UnityEngine.UI;             // ����ð�ť�ر�

public class CutscenePlayer : MonoBehaviour
{
    [SerializeField] private PlayableDirector timeline; // ��Ϊ�գ�����Timeline��
    [SerializeField] private Button closeButton;        // ��Ϊ�գ����ð�ť��

    private Action onDone;

    private void Awake()
    {
        if (timeline) timeline.stopped += _ => SafeDone();
        if (closeButton)
        {
            closeButton.onClick.RemoveAllListeners();
            closeButton.onClick.AddListener(SafeDone);
        }
    }

    // �� CutsceneManager ����
    public void Play(Action done)
    {
        onDone = done;
        gameObject.SetActive(true);
        if (timeline) timeline.Play();
        // �����û��TimelineҲû�а�ť��������ڶ����¼������ SignalDone()
    }

    public void SignalDone() => SafeDone();

    private void SafeDone()
    {
        var cb = onDone;
        onDone = null;
        try { cb?.Invoke(); }
        finally { Destroy(gameObject); } // �������ٱ��ι���ʵ��
    }
}
