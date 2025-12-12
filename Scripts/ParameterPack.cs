using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Bộ tham số cho việc sinh dữ liệu từ mô phỏng 3D
/// </summary>
[CreateAssetMenu(fileName = "ParameterPack", menuName = "SyncDataGen/ParameterPack", order = 1)]
public class ParameterPack : ScriptableObject
{
    public int NumberOfPicturesPerModel = 1000;     //Số lượng ảnh cần chụp cho mỗi loại mô hình
    public List<Model3D> Models;    //Danh sách các models

}
