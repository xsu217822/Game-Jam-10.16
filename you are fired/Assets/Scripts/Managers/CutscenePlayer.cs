// Assets/Scripts/CutscenePlayer.cs
using System;                     // Action
using UnityEngine;
using UnityEngine.Playables;      // 如果用Timeline
using UnityEngine.UI;             // 如果用按钮关闭

public class CutscenePlayer : MonoBehaviour
{
    [SerializeField] private PlayableDirector timeline; // 可为空（不用Timeline）
    [SerializeField] private Button closeButton;        // 可为空（不用按钮）

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

    // 由 CutsceneManager 调用
    public void Play(Action done)
    {
        onDone = done;
        gameObject.SetActive(true);
        if (timeline) timeline.Play();
        // 如果既没有Timeline也没有按钮，你可以在动画事件里调用 SignalDone()
    }

    public void SignalDone() => SafeDone();

    private void SafeDone()
    {
        var cb = onDone;
        onDone = null;
        try { cb?.Invoke(); }
        finally { Destroy(gameObject); } // 播完销毁本段过场实例
    }
}
