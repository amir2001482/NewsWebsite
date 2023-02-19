using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NewsWebsite.Areas.Admin.Controllers
{
    public class CategoryController : BaseController
    {
       
        public IActionResult Index()
        {
            return View();
        }
    }
}
