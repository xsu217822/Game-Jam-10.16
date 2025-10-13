// Assets/Scripts/CutsceneManager.cs
using System.Collections;          // ← 必须有：协程用到非泛型 IEnumerator
using UnityEngine;

public class CutsceneManager : MonoBehaviour, ICutsceneService
{
    [Header("允许按键跳过（可选）")]
    [SerializeField] private bool allowSkip = false;

    public IEnumerator PlaySequence(GameObject[] prefabs)
    {
        if (prefabs == null || prefabs.Length == 0) yield break;

        // 全局暂停
        float oldScale = Time.timeScale;
        Time.timeScale = 0f;

        for (int i = 0; i < prefabs.Length; i++)
        {
            var go = Instantiate(prefabs[i]);

            // 统一用 CutscenePlayer 来判定“什么时候结束”
            var cp = go.GetComponent<CutscenePlayer>() ?? go.AddComponent<CutscenePlayer>();

            bool done = false;
            cp.Play(() => done = true);

            // 等这一段播完；可选的“任意键跳过”
            while (!done)
            {
                if (allowSkip && Input.anyKeyDown)
                {
                    // 直接通知结束（如果CutscenePlayer里接了Timeline/按钮，也会正常收尾）
                    cp.SignalDone();
                }
                yield return null;
            }
        }

        // 恢复时间
        Time.timeScale = oldScale;
    }
}
