using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Slider healthSlider;
    [SerializeField] private Player player;

    void Start()
    {
        if (player == null)
            player = FindObjectOfType<Player>();
        if (healthSlider == null)
            healthSlider = GetComponent<Slider>();

        if (player != null && healthSlider != null)
        {
            healthSlider.maxValue = player.MaxHP;
            healthSlider.value = player.HP;
        }
    }

    void Update()
    {
        if (player != null && healthSlider != null)
        {
            healthSlider.value = player.HP;
        }
    }
}
