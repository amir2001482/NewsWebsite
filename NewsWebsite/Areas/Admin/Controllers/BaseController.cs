using Microsoft.AspNetCore.Mvc;
using NewsWebsite.Common.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NewsWebsite.Areas.Admin.Controllers
{
    [Area(AreaConstants.areaName)]
    public class BaseController : Controller
    {
        public const string InsertSuccess = "درج اطلاعات با موفقیت انجام شد.";
        public const string EditSuccess = "ویرایش اطلاعات با موفقیت انجام شد.";
        public const string DeleteSuccess = "حذف اطلاعات با موفقیت انجام شد.";
        public const string OperationSuccess = "عملیات با موفقیت انجام شد.";

        public IActionResult Notification()
        {
           return Content(TempData["notification"].ToString());
        }

        [HttpGet , AjaxOnly()]
        public IActionResult DeleteGroup()
        {
            try
            {
                return PartialView("_DeleteGroup");
            }
            catch (Exception ex)
            {
                return PartialView("_DeleteGroup");
            }
           
        }
    }
}
