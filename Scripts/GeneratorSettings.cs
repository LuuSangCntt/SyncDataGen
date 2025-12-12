using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public class GeneratorSettings : YamlData<GeneratorSettings>
{
    public bool SaveInStreamingAssets { get; set; } = true;
    public string SyncDataPath { get; set; } = "C:\\PerceptionData";

    // Tham số dùng để điều chính khoảng cách giữa đối tượng và camera. (distance = Factor * MaxSize)
    public int DistanceFactorMin { get; set; } = 5;

    public int DistanceFactorMax { get; set; } = 30;

    //Địa chỉ lưu dữ liệu địa hình, mô hình
    public string RootPath { get; set; } = "C:\\WORKDATA_SANG\\07_KC4.0_SinhAnh3D\\Code\\DataGen_HDRP_Ocen\\Assets\\StreamingAssets\\AssetBundles";


    private static GeneratorSettings _instance;
    public static GeneratorSettings Instance
    {
        get
        {
            if (_instance == null)
            {
                string configPath = System.IO.Path.Combine(Application.streamingAssetsPath, "GeneratorSettings.yaml");
                _instance = GeneratorSettings.Load(configPath);
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
                RootPath = "C:\\WORKDATA_SANG\\07_KC4.0_SinhAnh3D\\Code\\DataGen_HDRP_Ocen\\Assets\\StreamingAssets\\AssetBundles"// Địa chỉ lưu dữ liệu địa hình, mô hình

            };

            string configPath = System.IO.Path.Combine(Application.streamingAssetsPath, "GeneratorSettings.yaml");
            Save(_instance, configPath);
        }
    }
}
