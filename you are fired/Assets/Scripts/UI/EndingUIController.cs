using UnityEngine;
using UnityEngine.UI;
using System;

public class EndingUIController : MonoBehaviour
{
    [SerializeField] private Button closeButton; // ������/�������˵���֮��

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

    // ����Ƕ��������Զ��رգ�Ҳ���ڶ����¼���ֱ�ӵ��ã�
    public void CloseFromTimeline() => onClose?.Invoke();
}

