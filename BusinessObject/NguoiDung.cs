using CuahangNongduoc.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CuahangNongduoc.BusinessObject
{
    public class NguoiDung
    {
        public int Id { get; set; }
        public string TenDangNhap { get; set; }
        public string HoTen { get; set; }
        public string Role { get; set; }
        public bool MustChangePwd { get; set; }

        public virtual bool DaDangNhap { get { return true; } }
        public virtual bool CoQuyen(string maChucNang) { return PhanQuyen.CoQuyen(Role, maChucNang); }
    }

    public class NguoiDungKhach : NguoiDung
    {
        public NguoiDungKhach() { Id = 0; TenDangNhap = ""; HoTen = "Khách"; Role = ""; }
        public override bool DaDangNhap { get { return false; } }
        public override bool CoQuyen(string maChucNang) { return false; }
    }
}