
using System;
using System.Collections.Generic;

[Serializable]
public class GenDataSettings
{
    public List<string> Objects = new List<string>();
    public int SoLuongAnh = 100; //Ảnh cho mỗi đối tượng
    public int DoLonAnhMin = 30; //Tỉ lệ boundingbox của vật so với tỉ lệ ảnh
    public int DoLonAnhMax = 50;


    public bool Isvalid()
    {
        if (Objects == null || Objects.Count == 0) return false;
        if (SoLuongAnh <= 0) return false;
        if (DoLonAnhMin <= 0 || DoLonAnhMin > 100) return false;
        if (DoLonAnhMax <= 0 || DoLonAnhMax > 100) return false;
        if (DoLonAnhMin >= DoLonAnhMax) return false;
        return true;
    }
}
