using UnityEngine;

public class CutsceneManager : MonoBehaviour, ICutsceneService
{
    public IEnumerator PlaySequence(GameObject[] prefabs)
    {
        if (prefabs == null || prefabs.Length == 0) yield break;
        Time.timeScale = 0f;
        for (int i = 0; i < prefabs.Length; i++)
        {
            var go = Instantiate(prefabs[i]);
            var done = false;
            // ������Ĺ���Ԥ���Ϲ�һ�� CutscenePlayer����������� onDone
            var cp = go.GetComponent<CutscenePlayer>();
            if (!cp) cp = go.AddComponent<CutscenePlayer>();
            cp.Play(() => done = true);
            while (!done) yield return null;
        }
        Time.timeScale = 1f;
    }
}
