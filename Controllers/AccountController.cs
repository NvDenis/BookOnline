using bookstore.Models;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Text;

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;

namespace bookstore.Controllers
{

    public class HocVienController : Controller
    {
        private readonly string filePath = "~/App_Data/hocvien.txt";

        // Hiển thị form nhập
        public ActionResult Create()
        {
            return View();
        }

        // Xử lý khi bấm nút lưu
        [HttpPost]
        public IActionResult Create(HocVien model, IFormFile HinhFile)
        {
            // Lưu hình
            if (HinhFile != null && HinhFile.Length > 0)
            {
                var fileName = Path.GetFileName(HinhFile.FileName);
                var savePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", fileName);

                using (var stream = new FileStream(savePath, FileMode.Create))
                {
                    HinhFile.CopyTo(stream);
                }

                model.Hinh = fileName;
            }

            // Ghi vào file
            string data = $"{model.MaHocVien}|{model.HoTen}|{model.GioiTinh}|{model.NgaySinh:yyyy-MM-dd}|{model.HocPhi}|{model.Hinh}|{model.GhiChu}";
            string absolutePath = Path.Combine(Directory.GetCurrentDirectory(), "App_Data", "hocvien.txt");

            System.IO.File.AppendAllLines(absolutePath, new[] { data }, Encoding.UTF8);

            return RedirectToAction("Index");
        }

        // Đọc dữ liệu từ file và hiển thị
        public ActionResult Index()
        {
            var list = new List<HocVien>();
            string absolutePath = Path.Combine(Directory.GetCurrentDirectory(), "App_Data", "hocvien.txt");


            if (System.IO.File.Exists(absolutePath))
            {
                var lines = System.IO.File.ReadAllLines(absolutePath);
                foreach (var line in lines)
                {
                    var parts = line.Split('|');
                    list.Add(new HocVien
                    {
                        MaHocVien = parts[0],
                        HoTen = parts[1],
                        GioiTinh = parts[2],
                        NgaySinh = DateTime.Parse(parts[3]),
                        HocPhi = decimal.Parse(parts[4]),
                        Hinh = parts[5],
                        GhiChu = parts[6]
                    });
                }
            }

            return View(list);
        }
    }
}
