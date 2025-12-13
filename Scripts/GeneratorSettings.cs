using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public class GeneratorSettings : YamlData<GeneratorSettings>
{
    public bool SaveInStreamingAssets { get; set; } = true;
    public string SyncDataPath { get; set; } = "D:\\PerceptionData";

    // Tham số dùng để điều chính khoảng cách giữa đối tượng và camera. (distance = Factor * MaxSize)
    public int DistanceFactorMin { get; set; } = 1;

    public int DistanceFactorMax { get; set; } = 30;

    //Địa chỉ lưu dữ liệu địa hình, mô hình
    public string RootPath { get; set; } = "D:\\PerceptionData\\AssetBundles";


    public float TimeToRender { get; set; } = 1f;
    public float TimeToCapture { get; set; } = 1f;


    private static GeneratorSettings _instance;
    public static GeneratorSettings Instance
    {
        get
        {
            if (_instance == null)
            {
                string configPath = System.IO.Path.Combine(Application.streamingAssetsPath, "GeneratorSettings.yaml");
                _instance = GeneratorSettings.Load(configPath);
                _instance.SyncDataPath.Trim();
                _instance.RootPath.Trim();  
            }
            return _instance;
        }
    }
    public static void CreateDefault()
    {
        if (_instance == null)
        {
            _instance = new GeneratorSettings()
            {
                SaveInStreamingAssets = true,
                SyncDataPath = "C:\\PerceptionData",
                DistanceFactorMin = 5,
                DistanceFactorMax = 30,
                RootPath = "D:\\PerceptionData\\AssetBundles", // Địa chỉ lưu dữ liệu địa hình, mô hình
                TimeToRender = 1f,
                TimeToCapture = 1f
            };

            string configPath = System.IO.Path.Combine(Application.streamingAssetsPath, "GeneratorSettings.yaml");
            Save(_instance, configPath);
        }
    }
}
