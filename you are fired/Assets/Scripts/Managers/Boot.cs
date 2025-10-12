using UnityEngine;
using UnityEngine.SceneManagement;

public class Boot : MonoBehaviour
{
    [SerializeField] private GameObject systemRootPrefab;

    private void Awake()
    {
        // 保险：若还没有 SystemRoot，就创建一个
        if (FindObjectOfType<GameManager>() == null)
        {
            var root = Instantiate(systemRootPrefab);
            DontDestroyOnLoad(root);
        }

        SceneManager.LoadScene("MainMenu");
    }
}

