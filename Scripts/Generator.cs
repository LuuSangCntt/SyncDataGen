using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using UnityEngine.Perception.GroundTruth;
using UnityEngine.Perception.GroundTruth.Labelers;
using UnityEngine.Perception.GroundTruth.LabelManagement;
using UnityEngine.Perception.Settings;

public class Generator : MonoBehaviour
{
    public static Generator Ins;
    [SerializeField] private GameObject objCamera;

    [SerializeField] private ParameterPack parameters;

    // Biến điều khiển việc khởi tạo các Labeler/Config
    [Header("Label Config Generation")]
    private IdLabelConfig idLabelConfig;
    private SemanticSegmentationLabelConfig semanticSegmentationLabelConfig;

    public bool createBoundingboxLabel = true;
    public bool createSegmentationLabel = true;
    public bool createDepthLabel = false; 

 
    [SerializeField] private GenDataSettings ExtendedSettings;
    [SerializeField] private float timeToRender = 1f;
    [SerializeField] private float timeToCapture = 0.5f;
    [SerializeField] private bool autoCapture = false;
    [SerializeField] private int maxRetries = 100;

    private PerceptionCamera perceptionCamera;
    private GameObject currentModel;
    private bool isProcessing = false;
    private bool needCapture = false;



    //[SerializeField] WaterSurface waterSurface; // Tham chiếu đến WaterSurface với môi trường là biển

    [SerializeField]
    private static int generatedPicNum;
    [SerializeField]
    private static int totalPicNum;

    public static int GeneratedPicNum
    {
        get => generatedPicNum;
        private set => generatedPicNum = value;
    }

    public static int TotalPicNum
    {
        get => totalPicNum;
        private set => totalPicNum = value;
    }

    [System.Serializable]
    public static class ParameterConfig
    {
        public static int MinPixelCount = 1000;
        public static int MinBoundingBoxWidth = 50;
        public static int MinBoundingBoxHeight = 50;
    }

