using UnityEngine;
using UnityEngine.UI;

public class EXPbar : MonoBehaviour
{
    [SerializeField] private Slider expSlider;
    [SerializeField] private Player player;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = FindObjectOfType<Player>();
        if (expSlider == null)
            expSlider = GetComponent<Slider>();
    }

    // Update is called once per frame
    void Update()
    {
        if (player != null && expSlider != null)
        {
            // ÿ100����ֵ��һ������ǰ�ȼ��������
            expSlider.maxValue = player.Level * 100;
            expSlider.value = player.Exp;
        }
    }
}
