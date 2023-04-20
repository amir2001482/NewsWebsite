using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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
        private readonly IWebHostEnvironment _env;
        private readonly IEmailSender _emailSender;
        private readonly ISmsSender _smsSender;

        public ManageController(IApplicationRoleManager roleManager,
            IApplicationUserManager userManager,
            SignInManager<User> signInManager,
            ILogger<ManageController> logger,
            IHttpContextAccessor accessor,
            IMapper mapper,
            IWebHostEnvironment env ,
            IEmailSender emailSender,
            ISmsSender smsSender)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _accessor = accessor;
            _mapper = mapper;
            _env = env;
            _emailSender = emailSender;
            _smsSender = smsSender;
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
        [HttpGet]
        public IActionResult ExternalLogin(string provider, string returnUrl = null)
        {
            var redirectUrl = Url.Action(nameof(ExternalLoginCallback), "Manage", new { returnUrl });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return Challenge(properties, provider);
        }

        [HttpGet]
        public async Task<IActionResult> ExternalLoginCallback(string returnUrl = null, string remoteError = null)
        {
            if (remoteError != null)
            {
                ModelState.AddModelError(string.Empty, $"Error from external provider: {remoteError}");
                return View("Signin");
            }

            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                ModelState.AddModelError(string.Empty, "Error loading external login information.");
                return View("Signin");
            }

            var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);
            if (result.Succeeded)
            {
                _logger.LogInformation("{Name} logged in with {LoginProvider} provider.", info.Principal.Identity.Name, info.LoginProvider);
                return RedirectToAction("Index", "Dashboard");
            }

            if (result.IsLockedOut)
            {
                return View("Signin");
            }
            else
            {
                //ViewData["ReturnUrl"] = returnUrl;
                //ViewData["LoginProvider"] = info.LoginProvider;
                //var email = info.Principal.FindFirstValue(ClaimTypes.Email);
                //return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = email });
                return View("Signin");
            }
        }
        [HttpGet]
        public async Task<IActionResult> SendCode(bool RememberMe)
        {
            var FactorOptions = new List<SelectListItem>();
            var User = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (User == null)
                return NotFound();

            var UserFactors = await _userManager.GetValidTwoFactorProvidersAsync(User);
            foreach (var item in UserFactors)
            {
                if (item == "Authenticator")
                {
                    FactorOptions.Add(new SelectListItem { Text = "اپلیکشن احراز هویت", Value = item });
                }

                else
                {
                    FactorOptions.Add(new SelectListItem { Text = (item == "Email" ? "ارسال ایمیل" : "ارسال پیامک"), Value = item });
                }
            }
            return View(new SendCodeViewModel { Providers = FactorOptions, RememberMe = RememberMe });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendCode(SendCodeViewModel ViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(ViewModel);
            }

            var User = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (User == null)
                return NotFound();

            if (ViewModel.SelectedProvider != "Authenticator")
            {
                var Code = await _userManager.GenerateTwoFactorTokenAsync(User, ViewModel.SelectedProvider);
                if (string.IsNullOrWhiteSpace(Code))
                    return View("Error");

                var Message = "<p style='direction:rtl;font-size:14px;font-family:tahoma'>کد اعتبارسنجی شما :" + Code + "</p>";

                if (ViewModel.SelectedProvider == "Email")
                    await _emailSender.SendEmailAsync(User.Email, "کد اعتبارسنجی", Message);

                else if (ViewModel.SelectedProvider == "Phone")
                {
                    string ResponseSms = await _smsSender.SendAuthSmsAsync(Code, User.PhoneNumber);
                    if (ResponseSms == "Failed")
                    {
                        ModelState.AddModelError(string.Empty, "در ارسال پیامک خطایی رخ داده است.");
                        return View(ViewModel);
                    }

                }

                return RedirectToAction("VerifyCode", new { Provider = ViewModel.SelectedProvider, RememberMe = ViewModel.RememberMe });

            }

            else
                return View(ViewModel);

        }
        [HttpGet]
        public async Task<IActionResult> VerifyCode(string Provider, bool RememberMe)
        {
            var User = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (User == null)
                return NotFound();
            return View(new VerifyCodeViewModel { Provider = Provider, RememberMe = RememberMe });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> VerifyCode(VerifyCodeViewModel ViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(ViewModel);
            }

            var Result = await _signInManager.TwoFactorSignInAsync(ViewModel.Provider, ViewModel.Code, ViewModel.RememberMe, ViewModel.RememberBrowser);
            if (Result.Succeeded)
            {
                return RedirectToAction("Index", "Dashboard");
            }

            else if (Result.IsLockedOut)
            {
                ModelState.AddModelError(string.Empty, "حساب کاربری شما به دلیل تلاش های ناموفق به مدت 20 دقیقه قفل شد.");
            }

            else
            {
                ModelState.AddModelError(string.Empty, "کد اعتبارسنجی صحیح نمی باشد");
            }

            return View(ViewModel);

        }
    }
}