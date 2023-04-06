using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NewsWebsite.Services.Contracts;
using NewsWebsite.ViewModels.DynamicAccess;

namespace NewsWebsite.Areas.Admin.Controllers
{
    [DisplayName("مدیریت سطح دسترسی")]
    public class DynamicAccessController : BaseController
    {
        public readonly IApplicationUserManager _userManager;
        public readonly IMvcActionsDiscoveryService _mvcActionsDiscovery;
        public DynamicAccessController(IApplicationUserManager userManager, IMvcActionsDiscoveryService mvcActionsDiscovery)
        {
            _userManager = userManager;
            _mvcActionsDiscovery = mvcActionsDiscovery;
        }

        [HttpGet]
        [DisplayName("مشاهده")]
        //[Authorize(Policy = ConstantPolicies.DynamicPermission)]
        public async Task<IActionResult> Index(int userId)
        {
            if (userId == 0)
                return NotFound();


            var user = await _userManager.FindClaimsInUser(userId);
            if (user == null)
                return NotFound();

            var securedControllerActions = _mvcActionsDiscovery.GetAllSecuredControllerActionsWithPolicy(ConstantPolicies.DynamicPermission);
            return View(new DynamicAccessIndexViewModel
            {
                UserIncludeUserClaims = user,
                SecuredControllerActions = securedControllerActions,
            });
        }

        [HttpPost, ValidateAntiForgeryToken]
        [DisplayName("افزودن یا ویرایش")]
        //[Authorize(Policy = ConstantPolicies.DynamicPermission)]
        public async Task<IActionResult> Index(DynamicAccessIndexViewModel ViewModel)
        {
            var Result = await _userManager.AddOrUpdateClaimsAsync(ViewModel.UserId, ConstantPolicies.DynamicPermissionClaimType, ViewModel.ActionIds.Split(","));
            if (!Result.Succeeded)
                ModelState.AddModelError(string.Empty, "در حین انجام عملیات خطایی رخ داده است.");

            return RedirectToAction("Index", new { userId = ViewModel.UserId });
        }
    }
}