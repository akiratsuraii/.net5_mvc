using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using WebApplication1.Data;
using WebApplication1.Models;
using WebApplication1.Models.ViewModel;

namespace WebApplication1.Controllers
{
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _IWebHostEnviroment;

        //--inti
        public ProductController(ApplicationDbContext db, IWebHostEnvironment IWebHostEnviroment)
        {
            _db = db;
            _IWebHostEnviroment = IWebHostEnviroment;
        }
        #region Index
        public IActionResult Index()
        {
            IEnumerable<Product> objList = _db.Product.Include(u => u.Application).Include(u=>u.Category);
            //foreach (var obj in objList)
            //{
            //    if (obj.CategoryId != 0)
            //    {
            //        obj.Category = _db.Category.FirstOrDefault(u => u.Id == obj.CategoryId);
            //    }
            //}
            return View(objList);
        }
        #endregion
        #region Handle
        public IActionResult Handle(int? Id)
        {
            ProductVM productVM = new ProductVM()
            {
                Product = new Product(),
                CategoryDropDown = _db.Category.Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                }),
                ApplicationDropDown = _db.Application.Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                })
            };
            //查看是否帶PKID進入Handle頁面
            if (Id != null && Id != 0)
            {
                productVM.Product = _db.Product.Find(Id);
                if (productVM.Product != null)
                {
                    return View(productVM);
                }
                else
                {
                    return NotFound();
                }
            }
            return View(productVM);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Handle(ProductVM productVM)
        {

            if (ModelState.IsValid)
            {
                //圖片路徑
                var files = HttpContext.Request.Form.Files;
                string webRootPath = _IWebHostEnviroment.WebRootPath;
                string upload = webRootPath + WC.ImagePath;
                string fileName = Guid.NewGuid().ToString();
                string extension = "";

                if (productVM.Product.Id == 0)//新增
                {
                    //處理圖片
                    if (files.Count > 0)
                    {
                         extension = Path.GetExtension(files[0].FileName);

                        using (var fileStream = new FileStream(Path.Combine(upload, fileName + extension), FileMode.Create))
                        {
                            files[0].CopyTo(fileStream);
                        }

                        productVM.Product.Images = fileName + extension;
                    }
                    _db.Product.Add(productVM.Product);
                }
                else //update 
                {
                    //取得原本資料
                    var objFromDb = _db.Product.AsNoTracking().FirstOrDefault(x => x.Id == productVM.Product.Id);
                    if (objFromDb != null)
                    {
                        //處理圖片
                        if (files.Count > 0)
                        {
                            extension = Path.GetExtension(files[0].FileName);
                            using (var fileStream = new FileStream(Path.Combine(upload, fileName + extension), FileMode.Create))
                            {
                                files[0].CopyTo(fileStream);
                            }
                            //刪除舊圖片
                            if (objFromDb.Images != null)
                            {
                                var oldFile = Path.Combine(upload, objFromDb.Images);
                                deleteImg(oldFile);
                            }
                            productVM.Product.Images = fileName + extension;//置入新路徑
                        }
                        else
                        {
                            productVM.Product.Images = objFromDb.Images;//回填舊路徑
                        }

                        _db.Product.Update(productVM.Product);
                    }
                }
            }

            //載入DropDown值
            productVM.CategoryDropDown = _db.Category.Select(i => new SelectListItem
            {
                Text = i.Name,
                Value = i.Id.ToString()
            });
            productVM.ApplicationDropDown = _db.Application.Select(i => new SelectListItem
            {
                Text = i.Name,
                Value = i.Id.ToString()
            });
            _db.SaveChanges();
            return View(productVM);
        }
        #endregion

        #region Delete
        public IActionResult Delete(int? Id)
        {
            Product obj = _db.Product.Find(Id);
            if (obj != null)
            {
                if (obj.Images != null)
                {
                    deleteImg(obj.Images);
                }
                _db.Product.Remove(obj);
                _db.SaveChanges();
            }
            return RedirectToAction("Index");

        }
        #endregion

        #region GenericFunction
        public bool deleteImg(string imgPath)
        {
            bool result = false;
            try
            {
                if (System.IO.File.Exists(imgPath))//刪除原有圖片
                {
                    System.IO.File.Delete(imgPath);
                }
                result = true;
            }
            catch { }
            return result;
        }
        #endregion
    }
}
