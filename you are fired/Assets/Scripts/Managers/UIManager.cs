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

        // ʵ���������һ����һ���� Canvas ��Ԥ���壩
        currentEndingUI = Instantiate(uiPrefab);
        // ��ͣʱ���ᣬ��� UI �����ٻָ�
        Time.timeScale = 0f;

        // �� UI �Լ��������رգ���ť�¼���� End��
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
