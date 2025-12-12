using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class AssetBundleLoader : MonoBehaviour
{
    public static AssetBundleLoader Instance;

    public Dictionary<string, GameObject> LoadedModels = new Dictionary<string, GameObject>();


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject); // Giữ qua scene nếu cần
    }

    public string objectName = "ObjectName"; // Tên đối tượng để debug
    public GameObject target;
    public void Update()
    {
       
        if (Input.GetKeyDown(KeyCode.L))
        {
            TryGetModel(objectName, out target);
        }
        if(Input.GetKeyDown(KeyCode.U))
        {
            UnloadAllModels();
        }
    }

    /// <summary>
    /// Lấy model theo relative_path.
    /// Nếu đã cache thì dùng lại, nếu chưa thì load từ AssetBundle.
    /// </summary>
    public bool TryGetModel(string relative_path, out GameObject obj)
    {
        obj = null;

        if (string.IsNullOrEmpty(relative_path))
        {
            Debug.LogWarning("Path invalid: " + relative_path);
            return false;
        }

        // Nếu đã load prefab trước đó
        if (LoadedModels.TryGetValue(relative_path, out GameObject prefab))
        {
            obj = Instantiate(prefab);
            return true;
        }

        // Chưa load -> thử load từ assetbundle
        obj = TryLoadAssetBundle(relative_path);
        return obj != null;
    }

    /// <summary>
    /// Load prefab từ file AssetBundle
    /// </summary>
    private GameObject TryLoadAssetBundle(string relative_path)
    {
        string configPath = Path.Combine(Application.streamingAssetsPath, "GeneratorSettings.yaml");
        string path = Path.Combine(GeneratorSettings.Instance.RootPath, relative_path);

        if (!File.Exists(path))
        {
            Debug.LogWarning("File not found: " + path);
            return null;
        }

        var bundle = AssetBundle.LoadFromFile(path);
        if (bundle == null)
        {
            Debug.LogWarning("Failed to load AssetBundle: " + path);
            return null;
        }

        // (Điều kiện: kiểm soát AssetBundle chỉ có 1 GameObject)
        GameObject prefab = bundle.LoadAllAssets()[0] as GameObject;
        if (prefab == null)
        {
            Debug.LogWarning("First asset is not a GameObject in bundle: " + path);
            bundle.Unload(false);
            return null;
        }

        // Cache prefab trước khi unload bundle
        LoadedModels[relative_path] = prefab;

        // Tạo instance để trả về
        GameObject obj = Instantiate(prefab);

        // Giải phóng data bundle, giữ lại prefab đã cache
        bundle.Unload(false);

        return obj;
    }

    /// <summary>
    /// Giải phóng tất cả prefab đã cache.
    /// Gọi khi thoát game hoặc dừng Play trong Editor.
    /// </summary>
    public void UnloadAllModels()
    {
        foreach (var kvp in LoadedModels)
        {
            // Giải phóng memory prefab
            Resources.UnloadAsset(kvp.Value);
        }

        LoadedModels.Clear();

        // Dọn rác bộ nhớ
        Resources.UnloadUnusedAssets();
        System.GC.Collect();

        Debug.Log("All cached models unloaded.");
    }

    private void OnApplicationQuit()
    {
        UnloadAllModels();
    }

    private void OnDestroy()
    {
#if UNITY_EDITOR
        // Khi dừng Play trong Editor
        if (!Application.isPlaying)
        {
            UnloadAllModels();
        }
#endif
    }
}
