using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(BoxCollider))]
public class RandomPositionSetter : MonoBehaviour
{
    public static RandomPositionSetter Ins;
    [SerializeField] bool Enable = true;

    [Tooltip("Lấy theo vị trí của GameObject này")]
    public Vector3 CenterPoint;
    [Tooltip("Tính toán để vùng sinh đối tượng nằm trong BoxCollider đi kèm")]
    public float Range = 500;
    public float RangeY = 200;

    float MinX = -10;
    float MaxX = 10;
    float MinZ = -10;
    float MaxZ = 10;

    [Tooltip("-1 nghĩa là không áp dụng, còn >=0 là áp dụng, vị trí đối tượng bắt buộc cao hơn địa hình ít nhất khoảng được thiết lập")]
    public float HeightOffsetOverGround = -1;

    //public float DistanceFactorMin = 5;
    //public float DistanceFactorMax = 30;
    // Start is called before the first frame update
    void Start()
    {
        Ins = this;
        InitValues();
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void InitValues()
    {
        //Read config
        //DistanceFactorMin = GeneratorSettings.Instance.DistanceFactorMin;
        //DistanceFactorMax = GeneratorSettings.Instance.DistanceFactorMax;

        CenterPoint = transform.position;
        BoxCollider boxCollider = GetComponent<BoxCollider>();
        //Debug.Log("collider size: " + boxCollider.size);
        Debug.Log("Init Spawn Area" + boxCollider.size);
        Range = boxCollider.size.x / 2;
        RangeY = boxCollider.size.y / 2;

        MinX = CenterPoint.x - Range;
        MaxX = CenterPoint.x + Range;
        MinZ = CenterPoint.z - Range;
        MaxZ = CenterPoint.z + Range;
    }

    /// <summary>
    /// Đặt vị trí ngẫu nhiên cho đối tượng trong khu vực đã thiết lập
    /// </summary>
    /// <param name="obj"></param>
    public void SetRandomPosition(GameObject obj, Model3D modelInfo)
    {
        //Debug.Log("Set object random position");
        if (obj == null)
        {
            return;
        }
        if (Enable == false)
        {
            return;
        }

        //Thiết lập một số thuộc tính
        float HeightMin = 0f; // Chiều cao tối thiểu cho các mô hình khác
        float HeightMax = 1f; // Chiều cao tối đa cho các mô hình khác
        bool RotateOnlyYAxis = true; // Chỉ xoay theo trục Y cho các mô hình khác
        if (modelInfo.Label.StartsWith("Air."))
        {
            HeightMin = 150f; // Đặt chiều cao tối thiểu cho mô hình Air
            HeightMax = 400f; // Đặt chiều cao tối đa cho mô hình Air
            RotateOnlyYAxis = false; // Không xoay chỉ theo trục Y
        }
        else
        {
            HeightMin = 0f; // Chiều cao tối thiểu cho các mô hình khác
            HeightMax = 1f; // Chiều cao tối đa cho các mô hình khác
            RotateOnlyYAxis = true; // Chỉ xoay theo trục Y cho các mô hình khác
        }


        //Random position
        float x = Random.Range(MinX, MaxX);
        float z = Random.Range(MinZ, MaxZ);
        float y = obj.transform.position.y;


        if (obj.name.StartsWith("Sea"))
        {
            //Với tàu thuyền thì ko cần thiết lập độ cao, nó tự động trên mặt biển
        }
        else
        {
            float terrainHeight = GetTerrainHeight(new Vector3(x, 3000, z));
            if (terrainHeight == -9999)
            {
                Debug.LogError("Terrain height not found at " + new Vector3(x, 3000, z));
            }
            else { y = terrainHeight + Random.Range(HeightMin, HeightMax); }
        }

        obj.transform.position = new Vector3(x, y, z);

        //Random rotation
        if (RotateOnlyYAxis)
        {
            obj.transform.rotation = Quaternion.Euler(0, Random.Range(0, 360), 0);
        }
        else
        {
            obj.transform.rotation = Quaternion.Euler(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360));
        }
    }

