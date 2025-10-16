// Assets/Scripts/CutscenePlayer.cs
using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

public class CutscenePlayer : MonoBehaviour
{
    [SerializeField] private PlayableDirector timeline;
    [SerializeField] private Button closeButton;

    private Action onDone;
    private bool finished;

    private void Awake()
    {
        if (!timeline) timeline = GetComponentInChildren<PlayableDirector>(true);
        if (timeline)
        {
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

    public void Play(Action done)
    {
        onDone = done;
        gameObject.SetActive(true);
        if (timeline)
        {
            timeline.time = 0;
            timeline.Play();
        }
    }

    public void SignalDone() => SafeDone();

    private void SafeDone()
    {
        if (finished) return;
        finished = true;
        var cb = onDone; onDone = null;
        try { cb?.Invoke(); }
        finally { Destroy(gameObject); }
    }
}
