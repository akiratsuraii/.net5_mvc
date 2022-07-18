using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using WebApplication1.Data;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class ApplicationController : Controller
    {
        private readonly ApplicationDbContext _db;

        public ApplicationController(ApplicationDbContext db)
        {
            _db = db;
        }
        public IActionResult Index()
        {
            IEnumerable<Application> objList = _db.Application;
            return View(objList);
        }

        public IActionResult Create(int? id)
        {
            if (id != 0)
            {
                var updatingObj = _db.Application.Find(id);
                return View(updatingObj);
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Application obj)
        {
            if (ModelState.IsValid)
            {
                if (obj.Id != 0)
                {
                    _db.Application.Update(obj);
                }
                else
                {
                    _db.Application.Add(obj);
                }
                _db.SaveChanges();
            }
            return View();
        }
    }
}
