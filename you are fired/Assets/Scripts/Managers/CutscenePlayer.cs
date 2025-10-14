// Assets/Scripts/CutscenePlayer.cs
using System;                     // Action
using UnityEngine;
using UnityEngine.Playables;      // 兼容Timeline
using UnityEngine.UI;             // 兼容按钮关闭

public class CutscenePlayer : MonoBehaviour
{
    [SerializeField] private PlayableDirector timeline; // 可为空：自动在子物体查找
    [SerializeField] private Button closeButton;        // 可为空：自动在子物体查找

    private Action onDone;
    private bool finished;

    private void Awake()
    {
        if (!timeline) timeline = GetComponentInChildren<PlayableDirector>(true);
        if (timeline)
        {
            // 让Timeline在暂停态下仍能播放
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

    // 由 CutsceneManager 调用
    public void Play(Action done)
    {
        onDone = done;
        gameObject.SetActive(true);
        if (timeline)
        {
            timeline.time = 0;
            timeline.Play();
        }
        // 没有Timeline/按钮的Prefab可通过外部 SignalDone() 结束
    }

    public void SignalDone() => SafeDone();

    private void SafeDone()
    {
        if (finished) return;
        finished = true;

        var cb = onDone;
        onDone = null;
        try { cb?.Invoke(); }
        finally { Destroy(gameObject); } // 剧情结束后销毁实例
    }
}
