    // Assets/Scripts/CutsceneManager.cs
using System.Collections;          // �� �����У�Э���õ��Ƿ��� IEnumerator
using UnityEngine;
using UnityEngine.InputSystem;     // ֧��������ϵͳ����ѡ��

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
        try
        {
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
            // �ָ�ʱ�䣨��ֹ�쳣���¿�������̬ͣ��
            Time.timeScale = oldScale;
        }
    }
}
