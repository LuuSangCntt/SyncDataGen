using UnityEngine;

public class BoundingBoxScreenSize : MonoBehaviour
{
    public Camera mainCamera; // Gán camera chính của bạn vào đây.
    public GameObject targetObject; // Đối tượng cần tính bounding box.

    void Update()
    {
        if (targetObject == null || mainCamera == null) return;

        // Lấy Renderer của đối tượng.
        Renderer renderer = targetObject.GetComponent<Renderer>();
        if (renderer == null) return;

        // Lấy các góc của bounding box trong không gian thế giới.
        Bounds bounds = renderer.bounds;
        Vector3[] worldCorners = new Vector3[8];
        worldCorners[0] = bounds.min;
        worldCorners[1] = new Vector3(bounds.max.x, bounds.min.y, bounds.min.z);
        worldCorners[2] = new Vector3(bounds.min.x, bounds.max.y, bounds.min.z);
        worldCorners[3] = new Vector3(bounds.min.x, bounds.min.y, bounds.max.z);
        worldCorners[4] = new Vector3(bounds.max.x, bounds.max.y, bounds.min.z);
        worldCorners[5] = new Vector3(bounds.max.x, bounds.min.y, bounds.max.z);
        worldCorners[6] = new Vector3(bounds.min.x, bounds.max.y, bounds.max.z);
        worldCorners[7] = bounds.max;

        // Chuyển đổi các góc từ không gian thế giới sang không gian màn hình.
        Vector2 minScreen = new Vector2(float.MaxValue, float.MaxValue);
        Vector2 maxScreen = new Vector2(float.MinValue, float.MinValue);
        foreach (var corner in worldCorners)
        {
            Vector3 screenPoint = mainCamera.WorldToScreenPoint(corner);
            minScreen = Vector2.Min(minScreen, screenPoint);
            maxScreen = Vector2.Max(maxScreen, screenPoint);
        }

        // Tính chiều rộng và chiều cao trên màn hình.
        float width = maxScreen.x - minScreen.x;
        float height = maxScreen.y - minScreen.y;

        // Hiển thị kết quả.
        Debug.Log($"Bounding Box Screen Width: {width}, Height: {height}");
    }
}
