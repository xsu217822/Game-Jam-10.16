using UnityEngine;

public class ExplosionFX : MonoBehaviour
{
    public float duration = 0.25f;
    public float maxScale = 1.5f;

    private float timer;

    private void Update()
    {
        timer += Time.deltaTime;
        float t = timer / duration;

        transform.localScale = Vector3.one * Mathf.Lerp(0.2f, maxScale, t);

        if (t >= 1f)
            Destroy(gameObject);
    }
}
