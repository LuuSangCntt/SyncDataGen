using System;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Bộ tham số cho việc sinh dữ liệu từ mô phỏng 3D
/// </summary>
[Serializable]
public class ParameterPack : YamlData<ParameterPack>
{
    //CÁC giới hạn tối thiểu về chất lượng ảnh đối tượng theo thuyết minh
    public int MinPixelCount = 1000;
    public int MinBoundingBoxWidth = 50;
    public int MinBoundingBoxHeight = 50;

    //Giới hạn tùy chỉnh về ảnh đối tượng
    public int DoLonAnhMin = 30; //Tỉ lệ về diện tích boundingbox của vật so với tỉ lệ ảnh (%)
    public int DoLonAnhMax = 50;

    public int NumberOfPicturesPerModel = 10;     //Số lượng ảnh cần chụp cho mỗi loại mô hình
    public List<Model3D> Models;    //Danh sách các models


    private static ParameterPack _instance;
    public static ParameterPack Instance
    {
        get
        {
            if (_instance == null)
            {
                string configPath = System.IO.Path.Combine(Application.streamingAssetsPath, "ParameterPack.yaml");
                _instance = ParameterPack.Load(configPath);
            }
            return _instance;
        }
    }
    public static void CreateDefault()
    {
        if (_instance == null)
        {
            _instance = new ParameterPack()
            {
                MinPixelCount = 1000,
                MinBoundingBoxWidth = 50,
                MinBoundingBoxHeight = 50,
                DoLonAnhMin = 30,
                DoLonAnhMax = 50,
                NumberOfPicturesPerModel = 10,
                Models = new List<Model3D>()
                {
                    new Model3D()
                    {
                        Label = "Xe tăng",
                        Variants = new List<ModelVariant>()
                        {
                            new ModelVariant()
                            {
                                Name = "Xe tăng 1",
                                IsSelected = true
                            },
                            new ModelVariant()
                            {
                                Name = "Xe tăng 2",
                                IsSelected = true
                            }
                        }
                    }
                }
            };

            string configPath = System.IO.Path.Combine(Application.streamingAssetsPath, "ParameterPack.yaml");
            Save(_instance, configPath);
        }
    }
}
