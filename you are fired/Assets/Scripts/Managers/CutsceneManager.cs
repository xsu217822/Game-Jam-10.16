    // Assets/Scripts/CutsceneManager.cs
using System.Collections;          // ← 必须有：协程用到非泛型 IEnumerator
using UnityEngine;
using UnityEngine.InputSystem;     // 支持新输入系统（可选）

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
        try
        {
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
                    if (allowSkip && (Input.anyKeyDown ||
                        (Keyboard.current != null && Keyboard.current.anyKey.wasPressedThisFrame)))
                    {
                        cp.SignalDone();
                    }
                    yield return null;
                }
            }
        }
        finally
        {
            // 恢复时间（防止异常导致卡死在暂停态）
            Time.timeScale = oldScale;
        }
    }
}
