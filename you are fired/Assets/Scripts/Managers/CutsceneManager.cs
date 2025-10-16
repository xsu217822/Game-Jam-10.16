// Assets/Scripts/CutsceneManager.cs
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class CutsceneManager : MonoBehaviour, ICutsceneService
{
    [Header("允许按键跳过")]
    [SerializeField] private bool allowSkip = true;

    [Header("简易页式剧情UI（可不填：会自动生成临时Canvas）")]
    [SerializeField] private Canvas prefabCanvas;    // 可空
    [SerializeField] private Image prefabImage;      // 可空
    [SerializeField] private Text prefabText;        // 可空
    [SerializeField] private float fadeSeconds = 0.35f;
    [SerializeField] private float typewriterCharsPerSec = 28f;

    public IEnumerator PlaySequence(GameObject[] prefabs)
    {
        if (prefabs == null || prefabs.Length == 0) yield break;
        float oldScale = Time.timeScale; Time.timeScale = 0f;
        try
        {
            for (int i = 0; i < prefabs.Length; i++)
            {
                var go = Instantiate(prefabs[i]);
                var cp = go.GetComponent<CutscenePlayer>() ?? go.AddComponent<CutscenePlayer>();

                bool done = false;
                cp.Play(() => done = true);

                while (!done)
                {
                    if (allowSkip && (Input.anyKeyDown ||
                        (Keyboard.current != null && Keyboard.current.anyKey.wasPressedThisFrame)))
                        cp.SignalDone();
                    yield return null;
                }
            }
        }
        finally { Time.timeScale = oldScale; }
    }

    public IEnumerator PlayPages(LevelConfig.StoryPage[] pages)
    {
        if (pages == null || pages.Length == 0) yield break;

        float oldScale = Time.timeScale; Time.timeScale = 0f;

        Canvas canvas = prefabCanvas ? Instantiate(prefabCanvas) : CreateRuntimeCanvas(out var _);
        Image img = prefabImage ? Instantiate(prefabImage, canvas.transform) : CreateRuntimeImage(canvas.transform);
        Text txt = prefabText ? Instantiate(prefabText, canvas.transform) : CreateRuntimeText(canvas.transform);

        var cg = canvas.GetComponent<CanvasGroup>() ?? canvas.gameObject.AddComponent<CanvasGroup>();
        cg.alpha = 0f;

        try
        {
            for (int i = 0; i < pages.Length; i++)
            {
                var p = pages[i];
                img.sprite = p.image;
                img.gameObject.SetActive(p.image != null);

                txt.text = "";
                string full = p.text ?? "";
                int shown = 0;

                yield return Fade(cg, 0f, 1f, fadeSeconds);

                float t = 0f;
                bool pageDone = false;
                while (!pageDone)
                {
                    t += Time.unscaledDeltaTime;
                    int target = Mathf.Min(full.Length, Mathf.FloorToInt(t * typewriterCharsPerSec));
                    if (target > shown)
                    {
                        txt.text = full.Substring(0, target);
                        shown = target;
                    }

                    bool timeUp = p.holdSeconds > 0 && t >= p.holdSeconds && shown >= full.Length;
                    bool key = allowSkip && (Input.anyKeyDown ||
                                (Keyboard.current != null && Keyboard.current.anyKey.wasPressedThisFrame));
                    if (timeUp || key) pageDone = true;

                    yield return null;
                }

                yield return Fade(cg, 1f, 0f, fadeSeconds);
            }
        }
        finally
        {
            if (canvas) Destroy(canvas.gameObject);
            Time.timeScale = oldScale;
        }
    }

    // === 生成临时UI ===
    private static Canvas CreateRuntimeCanvas(out Image panel)
    {
        var go = new GameObject("StoryCanvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster), typeof(CanvasGroup));
        var c = go.GetComponent<Canvas>(); c.renderMode = RenderMode.ScreenSpaceOverlay;
        var scaler = go.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);

        var panelGO = new GameObject("Panel", typeof(Image));
        panelGO.transform.SetParent(go.transform, false);
        var p = panelGO.GetComponent<Image>();
        p.color = new Color(0f, 0f, 0f, 0.4f);
        var rt = p.rectTransform; rt.anchorMin = Vector2.zero; rt.anchorMax = Vector2.one; rt.offsetMin = rt.offsetMax = Vector2.zero;

        panel = p;
        return c;
    }

    private static Image CreateRuntimeImage(Transform parent)
    {
        var go = new GameObject("StoryImage", typeof(Image));
        go.transform.SetParent(parent, false);
        var img = go.GetComponent<Image>();
        var rt = img.rectTransform;
        rt.anchorMin = new Vector2(0.5f, 0.6f); rt.anchorMax = new Vector2(0.5f, 0.6f);
        rt.sizeDelta = new Vector2(800, 450);
        return img;
    }

    private static Text CreateRuntimeText(Transform parent)
    {
        var go = new GameObject("StoryText", typeof(Text));
        go.transform.SetParent(parent, false);
        var txt = go.GetComponent<Text>();
        txt.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        txt.fontSize = 36;
        txt.alignment = TextAnchor.UpperLeft;
        txt.horizontalOverflow = HorizontalWrapMode.Wrap;
        txt.verticalOverflow = VerticalWrapMode.Truncate;
        var rt = txt.rectTransform;
        rt.anchorMin = new Vector2(0.1f, 0.1f); rt.anchorMax = new Vector2(0.9f, 0.4f);
        rt.offsetMin = rt.offsetMax = Vector2.zero;
        return txt;
    }

    private static IEnumerator Fade(CanvasGroup cg, float a, float b, float sec)
    {
        float t = 0f;
        while (t < sec)
        {
            t += Time.unscaledDeltaTime;
            cg.alpha = Mathf.Lerp(a, b, t / sec);
            yield return null;
        }
        cg.alpha = b;
    }
}
