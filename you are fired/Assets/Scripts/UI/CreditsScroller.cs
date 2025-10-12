using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CreditsScroller : MonoBehaviour
{
    [Header("Scroll Settings")]
    public float scrollSpeed = 50f; // �����ٶȣ�����/�룩
    public float resetDelay = 2f;   // ������ȴ������ٻص���㣨��Ҫѭ����

    private RectTransform rectTransform;
    private float startY;
    private float endY;
    private bool reachedEnd = false;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        startY = rectTransform.anchoredPosition.y;

        // ��������յ㣺���ݸ߶� - ���Ӵ��ڸ߶�
        float contentHeight = rectTransform.rect.height;
        float viewportHeight = transform.parent.GetComponent<RectTransform>().rect.height;
        endY = contentHeight - viewportHeight;
    }

    void Update()
    {
        if (!reachedEnd)
        {
            rectTransform.anchoredPosition += Vector2.up * scrollSpeed * Time.deltaTime;

            if (rectTransform.anchoredPosition.y >= endY)
            {
                reachedEnd = true;
                Invoke(nameof(ResetCredits), resetDelay);
            }
        }
    }

    void ResetCredits()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }

}
