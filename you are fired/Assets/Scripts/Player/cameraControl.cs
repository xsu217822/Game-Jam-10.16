using UnityEngine;

public class cameraControl : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private Vector3 offset = new Vector3(0, 0, -10);
    [SerializeField] private float smoothSpeed = 5f;

    void Start()
    {
        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p) player = p.transform;
        }
    }

    void LateUpdate()
    {
        if (player == null) return;

        Vector3 targetPosition = player.position + offset;
        transform.position = Vector3.Lerp(transform.position, targetPosition, smoothSpeed * Time.deltaTime);
    }

    // Call this to set the camera to render a specific canvas
    public void SetCameraToCanvas(Canvas canvas)
    {
        if (canvas == null) return;

        // For World Space canvas, set its worldCamera to this camera
        if (canvas.renderMode == RenderMode.WorldSpace || canvas.renderMode == RenderMode.ScreenSpaceCamera)
        {
            canvas.worldCamera = GetComponent<Camera>();
        }
        // For ScreenSpaceOverlay, no camera assignment is needed
    }
}
