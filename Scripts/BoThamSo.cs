

using System;
using System.Collections.Generic;

[System.Serializable]
public class BoThamSo : YamlData<BoThamSo>
{
    public string Id;
    public string Id_NguoiDung;
    public string TenBoThamSo;
    public string MoTa;
    public string NgayTao;

    //Các danh sách lưu giá trị id, cách nhau bằng dấu phẩy, cái này lưu vào database, khi lấy ra thì sẽ tách ra thành list
    public string Ds_DiaHinh; //Địa hình
    public List<string> listDiaHinh;


    public string Ds_ThoiTiet; //Thời tiết
    public List<string> listThoiTiet;

    public string Ds_MocThoiGian; //Mốc thời gian
    public List<string> listMocThoiGian;

    //Số lượng ảnh cần sinh cho mỗi loại đối tượng
    public int SoLuongAnhSinh_MoHinh3D; //Số lượng ảnh sinh cho mô hình 3D

    public int LoaiMayAnh; //Loại máy ảnh
    public int Fov; //Field of View
    public int DoPhanGiai_X; //Độ phân giải
    public int DoPhanGiai_Y; //Độ phân giải
    public int KichThuocDoiTuongNhoNhat_X; //Kích thước đối tượng nhỏ nhất
    public int KichThuocDoiTuongNhoNhat_Y; //Kích thước đối tượng nhỏ nhất


    public string Ds_MoHinh; //Mô hình
    public List<string> listMoHinh;

    /// <summary>
    /// Chuyển đổi các danh sách từ chuỗi sang danh sách
    /// </summary>
    public void ToList()
    {
        try
        {
            // Chuyển đổi danh sách địa hình bằng cách tách chuỗi theo dấu phẩy
            if (!string.IsNullOrEmpty(Ds_DiaHinh))
            {
                listDiaHinh = new List<string>(Ds_DiaHinh.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries));
            }
            else
            {
                listDiaHinh = new List<string>();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Lỗi chuyển đổi danh sách: " + ex.Message);
        }
        try
        {
            // Chuyển đổi danh sách địa hình bằng cách tách chuỗi theo dấu phẩy
            if (!string.IsNullOrEmpty(Ds_MoHinh))
            {
                listMoHinh = new List<string>(Ds_MoHinh.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries));
            }
            else
            {
                listMoHinh = new List<string>();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Lỗi chuyển đổi danh sách: " + ex.Message);
        }
        try
        {
            // Chuyển đổi danh sách thời tiết bằng cách tách chuỗi theo dấu phẩy
            if (!string.IsNullOrEmpty(Ds_ThoiTiet))
            {
                listThoiTiet = new List<string>(Ds_ThoiTiet.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries));
            }
            else
            {
                listThoiTiet = new List<string>();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Lỗi chuyển đổi danh sách: " + ex.Message);
        }
        try
        {
            // Chuyển đổi danh sách mốc thời gian bằng cách tách chuỗi theo dấu phẩy
            if (!string.IsNullOrEmpty(Ds_MocThoiGian))
            {
                listMocThoiGian = new List<string>(Ds_MocThoiGian.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries));
            }
            else
            {
                listMocThoiGian = new List<string>();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Lỗi chuyển đổi danh sách: " + ex.Message);
        }
    }

    /// <summary>
    /// Hàm cập nhật các danh sách từ chuỗi
    /// </summary>
    public void ListToString()
    {
        try
        {
            // Chuyển đổi danh sách địa hình thành chuỗi
            Ds_DiaHinh = string.Join(",", listDiaHinh);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Lỗi chuyển đổi danh sách địa hình: " + ex.Message);
        }
        try
        {
            // Chuyển đổi danh sách mô hình thành chuỗi
            Ds_MoHinh = string.Join(",", listMoHinh);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Lỗi chuyển đổi danh sách mô hình: " + ex.Message);
        }
        try
        {
            // Chuyển đổi danh sách thời tiết thành chuỗi
            Ds_ThoiTiet = string.Join(",", listThoiTiet);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Lỗi chuyển đổi danh sách thời tiết: " + ex.Message);
        }
        try
        {
            // Chuyển đổi danh sách mốc thời gian thành chuỗi
            Ds_MocThoiGian = string.Join(",", listMocThoiGian);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Lỗi chuyển đổi danh sách mốc thời gian: " + ex.Message);
        }
    }
}

[System.Serializable]
public enum LoaiMayAnh
{
    CameraRGB = 0,
    CameraAnhNhiet = 1, //Camera Ảnh nhiệt
    CameraKhuechDaiAnhSangMo = 2, //Camera Khuyếch đại ánh sáng

}
[System.Serializable]
public class DiaHinh3D
{
    public string Id;
    public string TenDiaHinh;
    public string MoTa;
    public string NgayTao;
    public string DuongDan_AnhDaiDien; //Đường dẫn ảnh đại diện
    public string DuongDan_MoHinh3D; //Đường dẫn mô hình 3D
}
[System.Serializable]
public class MoHinh3D
{
    public string Id;
    public string TenMoHinh;
    public string MoTa;
    public string Nhan; //Nhãn mô hình
    public string NgayTao;
    public string DuongDan_AnhDaiDien; //Đường dẫn ảnh đại diện

    //Đường dẫn mô hình 3D; mỗi phiên bản mô hình có một đường dẫn, phân tách nhau bằng dấu phẩy
    public string DuongDan_MoHinh3D; //Đường dẫn mô hình 3D
    public List<string> listDuongDan_MoHinh3D; //Danh sách đường dẫn mô hình 3D

    public void ToList()
    {
        try
        {
            // Chuyển đổi danh sách đường dẫn mô hình 3D thành danh sách
            if (!string.IsNullOrEmpty(DuongDan_MoHinh3D))
            {
                listDuongDan_MoHinh3D = new List<string>(DuongDan_MoHinh3D.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries));
            }
            else
            {
                listDuongDan_MoHinh3D = new List<string>();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Lỗi chuyển đổi danh sách đường dẫn mô hình 3D: " + ex.Message);
        }
    }
    public void ListToString()
    {
        try
        {
            // Chuyển đổi danh sách đường dẫn mô hình 3D thành chuỗi
            DuongDan_MoHinh3D = string.Join(",", listDuongDan_MoHinh3D);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Lỗi chuyển đổi danh sách đường dẫn mô hình 3D: " + ex.Message);
        }
    }
}

[System.Serializable]
public class DanhSachDiaHinh3D : JsonData<DanhSachDiaHinh3D>
{
    public List<DiaHinh3D> dsDiaHinh3D;
}

[System.Serializable]
public class DanhSachMoHinh3D : JsonData<DanhSachMoHinh3D>
{
    public List<MoHinh3D> dsMoHinh3D;
}