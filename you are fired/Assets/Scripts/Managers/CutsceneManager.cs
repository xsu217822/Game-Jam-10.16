// Assets/Scripts/CutsceneManager.cs
using System.Collections;          // �� �����У�Э���õ��Ƿ��� IEnumerator
using UnityEngine;

public class CutsceneManager : MonoBehaviour, ICutsceneService
{
    [Header("��������������ѡ��")]
    [SerializeField] private bool allowSkip = false;

    public IEnumerator PlaySequence(GameObject[] prefabs)
    {
        if (prefabs == null || prefabs.Length == 0) yield break;

        // ȫ����ͣ
        float oldScale = Time.timeScale;
        Time.timeScale = 0f;

        for (int i = 0; i < prefabs.Length; i++)
        {
            var go = Instantiate(prefabs[i]);

            // ͳһ�� CutscenePlayer ���ж���ʲôʱ�������
            var cp = go.GetComponent<CutscenePlayer>() ?? go.AddComponent<CutscenePlayer>();

            bool done = false;
            cp.Play(() => done = true);

            // ����һ�β��ꣻ��ѡ�ġ������������
            while (!done)
            {
                if (allowSkip && Input.anyKeyDown)
                {
                    // ֱ��֪ͨ���������CutscenePlayer�����Timeline/��ť��Ҳ��������β��
                    cp.SignalDone();
                }
                yield return null;
            }
        }

        // �ָ�ʱ��
        Time.timeScale = oldScale;
    }
}
