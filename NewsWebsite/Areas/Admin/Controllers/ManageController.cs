using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NewsWebsite.Common;
using NewsWebsite.Entities.identity;
using NewsWebsite.Services.Contracts;
using NewsWebsite.ViewModels.Manage;

namespace NewsWebsite.Areas.Admin.Controllers
{
    public class ManageController : BaseController
    {
        private readonly IApplicationRoleManager _roleManager;
        private readonly IApplicationUserManager _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ILogger<ManageController> _logger;
        private readonly IHttpContextAccessor _accessor;
        private readonly IMapper _mapper;
        private readonly IHostingEnvironment _env;

        public ManageController(IApplicationRoleManager roleManager, IApplicationUserManager userManager, SignInManager<User> signInManager, ILogger<ManageController> logger, IHttpContextAccessor accessor, IMapper mapper, IHostingEnvironment env)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _accessor = accessor;
            _mapper = mapper;
            _env = env;
        }

        [HttpGet]
        public IActionResult SignIn(string ReturnUrl = null)
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SignIn(SignInViewModel ViewModel)
        {
            if (ModelState.IsValid)
            {
                var User = await _userManager.FindByNameAsync(ViewModel.UserName);
                if (User != null)
                {
                    if (User.IsActive)
                    {
                        var result = await _signInManager.PasswordSignInAsync(ViewModel.UserName, ViewModel.Password, ViewModel.RememberMe, true);
                        if (result.Succeeded)
                            return RedirectToAction("Index", "Dashboard", new { area = "Admin" });

                        else if (result.IsLockedOut)
                            ModelState.AddModelError(string.Empty, "حساب کاربری شما به مدت 20 دقیقه به دلیل تلاش های ناموفق قفل شد.");

                        else if (result.RequiresTwoFactor)
                            return RedirectToAction("SendCode", new { RememberMe = ViewModel.RememberMe });

                        else
                        {
                            ModelState.AddModelError(string.Empty, "نام کاربری یا کلمه عبور شما صحیح نمی باشد.");
                            _logger.LogWarning($"The user attempts to login with the IP address({_accessor.HttpContext?.Connection?.RemoteIpAddress.ToString()}) and username ({ViewModel.UserName}) and password ({ViewModel.Password}).");
                        }
                    }
                    else
                        ModelState.AddModelError(string.Empty, "حساب کابری شما غیرفعال است.");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "نام کاربری یا کلمه عبور شما صحیح نمی باشد.");
                    _logger.LogWarning($"The user attempts to login with the IP address({_accessor.HttpContext?.Connection?.RemoteIpAddress.ToString()}) and username ({ViewModel.UserName}) and password ({ViewModel.Password}).");
                }
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SignOut()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("SignIn", "Manage", new { area = "Admin" });
        }


        [HttpGet]
        public async Task<IActionResult> ChangePassword()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return NotFound();

            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel ViewModel)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return NotFound();

            if (ModelState.IsValid)
            {
                var changePassResult = await _userManager.ChangePasswordAsync(user, ViewModel.OldPassword, ViewModel.NewPassword);
                if (changePassResult.Succeeded)
                    ViewBag.Alert = "کلمه عبور شما با موفقیت تغییر یافت.";

                else
                    ModelState.AddErrorsFromResult(changePassResult);
            }

            return View(ViewModel);
        }


        [HttpGet]
        public async Task<IActionResult> Profile(int? userId)
        {
            var profileViewModel = new ProfileViewModel();
            if (userId == null)
                return NotFound();
            else
            {
                var user = await _userManager.FindByIdAsync(userId.ToString());
                if (user == null)
                    return NotFound();
                else
                {
                    profileViewModel = _mapper.Map<ProfileViewModel>(user);
                    profileViewModel.PersianBirthDate = profileViewModel.BirthDate.ConvertMiladiToShamsi("yyyy/MM/dd");
                }
            }

            return View(profileViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Profile(ProfileViewModel viewModel)
        {
            if (viewModel.Id == null)
                return NotFound();
            else
            {
                var user = await _userManager.FindByIdAsync(viewModel.Id.ToString());
                if (user == null)
                    return NotFound();
                else
                {
                    if (viewModel.ImageFile != null)
                    {
                        viewModel.Image = viewModel.ImageFile.FileName;
                        await viewModel.ImageFile.UploadFileAsync($"{_env.WebRootPath}/avatars/{viewModel.Image}");
                    }
                   
                    else
                        viewModel.Image = user.Image;

                    viewModel.BirthDate = viewModel.PersianBirthDate.ConvertShamsiToMiladi();
                    var result = await _userManager.UpdateAsync(_mapper.Map(viewModel, user));
                    if (result.Succeeded)
                        ViewBag.Alert = EditSuccess;
                    else
                        ModelState.AddErrorsFromResult(result);
                }

                return View(viewModel);
            }
        }
    }
}