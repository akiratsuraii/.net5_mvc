using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace WebApplication1.Models.ViewModel
{
    public class ProductVM
    {
        public Product Product { get; set; }

        public IEnumerable<SelectListItem> ApplicationDropDown { get; set; }
        public IEnumerable<SelectListItem> CategoryDropDown { get; set; }

    }
}
