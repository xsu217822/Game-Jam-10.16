using UnityEngine;
using UnityEngine.UI;
using System;

public class EndingUIController : MonoBehaviour
{
    [SerializeField] private Button closeButton; // “继续/返回主菜单”之类

    private Action onClose;

    public void Bind(Action onClose)
    {
        this.onClose = onClose;
        if (closeButton != null)
        {
            closeButton.onClick.RemoveAllListeners();
            closeButton.onClick.AddListener(() => this.onClose?.Invoke());
        }
    }

    // 如果是动画结束自动关闭，也可在动画事件里直接调用：
    public void CloseFromTimeline() => onClose?.Invoke();
}

