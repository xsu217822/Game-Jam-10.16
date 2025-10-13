using UnityEngine;
using UnityEngine.UI;
using System;

public class EndingUIController : MonoBehaviour
{
    [SerializeField] private Button closeButton;
    private Action onClose;

    public void Bind(Action onClose)
    {
        this.onClose = onClose;
        if (closeButton)
        {
            closeButton.onClick.RemoveAllListeners();
            closeButton.onClick.AddListener(() => this.onClose?.Invoke());
        }
    }

    // 可从动画事件/Timeline 调
    public void CloseFromTimeline() => onClose?.Invoke();
}