    private void Start()
    {
        Debug.LogError("Starting....");
        Ins = this;
        InitializeLabels();
        Debug.Log("Start");
        SetOutPath();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            //StartGenerateData();
            if (isProcessing)
            {
                needCapture = true; // Đặt lại cờ để chụp ảnh mới
            }
            else
            {
                StartGenerateData(); // Bắt đầu lại quá trình tạo dữ liệu
            }
        }
    }

    /// <summary>
    /// Khởi tạo danh sách nhãn từ Parameters.Models.
    /// </summary>
    //private void InitializeLabels()
    //{
    //    if (parameters == null || parameters.Models == null)
    //    {
    //        Debug.LogError("Parameters or Models is null");
    //        return;
    //    }

    //    var labelEntries = new List<IdLabelEntry>();
    //    labelEntries.Add(new IdLabelEntry
    //    {
    //        label = "Terrain", // Thêm nhãn cho nền
    //        id = 0 // ID cho nền
    //    });
    //    labelEntries.Add(new IdLabelEntry
    //    {
    //        label = "Tree", // Thêm nhãn cho nền
    //        id = 1 // ID cho nền
    //    });
    //    for (int i = 0; i < parameters.Models.Count; i++)
    //    {
    //        labelEntries.Add(new IdLabelEntry
    //        {
    //            label = parameters.Models[i].Label,
    //            id = i + 2
    //        });
    //    }
    //    if (idLabelConfig != null)
    //        idLabelConfig.Init(labelEntries);

    //    // Khởi tạo nhãn cho phân đoạn ngữ nghĩa
    //    var semanticLabelEntries = new List<SemanticSegmentationLabelEntry>();
    //    semanticLabelEntries.Add(new SemanticSegmentationLabelEntry
    //    {
    //        label = "Terrain", // Thêm nhãn cho nền
    //        color = Color.gray // Màu sắc cho nền
    //    });
    //    semanticLabelEntries.Add(new SemanticSegmentationLabelEntry
    //    {
    //        label = "Tree", // Thêm nhãn cho cây
    //        color = Color.green // Màu sắc cho cây
    //    });
    //    for (int i = 0; i < parameters.Models.Count; i++)
    //    {
    //        semanticLabelEntries.Add(new SemanticSegmentationLabelEntry
    //        {
    //            label = parameters.Models[i].Label,
    //            color = Color.HSVToRGB(i / (float)parameters.Models.Count, 1f, 1f), // Màu sắc khác nhau cho mỗi nhãn
    //        });
    //    }
    //    if (semanticSegmentationLabelConfig != null)
    //        semanticSegmentationLabelConfig.Init(semanticLabelEntries);
    //}
    private void InitializeLabels()
    {
        if (parameters == null || parameters.Models == null)
        {
            Debug.LogError("Parameters or Models list is null. Cannot initialize labels.");
            return;
        }

        // --- 1. Tạo và Khởi tạo IdLabelConfig ---
        if (createBoundingboxLabel)
        {
            // Tạo mới (hoặc tái sử dụng) ScriptableObject
            if (idLabelConfig == null)
            {
                idLabelConfig = ScriptableObject.CreateInstance<IdLabelConfig>();
            }

            var labelEntries = new List<IdLabelEntry>();
            // Thêm các nhãn cố định
            labelEntries.Add(new IdLabelEntry { label = "Terrain", id = 0 });
            labelEntries.Add(new IdLabelEntry { label = "Tree", id = 1 });

            // Thêm các nhãn từ Models (bắt đầu từ ID 2)
            for (int i = 0; i < parameters.Models.Count; i++)
            {
                labelEntries.Add(new IdLabelEntry
                {
                    label = parameters.Models[i].Label,
                    id = i + 2
                });
            }

            idLabelConfig.Init(labelEntries);
            Debug.Log($"IdLabelConfig đã được tạo với {labelEntries.Count} nhãn.");
        }


        // --- 2. Tạo và Khởi tạo SemanticSegmentationLabelConfig ---
        if (createSegmentationLabel)
        {
            if (semanticSegmentationLabelConfig == null)
            {
                semanticSegmentationLabelConfig = ScriptableObject.CreateInstance<SemanticSegmentationLabelConfig>();
            }

            var semanticLabelEntries = new List<SemanticSegmentationLabelEntry>();
            // Thêm các nhãn cố định với màu sắc
            semanticLabelEntries.Add(new SemanticSegmentationLabelEntry { label = "Terrain", color = Color.gray });
            semanticLabelEntries.Add(new SemanticSegmentationLabelEntry { label = "Tree", color = Color.green });

            // Thêm các nhãn Models với màu sắc tự động
            for (int i = 0; i < parameters.Models.Count; i++)
            {
                semanticLabelEntries.Add(new SemanticSegmentationLabelEntry
                {
                    label = parameters.Models[i].Label,
                    // Sử dụng HSV để tạo màu sắc độc đáo cho mỗi nhãn
                    color = Color.HSVToRGB(i / (float)parameters.Models.Count, 1f, 1f),
                });
            }

            semanticSegmentationLabelConfig.Init(semanticLabelEntries);
            Debug.Log($"SemanticSegmentationLabelConfig đã được tạo với {semanticLabelEntries.Count} nhãn.");
        }
    }