    /// <summary>
    /// Đặt camera tại vị trí cách vật thể một khoảng ngẫu nhiên
    /// Góc tà và phương vị từ vật thể đến camera cũng ngẫu nhiên
    /// </summary>
    /// <param name="_cam"></param>
    /// <param name="_target"></param>
    public void SetCameraRandomPosition(Transform _cam, Transform _target, float targetRatio, bool highAngleOnly)
    {
        //Debug.Log("Try set random camera position1 ");
        if (_cam == null || _target == null)
        {
            Debug.LogError("Camera or target is null");
            return;
        }
        if (Enable == false)
        {
            Debug.Log("Not enable");
            return;
        }
        targetGroup = _target.gameObject;
        if (targetGroup == null) return;
        //Debug.Log("Try set random camera position");
        // 1. Tính toán Bounding Box tổng hợp từ tất cả các Renderer con
        Bounds combinedBounds = CalculateCombinedBounds(targetGroup);
        Vector3 center = combinedBounds.center;
        float objectSize = combinedBounds.size.magnitude/4; // Đường kính bao quát

        // 2. Tính toán khoảng cách dựa trên FOV và tỷ lệ diện tích
        // Diện tích tỷ lệ thuận với bình phương kích thước tuyến tính
        float linearScale = Mathf.Sqrt(targetRatio);

        // Công thức: distance = (Kích thước vật) / (Tỷ lệ chiếm dụng * 2 * tan(FOV/2))
        float fovRad = Camera.main.fieldOfView * Mathf.Deg2Rad;
        float distance = objectSize / (linearScale * 2f * Mathf.Tan(fovRad / 2f));

        // 3. Xác định vị trí Camera
        Vector3 randomDir = Random.onUnitSphere;
        if (highAngleOnly && randomDir.y < 0) randomDir.y *= -1; // Luôn nhìn từ trên xuống

        // Đặt camera ở khoảng cách đã tính so với tâm vật thể
        _cam.position = center + randomDir.normalized * distance;

        // 4. Luôn nhìn vào tâm vật thể
        _cam.LookAt(center);

        Debug.Log($"Đã Thiết lập Camera. Khoảng cách: {distance}, Tỉ lệ mục tiêu: {targetRatio}");
    }
    /// </summary>
    /// <param name="cam">Transform của Camera.</param>
    /// <param name="obj">Transform của Vật thể mục tiêu.</param>
    /// <param name="targetRatio">Tỷ lệ diện tích mong muốn (ví dụ: 0.2).</param>
    /// <param name="currentRatio">Tỷ lệ diện tích hiện tại của vật thể trên màn hình.</param>
    public void AdjustCamera(Transform cam, Transform obj, float targetRatio, float currentRatio)
    {
        //Debug.Log("Căn chỉnh lại vị trí camera " + currentRatio + " -> " + targetRatio);
        // 1. Lấy thông tin hiện tại
        Vector3 currentPosition = cam.position;
        Vector3 targetPosition = obj.position;
        float currentDist = Vector3.Distance(currentPosition, targetPosition);

        // Cần đảm bảo các giá trị không bị chia cho 0 hoặc quá nhỏ
        if (targetRatio <= 0.0001f || currentRatio <= 0.0001f)
        {
            Debug.LogError($"TargetRatio ({targetRatio}) hoặc CurrentRatio ({currentRatio}) quá nhỏ, không thể điều chỉnh.");
            return;
        }

        // 2. Tính khoảng cách mới (d_new)
        // d_new = d_current * sqrt(currentRatio / targetRatio)   
                float targetDist = currentDist * Mathf.Sqrt(currentRatio / targetRatio);
        //Thêm tỉ lệ 0.95 cho nó hội tụ nhanh hơn
        if (currentRatio < targetRatio)
        {
            targetDist *= 0.95f;
        }
        else
        {
            targetDist *= 1.05f;
        }

        // 3. Xác định hướng di chuyển
        // Lấy hướng vector từ mục tiêu đến camera (hướng hiện tại)
        Vector3 currentDirection = (currentPosition - targetPosition).normalized;

        // 4. Cập nhật vị trí camera
        // Vị trí mới = Vị trí mục tiêu + Hướng hiện tại * Khoảng cách mới
        Vector3 newPosition = targetPosition + currentDirection * targetDist;

        cam.position = newPosition;

        // 5. Đảm bảo camera vẫn nhìn vào vật thể (nếu cần)
        // Nếu camera đã được LookAt trước đó, việc giữ nguyên hướng này là hợp lý.
        cam.LookAt(obj.position); 

        //Nếu tỉ lệ ảnh nhỏ hoặc rất lớn, có thể điều chỉnh góc quay camera một chút để vật thể không nằm chính giữa ảnh
        if (targetRatio <= 0.3f  || targetRatio >= 0.7f)
        {
            cam.Rotate(Random.Range(-5, 5), Random.Range(-5, 5), 0);
        }

        Debug.Log($"Camera được điều chỉnh từ khoảng cách {currentDist} thành {targetDist}. Tỷ lệ mới sẽ gần {targetRatio} hơn.");
    }



