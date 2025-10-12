using UnityEngine;
using System;

public class UIManager : MonoBehaviour
{
    private GameObject currentEndingUI;
    private Action onEndingClosed;

    public void ShowEndingUI(GameObject uiPrefab, Action onClosed)
    {
        if (currentEndingUI != null) return;
        onEndingClosed = onClosed;

        // 实例到场景里（一般是一个带 Canvas 的预制体）
        currentEndingUI = Instantiate(uiPrefab);
        // 暂停时间轴，结局 UI 播完再恢复
        Time.timeScale = 0f;

        // 让 UI 自己来触发关闭（按钮事件里调 End）
        var ctrl = currentEndingUI.GetComponent<EndingUIController>();
        if (ctrl != null) ctrl.Bind(OnUserCloseEnding);
    }

    private void OnUserCloseEnding()
    {
        if (currentEndingUI != null) Destroy(currentEndingUI);
        currentEndingUI = null;
        Time.timeScale = 1f;
        onEndingClosed?.Invoke();
        onEndingClosed = null;
    }
}
