using UnityEngine;

public class TopCameraSetup : MonoBehaviour
{
    [Header("Camera dưới (Main Camera)")]
    [SerializeField] private Camera mainCamera;   // kéo MainCamera vào đây

    private Camera topCamera;

    private void Awake()
    {
        topCamera = GetComponent<Camera>();

        // nếu chưa gán thì tự tìm Main Camera theo tag
        if (mainCamera == null)
            mainCamera = Camera.main;
    }

    private void Start()
    {
        if (mainCamera == null || topCamera == null)
            return;

        // Lấy viewport rect của MainCamera
        Rect mainRect = mainCamera.rect;

        // Tính rect cho TopCamera
        float x = mainRect.x;
        float width = mainRect.width;

        float y = mainRect.y + mainRect.height;   // phần bắt đầu của top
        float height = 1f - y;                    // phần còn lại

        if (height < 0) height = 0;

        // Gán rect mới
        topCamera.rect = new Rect(x, y, width, height);

        // (OPTIONAL) đồng bộ orthographic size nếu muốn world scale giống nhau
        if (mainCamera.orthographic && topCamera.orthographic)
        {
            topCamera.orthographicSize = mainCamera.orthographicSize;
        }
    }
}
