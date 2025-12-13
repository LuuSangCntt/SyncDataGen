using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Model3D
{
    public string Label;

    public List<ModelVariant> Variants;   //Các models với các biến thể khác nhau của texture
    //public int NumberOfPictures = 1000;     //Số lượng ảnh cần chụp   
    //public bool RotateOnlyYAxis = false;
    //public float HeightMin = 0;             //Chiều cao tối thiểu so với mặt đất
    //public float HeightMax = 100;           //Chiều cao tối đa so với mặt đất

}

[System.Serializable]
public class ModelVariant
{
    //public GameObject Model;
    public string Name;
    public bool IsSelected = true;
}