/// <summary>
/// Thiết lập đường dẫn lưu ảnh và nhãn
/// </summary>
private void SetOutPath()
    {
        PerceptionSettings.SetOutputBasePath(GeneratorSettings.Instance.SyncDataPath);
        DatasetCapture.ResetSimulation();
    }

    /// <summary>
    /// Thiết lập PerceptionCamera cho việc chụp ảnh và gắn nhãn.
    /// </summary>
    private bool SetupPerceptionCamera()
    {
        if (objCamera == null)
        {
            Debug.LogError("ObjCamera is null");
            return false;
        }

        if (objCamera.TryGetComponent<Camera>(out var camera))
        {
            camera.fieldOfView = 15; // TODO: Đọc từ Parameters
        }

        if (objCamera.TryGetComponent<PerceptionCamera>(out var existingCamera))
        {
            DestroyImmediate(existingCamera);
            //Xóa cả 2 component nữa là  HUDPanel và OverlayPanel nếu có
            var hudPanel = objCamera.GetComponent<UnityEngine.Perception.GroundTruth.HUDPanel>();
            if (hudPanel != null)
            {
                DestroyImmediate(hudPanel);
            }
            var overlayPanel = objCamera.GetComponent<UnityEngine.Perception.GroundTruth.OverlayPanel>();
            if (overlayPanel != null)
            {
                DestroyImmediate(overlayPanel);
            }
        }

        perceptionCamera = objCamera.AddComponent<PerceptionCamera>();
        perceptionCamera.captureTriggerMode = UnityEngine.Perception.GroundTruth.DataModel.CaptureTriggerMode.Manual;
        if (createBoundingboxLabel && idLabelConfig != null)
        {
            var boundingBox2DLabeler = new BoundingBox2DLabeler { idLabelConfig = idLabelConfig };
            perceptionCamera.AddLabeler(boundingBox2DLabeler);
        }
        if (createSegmentationLabel && semanticSegmentationLabelConfig != null)
        {
            var SemanticSegmentationLabeler = new SemanticSegmentationLabeler(semanticSegmentationLabelConfig);
            perceptionCamera.AddLabeler(SemanticSegmentationLabeler);
        }
        if (createDepthLabel)
        {
            // 1. Khởi tạo đối tượng DepthLabeler
            var depthLabeler = new DepthLabeler(); // Khởi tạo không tham số, đúng cú pháp

            // 2. Thêm Labeler vào PerceptionCamera
            perceptionCamera.AddLabeler(depthLabeler);

            Debug.Log("DepthLabeler đã được khởi tạo và thêm vào PerceptionCamera.");
        }
        perceptionCamera.RenderedObjectInfosCalculated += OnRenderedObjectInfosCalculated;

        return true;
    }

    /// <summary>
    /// Xử lý sự kiện khi thông tin đối tượng được render.
    /// </summary>
    private void OnRenderedObjectInfosCalculated(int frameCount, NativeArray<RenderedObjectInfo> renderedObjectInfos, SceneHierarchyInformation hierarchyInfo)
    {
        //Debug.Log($"Frame {frameCount} - Rendered object count: {renderedObjectInfos.Length}");
        CountValidObjects(renderedObjectInfos);
        waitingForResult = false;
    }


    public void StartGenerateDataWithSetting(GenDataSettings settings)
    {
        Debug.Log("StartGenerateDataWithSetting");
        Debug.Log("Process status: isProcessing = " + isProcessing);
        ExtendedSettings = settings;
        StartGenerateData();
    }
    /// <summary>
    /// Bắt đầu quá trình tạo dữ liệu.
    /// </summary>
    public void StartGenerateData()
    {
        if (isProcessing)
        {
            Debug.LogWarning("Data generation is already running");
            return;
        }

        if (parameters == null || parameters.Models == null || parameters.Models.Count == 0)
        {
            Debug.LogError("Parameters or Models is null or empty");
            return;
        }
        //SetOutPath();
        if (!SetupPerceptionCamera())
        {
            return;
        }

        isProcessing = true;
        ResetCounter();
        ResetStepIndex();
        StartCoroutine(GenerateData());
    }

    /// <summary>
    /// Đặt lại bộ đếm số ảnh.
    /// </summary>
    private void ResetCounter()
    {
        GeneratedPicNum = 0;
        TotalPicNum = 0;
        foreach (var model in parameters.Models)
        {
            TotalPicNum += model.NumberOfPictures;
        }
    }

    /// <summary>
    /// Coroutine chính để tạo dữ liệu ảnh và nhãn.
    /// </summary>
    private IEnumerator GenerateData()
    {
        Debug.Log("Starting data generation");
        float startTime = Time.time;

        foreach (var model in parameters.Models)
        {
            if (ExtendedSettings != null)
            {
                if (!ExtendedSettings.Objects.Contains(model.Label))
                {
                    Debug.LogWarning($"Model {model.Label} is not selected");
                    continue;
                }
            }

            int pictureCount = 0;
            int selectedVariantCount = model.Variants.Count(v => v.IsSelected);
            if (selectedVariantCount == 0) continue;

            if (ExtendedSettings != null)
            {
                model.NumberOfPictures = ExtendedSettings.SoLuongAnh;
            }
            else
            {
                model.NumberOfPictures = parameters.NumberOfPicturesPerModel;
            }


            //Override một số thuộc tính
            int picturesPerVariant = Mathf.CeilToInt((float)model.NumberOfPictures / selectedVariantCount);
            if (model.Label.StartsWith("Air."))
            {
                model.HeightMin = 150f; // Đặt chiều cao tối thiểu cho mô hình Air
                model.HeightMax = 400f; // Đặt chiều cao tối đa cho mô hình Air
                model.RotateOnlyYAxis = false; // Không xoay chỉ theo trục Y
            }
            else
            {
                model.HeightMin = 0f; // Chiều cao tối thiểu cho các mô hình khác
                model.HeightMax = 1f; // Chiều cao tối đa cho các mô hình khác
                model.RotateOnlyYAxis = true; // Chỉ xoay theo trục Y cho các mô hình khác
            }
            //=====
            foreach (var variant in model.Variants)
            {
                if (!variant.IsSelected) continue;

                DestroyCurrentModel();
                currentModel = InstantiateModel(variant.Model, model.Label);
                //if (currentModel.name.StartsWith("Sea"))
                //{
                //    var fitToWater = currentModel.AddComponent<FitToWaterSurface>();
                //    fitToWater.targetSurface = waterSurface; // Gắn WaterSurface vào FitToWaterSurface
                //}

                if (currentModel == null) continue;

                for (int i = 0; i < picturesPerVariant && pictureCount < model.NumberOfPictures; i++)
                {
                    yield return GenerateSingleImage(model);

                    pictureCount++;
                    GeneratedPicNum++;
                    //Debug.Log($"Captured {model.Label} - Image {pictureCount}/{model.NumberOfPictures}");


                    if (autoCapture)
                    {
                        needCapture = true;
                    }
                    else
                    {
                        needCapture = false;
                        while (!needCapture) yield return new WaitForSeconds(0.5f); ;
                    }
                }
            }
        }

        DestroyCurrentModel();
        if (perceptionCamera != null)
        {
            DestroyImmediate(perceptionCamera);
            perceptionCamera = null;
        }

        Debug.Log($"Data generation completed in {Time.time - startTime} seconds");
        isProcessing = false;

        //// Lưu InvalidPicturesIndex ra file
        //Debug.Log("Invalid pictures indices: " + string.Join(", ", InvalidPicturesIndex));
        //SaveInvalidPicturesIndex(InvalidPicturesIndex);
        Shutdown();
    }

    private void ResetStepIndex()
    {
        SimulationState.m_Step = 0;
    }
    public int SoAnhDaTao = 0;
    private void IncrementStepIndex()
    {
        SimulationState.m_Step++;
        Debug.Log($"Số ảnh {SimulationState.m_Step}");
        SoAnhDaTao = SimulationState.m_Step + 1;
    }

    //public List<int> InvalidPicturesIndex { get; private set; } = new List<int>();
    //int lastCaptureIndex = -1;
    /// <summary>
    /// Tạo một ảnh đơn lẻ và kiểm tra tính hợp lệ.
    /// </summary>
    /// <param name="model">Mô hình 3D cần xử lý.</param>
    /// <returns>Coroutine để tạo và kiểm tra ảnh.</returns>
    private IEnumerator GenerateSingleImage(Model3D model)
    {
        objectNumber = 0;
        validCount = 0;

        if (currentModel == null || objCamera == null || perceptionCamera == null)
        {
            Debug.LogError($"Cannot generate image for {model.Label}: currentModel, objCamera, or perceptionCamera is null");
            yield break;
        }

        if (RandomPositionSetter.Ins == null)
        {
            Debug.LogError("RandomPositionSetter instance is null");
            validCount = 0;
            yield break;
        }

        WeatherManager.Instance.SetRandomSceneBrightness();

        bool finish = false;
        while (!finish)
        {
            float targetRatio = (float)UnityEngine.Random.Range(ExtendedSettings.DoLonAnhMin, ExtendedSettings.DoLonAnhMax) / 100;
            Debug.Log("Target ratio = " + targetRatio);


            //Bước 1: tìm vị trí đặt vật và camera sao cho tỉ lệ hiển thị vật khoảng 0.3
            Debug.Log("Bắt đầu bước 1");
            yield return StartCoroutine(SetRandomObjectPosionAnDefaultCameraPosition(model, maxRetries));

            if (step1_success)
            {
                Debug.Log("Bắt đầu bước 2");
                //Bước 2: điều chỉnh khoảng cách của camera để tỉ lệ đối tượng đúng như tỉ lệ mong muốn
                yield return StartCoroutine(AdjustCameraAndTakeCapture(model, targetRatio, maxRetries));

                if (step2_success)
                {
                    //Dừng để chụp ảnh
                    yield return new WaitForSeconds(timeToRender);
                    CaptureImage();
                    yield return new WaitForSeconds(0.2f);
                    finish = true;
                }
            }
        }
        IncrementStepIndex();
    }
    bool step1_success = false;
    bool step2_success = false;
    private IEnumerator SetRandomObjectPosionAnDefaultCameraPosition(Model3D model, int maxRetries)
    {
        step1_success = false;
        int retryCount = 0;

        while (step1_success == false && retryCount < maxRetries)
        {
            retryCount++;
            Debug.Log($"Step 1 Retry " + retryCount);
            float defaultRatio = 0.3f;
            bool highAngleOnly = true;
            if (model.Label.StartsWith("Air")) highAngleOnly = false;
            RandomPositionSetter.Ins.SetRandomPosition(currentModel, model);
            RandomPositionSetter.Ins.SetCameraRandomPosition(objCamera.transform, currentModel.transform, defaultRatio, highAngleOnly);

            yield return new WaitForSeconds(0.1f);
            CaptureImage(); //Capture lần đầu chỉ để lấy vị trí tương đói, chưa cần đẹp

            //Đợi đến khi máy render và tính toán xong
            waitingForResult = true;
            while (waitingForResult)
            {
                yield return null;
            }
            if (objectNumber > 0 && areaRatio > 20 && areaRatio < 60)
            {
                step1_success = true;
                Debug.Log($"Xong bước 1 sau {retryCount} lần thử");
            }
        }
    }

    private IEnumerator AdjustCameraAndTakeCapture(Model3D model, float targetRatio, int maxRetries)
    {
        maxRetries = 5;
        int retryCount = 0;
        step2_success = false;
        while (validCount == 0 && retryCount < maxRetries)
        {
            retryCount++;
            //Debug.Log($"Step 2 Retry " + retryCount);

            if (areaRatio < 0.01f)
            {
                Debug.LogWarning($"Đối tượng quá nhỏ {areaRatio}. Restart sinh ảnh");
                break;
            }
            RandomPositionSetter.Ins.AdjustCamera(objCamera.transform, currentModel.transform, targetRatio, areaRatio / 100);
            // Chỗ này sau giảm đi
            yield return new WaitForSeconds(0.1f);
            CaptureImage();

            waitingForResult = true;
            while (waitingForResult)
            {
                yield return null;
            }

            if (validCount == 0)
            {
                Debug.LogWarning($"Retry {retryCount}/{maxRetries} failed for {model.Label}. \nCurrentRatio/TargetRatio {areaRatio}/{targetRatio} : ");
            }
            if (validCount > 0)
            {
                step2_success = true;
            }
        }
    }




    //Ảnh hiện tại
    private bool waitingForResult = false;
    private int objectNumber = 0;
    //private bool validResult = false;
    private int validCount = 0;
    private float areaRatio = 0;
    /// <summary>
    /// Đếm số đối tượng hợp lệ dựa trên số điểm ảnh và kích thước bounding box.
    /// </summary>
    private void CountValidObjects(NativeArray<RenderedObjectInfo> renderedObjectInfos)
    {
        validCount = 0;
        objectNumber = 0;
        areaRatio = 0;
        if (renderedObjectInfos == null || renderedObjectInfos.Length == 0) { return; }
        //foreach (var objectInfo in renderedObjectInfos)
        {
            objectNumber = renderedObjectInfos.Length;

            //Tạm thời chỉ xét một đối tượng
            var objectInfo = renderedObjectInfos[0];

            float dolonDoiTuong = objectInfo.boundingBox.width * objectInfo.boundingBox.height;
            var cam = Camera.main;
            float dolonAnh = cam.pixelWidth * cam.pixelHeight;
            areaRatio = dolonDoiTuong / dolonAnh * 100;

            //Debug.Log($"Object info: {objectInfo.boundingBox}, PixelCount: {objectInfo.pixelCount} Ratio: {areaRatio}");

            if (objectInfo.pixelCount > ParameterConfig.MinPixelCount &&
                objectInfo.boundingBox.width > ParameterConfig.MinBoundingBoxWidth &&
                objectInfo.boundingBox.height > ParameterConfig.MinBoundingBoxHeight)
            {
                if (ExtendedSettings != null)
                {
                    if (areaRatio >= ExtendedSettings.DoLonAnhMin && areaRatio <= ExtendedSettings.DoLonAnhMax)
                    {
                        //Debug.Log($"Valid object: {objectInfo.boundingBox}, PixelCount: {objectInfo.pixelCount} Ratio: {areaRatio}");
                        validCount++;
                    }
                    else
                    {
                        //Debug.LogWarning($"Ảnh có độ lớn không phù hợp. Ratio {areaRatio}");
                    }
                }
                //validCount++;
            }
            else
            {
                Debug.LogWarning("Ảnh không đạt kích thước tối thiểu");
            }
        }
    }

    /// <summary>
    /// Đóng hẳn chương trình đang chạy (bản build)
    /// Stop nếu đang chạy trong Unity Editor.
    /// </summary>
    private void Shutdown()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
        Debug.Log("Application is shutting down.");
    }

    /// <summary>
    /// Chụp ảnh bằng PerceptionCamera.
    /// </summary>
    private void CaptureImage()
    {
        if (perceptionCamera == null)
        {
            Debug.LogError("PerceptionCamera is null");
            return;
        }
        perceptionCamera.RequestCapture();
        //Debug.Log("Capture data");
    }

    /// <summary>
    /// Tạo instance của mô hình 3D và gắn nhãn.
    /// </summary>
    private GameObject InstantiateModel(GameObject model, string label)
    {
        if (model == null)
        {
            Debug.LogError("Model is null");
            return null;
        }

        GameObject obj = Instantiate(model);
        var labeling = obj.AddComponent<Labeling>();
        labeling.labels = new List<string> { label };
        labeling.RefreshLabeling();
        return obj;
    }

    /// <summary>
    /// Hủy mô hình hiện tại.
    /// </summary>
    private void DestroyCurrentModel()
    {
        if (currentModel != null)
        {
            Destroy(currentModel);
            currentModel = null;
            StartCoroutine(WaitForFrame()); // Đợi một frame để đảm bảo hủy xong
        }
    }

    private IEnumerator WaitForFrame()
    {
        yield return null;
    }

    // Wrapper class for serialization
    [Serializable]
    public class InvalidPicturesIndexWrapper
    {
        public string DateTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        public List<int> indices;
    }

    private void SaveInvalidPicturesIndex(List<int> InvalidPicturesIndex)
    {
        // Save InvalidPicturesIndex to JSON file
        var wrapper = new InvalidPicturesIndexWrapper { indices = InvalidPicturesIndex };
        string json = JsonUtility.ToJson(wrapper, true);
        string path = System.IO.Path.Combine(Application.dataPath, "../InvalidPicturesIndex.json");
        System.IO.File.WriteAllText(path, json);
        Debug.Log($"InvalidPicturesIndex saved to {path}");
    }
}
