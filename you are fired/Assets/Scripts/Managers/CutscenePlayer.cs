// Assets/Scripts/CutscenePlayer.cs
using System;                     // Action
using UnityEngine;
using UnityEngine.Playables;      // ����Timeline
using UnityEngine.UI;             // ���ݰ�ť�ر�

public class CutscenePlayer : MonoBehaviour
{
    [SerializeField] private PlayableDirector timeline; // ��Ϊ�գ��Զ������������
    [SerializeField] private Button closeButton;        // ��Ϊ�գ��Զ������������

    private Action onDone;
    private bool finished;

    private void Awake()
    {
        if (!timeline) timeline = GetComponentInChildren<PlayableDirector>(true);
        if (timeline)
        {
            // ��Timeline����̬ͣ�����ܲ���
            timeline.timeUpdateMode = DirectorUpdateMode.UnscaledGameTime;
            timeline.stopped += _ => SafeDone();
        }

        if (!closeButton) closeButton = GetComponentInChildren<Button>(true);
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
        if (timeline)
        {
            timeline.time = 0;
            timeline.Play();
        }
        // û��Timeline/��ť��Prefab��ͨ���ⲿ SignalDone() ����
    }

    public void SignalDone() => SafeDone();

    private void SafeDone()
    {
        if (finished) return;
        finished = true;

        var cb = onDone;
        onDone = null;
        try { cb?.Invoke(); }
        finally { Destroy(gameObject); } // �������������ʵ��
    }
}
