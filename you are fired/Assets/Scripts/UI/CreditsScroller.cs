using UnityEngine;

public class CreditsScroller : MonoBehaviour
{
    [SerializeField] private float scrollSpeed = 50f;
    [SerializeField] private float resetDelay = 2f;

    private RectTransform rectTransform;
    private float endY;
    private bool reachedEnd = false;
    private CreditsUIManager creditsUIManager;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();

        float contentHeight = rectTransform.rect.height;
        float viewportHeight = transform.parent.GetComponent<RectTransform>().rect.height;
        endY = contentHeight - viewportHeight;
        
        creditsUIManager = FindAnyObjectByType<CreditsUIManager>();
    }

    void Update()
    {
        if (!reachedEnd)
        {
            rectTransform.anchoredPosition += Vector2.up * scrollSpeed * Time.deltaTime;

            if (rectTransform.anchoredPosition.y >= endY)
            {
                reachedEnd = true;
                
                if (creditsUIManager != null)
                {
                    creditsUIManager.OnCreditsScrollEnd();
                }
                else
                {
                    creditsUIManager = FindAnyObjectByType<CreditsUIManager>();
                    if (creditsUIManager != null)
                    {
                        creditsUIManager.OnCreditsScrollEnd();
                    }
                }
            }
        }
    }

    void ResetCredits()
    {
        if (creditsUIManager != null)
        {
            creditsUIManager.ExitCredits();
        }
    }
}