    private Bounds CalculateCombinedBounds(GameObject parent)
    {
        Renderer[] renderers = parent.GetComponentsInChildren<Renderer>();
        if (renderers.Length == 0) return new Bounds(parent.transform.position, Vector3.zero);

        Bounds b = renderers[0].bounds;
        foreach (Renderer r in renderers)
        {
            b.Encapsulate(r.bounds);
        }
        //Debug.Log("Bound " + b.center);
        //Debug.Log("Bound size: " + b.size);
        return b;
    }

    public GameObject targetGroup;
    // Vẽ Gizmos để dễ hình dung tâm vật thể trong Editor
    void OnDrawGizmosSelected()
    {
        if (targetGroup != null)
        {
            Bounds b = CalculateCombinedBounds(targetGroup);
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(b.center, b.size);
        }
    }
    private float GetTerrainHeight(Vector3 pos)
    {
        pos.y = 5000;
        int terrainLayer = LayerMask.NameToLayer("Terrain");
        LayerMask layerMask = LayerMask.GetMask("Terrain");

        if (Physics.Raycast(pos, Vector3.down, out RaycastHit hit, Mathf.Infinity, layerMask))
        {
            //Debug.Log("Hit : " + hit.collider.gameObject.name);
            if (hit.collider.gameObject.TryGetComponent<TerrainCollider>(out TerrainCollider terraincollider))
            {
                return hit.point.y;
            }
        }
        return -9999;
    }

    public Vector3 RandomPosition()
    {
        //Random position
        float x = Random.Range(MinX, MaxX);
        float z = Random.Range(MinZ, MaxZ);
        return new Vector3(x, 0, z);
    }

    public Quaternion RandomRotation()
    {
        //Random rotation
        return Quaternion.Euler(0, Random.Range(0, 360), 0);
    }

    /// <summary>
    /// Tính toán khoảng cách camera tối đa và tối thiểu đến vật thể
    /// Sao cho ảnh của vật thể chiếm 5-85% diện tích ảnh
    /// Biết FOV của camera là 90
    /// Vật có box collider bao sát đối tượng
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    private void CaculateMinDistance(GameObject _target, out float minDistance, out float maxDistance)
    {
        minDistance = 0;
        maxDistance = 0;

        if (_target == null)
        {
            Debug.LogError("Target is null");
            return;
        }
        BoxCollider box = _target.GetComponent<BoxCollider>();
        Vector3 size = box.size;
        float matNhoNhat = Mathf.Min(size.x * size.y, size.y * size.z, size.z * size.x) * 0.5f; //Giả định là đối tượng chiếm ít nhất 50% diện tích mặt đó
        float matLonNhat = Mathf.Max(size.x * size.y, size.y * size.z, size.z * size.x) * 0.8f;

        //diện tích ảnh chụp được ở khoảng cách d sẽ tính theo công thức F = 4*d*d (ảnh vuông)
        float tiLeAnhToiThieu = 0.05f;
        float tiLeAnhToiDa = 0.85f;
        maxDistance = Mathf.Sqrt(matNhoNhat / 4 / tiLeAnhToiThieu);
        minDistance = Mathf.Sqrt(matLonNhat / 4 / tiLeAnhToiDa);

        if (maxDistance <= minDistance)
        {
            Debug.LogError("Max < Min");
        }
        Debug.Log(_target.name + ".  Distance:    " + minDistance + " - " + maxDistance);
    }
}
