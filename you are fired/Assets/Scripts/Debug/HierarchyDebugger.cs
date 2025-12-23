using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 调试脚本 - 用于检查 Hierarchy 结构和 UI 组件
/// 将此脚本添加到任何对象上运行即可诊断问题
/// </summary>
public class HierarchyDebugger : MonoBehaviour
{
    void Start()
    {
        Debug.Log("\n========== Hierarchy Debugger ==========");
        PrintCanvasStructure();
        PrintAllButtons();
        PrintAllSliders();
        PrintAllToggles();
        PrintGameObjectByName("CreditsPanel");
        PrintGameObjectByName("ExitButton");
        Debug.Log("========== End Debugger ==========\n");
    }

    private void PrintCanvasStructure()
    {
        Debug.Log("\n[Debugger] === Canvas 结构 ===");
        Canvas canvas = FindAnyObjectByType<Canvas>();
        
        if (canvas == null)
        {
            Debug.LogError("[Debugger] ✗ 未找到 Canvas");
            return;
        }

        Debug.Log("[Debugger] Canvas: " + canvas.name);
        PrintTransformTree(canvas.transform, 1);
    }

    private void PrintTransformTree(Transform transform, int depth)
    {
        string indent = new string(' ', depth * 2);
        string status = transform.gameObject.activeSelf ? "✓" : "✗ (Inactive)";
        Debug.Log($"{indent}{transform.name} [{status}]");

        foreach (Transform child in transform)
        {
            PrintTransformTree(child, depth + 1);
        }
    }

    private void PrintAllButtons()
    {
        Debug.Log("\n[Debugger] === 所有 Button 组件 ===");
        Button[] buttons = FindObjectsByType<Button>(FindObjectsSortMode.None);
        
        if (buttons.Length == 0)
        {
            Debug.LogWarning("[Debugger] ⚠ 未找到任何 Button");
            return;
        }

        for (int i = 0; i < buttons.Length; i++)
        {
            string status = buttons[i].gameObject.activeSelf ? "✓" : "✗ (Inactive)";
            Debug.Log($"[Debugger] [{i}] {buttons[i].name} - {buttons[i].gameObject.transform.parent?.name ?? "Root"} [{status}]");
        }
    }

    private void PrintAllSliders()
    {
        Debug.Log("\n[Debugger] === 所有 Slider 组件 ===");
        Slider[] sliders = FindObjectsByType<Slider>(FindObjectsSortMode.None);
        
        if (sliders.Length == 0)
        {
            Debug.LogWarning("[Debugger] ⚠ 未找到任何 Slider");
            return;
        }

        for (int i = 0; i < sliders.Length; i++)
        {
            string status = sliders[i].gameObject.activeSelf ? "✓" : "✗ (Inactive)";
            Debug.Log($"[Debugger] [{i}] {sliders[i].name} - {sliders[i].gameObject.transform.parent?.name ?? "Root"} [{status}]");
        }
    }

    private void PrintAllToggles()
    {
        Debug.Log("\n[Debugger] === 所有 Toggle 组件 ===");
        Toggle[] toggles = FindObjectsByType<Toggle>(FindObjectsSortMode.None);
        
        if (toggles.Length == 0)
        {
            Debug.LogWarning("[Debugger] ⚠ 未找到任何 Toggle");
            return;
        }

        for (int i = 0; i < toggles.Length; i++)
        {
            string status = toggles[i].gameObject.activeSelf ? "✓" : "✗ (Inactive)";
            Debug.Log($"[Debugger] [{i}] {toggles[i].name} - {toggles[i].gameObject.transform.parent?.name ?? "Root"} [{status}]");
        }
    }

    private void PrintGameObjectByName(string name)
    {
        Debug.Log($"\n[Debugger] === 查找 '{name}' ===");
        
        // 方法 1: GameObject.Find
        GameObject found1 = GameObject.Find(name);
        if (found1 != null)
        {
            Debug.Log($"[Debugger] ✓ GameObject.Find() 找到: {found1.name} [Active: {found1.activeSelf}]");
            Debug.Log($"  Path: {GetGameObjectPath(found1)}");
            return;
        }

        // 方法 2: Canvas 下的所有对象
        Canvas canvas = FindAnyObjectByType<Canvas>();
        if (canvas != null)
        {
            Transform[] allTransforms = canvas.GetComponentsInChildren<Transform>(true);
            foreach (Transform t in allTransforms)
            {
                if (t.name == name)
                {
                    Debug.Log($"[Debugger] ✓ Canvas.GetComponentsInChildren() 找到: {t.name} [Active: {t.gameObject.activeSelf}]");
                    Debug.Log($"  Path: {GetGameObjectPath(t.gameObject)}");
                    return;
                }
            }
        }

        // 方法 3: 模糊匹配
        Canvas canvas2 = FindAnyObjectByType<Canvas>();
        if (canvas2 != null)
        {
            Transform[] allTransforms = canvas2.GetComponentsInChildren<Transform>(true);
            foreach (Transform t in allTransforms)
            {
                if (t.name.Contains(name))
                {
                    Debug.Log($"[Debugger] ◐ 模糊匹配找到: {t.name} [Active: {t.gameObject.activeSelf}]");
                    Debug.Log($"  Path: {GetGameObjectPath(t.gameObject)}");
                    return;
                }
            }
        }

        Debug.LogWarning($"[Debugger] ✗ 未找到 '{name}'");
    }

    private string GetGameObjectPath(GameObject go)
    {
        string path = go.name;
        Transform current = go.transform.parent;
        
        while (current != null)
        {
            path = current.name + "/" + path;
            current = current.parent;
        }
        
        return path;
    }
}
