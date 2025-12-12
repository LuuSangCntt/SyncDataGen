using UnityEngine;

public class RandomPositionAndRotation : MonoBehaviour
{
    public Vector3 originPosition; // Vị trí gốc để đặt vật thể quanh đó
    public float radius = 10f; // Bán kính 10m quanh vị trí gốc
    public float fixedHeight; // Chiều cao cố định (trục Y)
    public int intervalMilliseconds = 20; // Khoảng thời gian cập nhật vị trí và góc quay (ms)

    private float timer;

    void Start()
    {
        if (originPosition == Vector3.zero)
        {
            originPosition = transform.position; // Nếu không gán vị trí gốc, lấy vị trí hiện tại của vật thể
        }
        fixedHeight = originPosition.y; // Giữ chiều cao cố định bằng chiều cao vị trí gốc
        timer = 0f;
        SetRandomPositionAndRotation();
    }

    void Update()
    {
        timer += Time.deltaTime * 1000f; // Chuyển deltaTime sang ms
        if (timer >= intervalMilliseconds)
        {
            SetRandomPositionAndRotation();
            timer = 0f;
        }
    }

    void SetRandomPositionAndRotation()
    {
        // Tạo góc ngẫu nhiên trong phạm vi 2π (radian)
        float angle = Random.Range(0f, 2f * Mathf.PI);
        // Tính vị trí mới trên mặt phẳng xz trong bán kính 10m quanh originPosition
        float x = originPosition.x + Mathf.Cos(angle) * radius;
        float z = originPosition.z + Mathf.Sin(angle) * radius;
        // Cập nhật vị trí với chiều cao cố định
        transform.position = new Vector3(x, fixedHeight, z);

        // Tạo góc quay y ngẫu nhiên từ 0 đến 360 độ
        float yRotation = Random.Range(0f, 360f);
        transform.rotation = Quaternion.Euler(0f, yRotation, 0f);
    }
}
