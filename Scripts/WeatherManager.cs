using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.Rendering.HighDefinition;

public class WeatherManager : MonoBehaviour
{
    // --- 1. Singleton Pattern ---

    public static WeatherManager Instance { get; private set; }

    [Header("Ánh sáng Directional")]
    [Tooltip("Directional Light trong scene dùng để mô phỏng mặt trời.")]
    [SerializeField]
    private HDAdditionalLightData m_directionalLightHDData;

    [SerializeField]
    private Light m_directionalLight;

    [Header("Điều khiển Độ sáng Ngẫu nhiên")]
    [Tooltip("Khoảng Min/Max (Hệ số 0.0 đến 1.0) cho việc random độ sáng.")]
    [SerializeField]
    private Vector2 m_randomRange = new Vector2(0.5f, 1.0f); // Mặc định từ 50% đến 100% cường độ gốc

    private float m_originalLightIntensity;

    private void Awake()
    {
        // Khởi tạo Singleton
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
            InitializeLightData();
        }
    }

    private void InitializeLightData()
    {
        if (m_directionalLightHDData != null)
        {
            // Lấy cường độ gốc của Directional Light
            //m_originalLightIntensity = m_directionalLightHDData.intensity;
            m_originalLightIntensity = m_directionalLight.intensity;
            Debug.Log($"WeatherManager: Cường độ ánh sáng gốc được lưu trữ: {m_originalLightIntensity}");
        }
        else
        {
            Debug.LogError("Directional Light HD Data chưa được gán trong Inspector!");
        }
    }

    // --- 2. Hàm Điều khiển Độ sáng Cố định ---

    /// <summary>
    /// Điều chỉnh độ sáng của scene bằng cách thay đổi cường độ (Intensity) của Directional Light.
    /// </summary>
    /// <param name="brightnessFactor">Hệ số điều chỉnh độ sáng (0.0f đến 1.0f). 
    /// 1.0f = cường độ gốc, 0.0f = tối hoàn toàn.</param>
    public void SetSceneBrightness(float brightnessFactor)
    {
        if (m_directionalLightHDData == null)
        {
            Debug.LogError("Không thể điều chỉnh độ sáng: Directional Light HD Data chưa được gán.");
            return;
        }

        brightnessFactor = Mathf.Clamp01(brightnessFactor);
        float newIntensity = m_originalLightIntensity * brightnessFactor;
        //m_directionalLightHDData.intensity = newIntensity;
        m_directionalLight.intensity = newIntensity; 

        Debug.Log($"Đã điều chỉnh độ sáng scene. Hệ số: {brightnessFactor}, Cường độ mới: {newIntensity}");
    }

    // --- 3. Hàm Điều khiển Độ sáng Ngẫu nhiên (MỚI) ---

    /// <summary>
    /// Điều chỉnh độ sáng của scene ngẫu nhiên trong khoảng đã định nghĩa.
    /// </summary>
    public void SetRandomSceneBrightness()
    {
        if (m_directionalLightHDData == null)
        {
            Debug.LogError("Không thể điều chỉnh độ sáng: Directional Light HD Data chưa được gán.");
            return;
        }

        // 1. Lấy một hệ số ngẫu nhiên trong khoảng [Min, Max] đã định.
        float randomFactor = Random.Range(m_randomRange.x, m_randomRange.y);

        // 2. Đảm bảo hệ số vẫn nằm trong khoảng [0, 1] tổng thể.
        randomFactor = Mathf.Clamp01(randomFactor);

        // 3. Tính toán cường độ mới.
        //float newIntensity = m_originalLightIntensity * randomFactor;
        float newIntensity = Random.Range(ParameterPack.Instance.LightIntensity_min, ParameterPack.Instance.LightIntensity_max);
        float newTemperature = Random.Range(ParameterPack.Instance.Temperature_min, ParameterPack.Instance.Temperature_max);

        // 4. Gán cường độ mới cho Directional Light.
        //m_directionalLightHDData.intensity = newIntensity;
        m_directionalLight.intensity = newIntensity;
        m_directionalLight.colorTemperature = newTemperature;
        //m_directionalLight.intensity = newIntensity;

        //Debug.Log($"Đã điều chỉnh độ sáng ngẫu nhiên. Hệ số (random): {randomFactor}, Cường độ mới: {newIntensity}");
    }

    /// <summary>
    /// Reset độ sáng về cường độ gốc đã lưu.
    /// </summary>
    public void ResetBrightness()
    {
        SetSceneBrightness(1.0f);
    }
}