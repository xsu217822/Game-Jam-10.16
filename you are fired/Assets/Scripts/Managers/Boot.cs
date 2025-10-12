using UnityEngine;
using UnityEngine.SceneManagement;

public class Boot : MonoBehaviour
{
    [SerializeField] private GameObject systemRootPrefab;

    private void Awake()
    {
        // ���գ�����û�� SystemRoot���ʹ���һ��
        if (FindObjectOfType<GameManager>() == null)
        {
            var root = Instantiate(systemRootPrefab);
            DontDestroyOnLoad(root);
        }

        SceneManager.LoadScene("MainMenu");
    }
}

