using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CreditsScroller : MonoBehaviour
{
    [Header("Scroll Settings")]
    public float scrollSpeed = 50f; // 滚动速度（像素/秒）
    public float resetDelay = 2f;   // 到顶后等待几秒再回到起点（如要循环）

    private RectTransform rectTransform;
    private float startY;
    private float endY;
    private bool reachedEnd = false;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        startY = rectTransform.anchoredPosition.y;

        // 计算滚动终点：内容高度 - 可视窗口高度
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
