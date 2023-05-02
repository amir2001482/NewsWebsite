using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NewsWebsite.Data.Contracts;
using NewsWebsite.Entities.identity;
using NewsWebsite.Services.Contracts;
using NewsWebsite.ViewModels.Account;
using NewsWebsite.ViewModels.Manage;
using NewsWebsite.Common;
using NewsWebsite.Common.Attributes;
using NewsWebsite.Entities;
using Microsoft.AspNetCore.Authorization;
using NewsWebsite.Areas.Admin.Controllers;
using System.Security.Claims;
using AutoMapper;
using NewsWebsite.ViewModels.UserManager;
using Microsoft.AspNetCore.Hosting;

namespace NewsWebsite.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUnitOfWork _uw;
        private readonly IHttpContextAccessor _accessor;
        private readonly IApplicationUserManager _userManager;
        private readonly IApplicationRoleManager _roleManager;
        private readonly IEmailSender _emailSender;
        private readonly SignInManager<User> _signInManager;
        private readonly ILogger<AccountController> _logger;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _env;
        private const string BookmarkNotFound = "خبر بوکمارک شده یافت نشد.";
        public AccountController(IUnitOfWork uw,
            IHttpContextAccessor accessor,
            IApplicationUserManager userManager,
            IApplicationRoleManager roleManager,
            IEmailSender emailSender,
            SignInManager<User> signInManager,
            ILogger<AccountController> logger,
            IMapper mapper,
            IWebHostEnvironment env)
        {
            _uw = uw;
            _accessor = accessor;
            _userManager = userManager;
            _roleManager = roleManager;
            _emailSender = emailSender;
            _signInManager = signInManager;
            _logger = logger;
            _mapper = mapper;
            _env = env;
        }

        [HttpGet]
        public IActionResult SignIn()
        {
            return PartialView("_SignIn");
        }
        [HttpPost]
        public async Task<IActionResult> SignIn(SignInViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var User = await _userManager.FindByNameAsync(viewModel.UserName);
                if (User != null)
                {
                    if (User.IsActive)
                    {
                        var result = await _signInManager.PasswordSignInAsync(viewModel.UserName, viewModel.Password, viewModel.RememberMe, true);
                        if (result.Succeeded)
                            return Json("success");

                        else if (result.IsLockedOut)
                            ModelState.AddModelError(string.Empty, "حساب کاربری شما به مدت 20 دقیقه به دلیل تلاش های ناموفق قفل شد.");

                        else if (result.RequiresTwoFactor)
                            ModelState.AddModelError(string.Empty, "احراز هویت دو مرحله ای برای شما فعال شده است لطفا از صفحه ادمین اقدام به ورود کنید.");

                        else
                        {
                            _logger.LogWarning($"The user attempts to login with the IP address({_accessor.HttpContext?.Connection?.RemoteIpAddress.ToString()}) and username ({viewModel.UserName}) and password ({viewModel.Password}).");
                            ModelState.AddModelError(string.Empty, "نام کاربری یا کلمه عبور شما صحیح نمی باشد.");
                        }
                    }
                    else
                        ModelState.AddModelError(string.Empty, "حساب کاربری شما غیرفعال است.");
                }

                else
                {
                    _logger.LogWarning($"The user attempts to login with the IP address({_accessor.HttpContext?.Connection?.RemoteIpAddress.ToString()}) and username ({viewModel.UserName}) and password ({viewModel.Password}).");
                    ModelState.AddModelError(string.Empty, "نام کاربری یا کلمه عبور شما صحیح نمی باشد.");
                }
            }

            return PartialView("_SignIn");
        }
        [HttpGet]
        public IActionResult Register()
        {
            return PartialView("_Register");
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var user = new User { UserName = viewModel.UserName, Email = viewModel.Email, RegisterDateTime = DateTime.Now, IsActive = true  , FirstName ="" , LastName = "" };
                IdentityResult result = await _userManager.CreateAsync(user, viewModel.Password);

                if (result.Succeeded)
                {
                    var role = await _roleManager.FindByNameAsync("کاربر");
                    if (role == null)
                        await _roleManager.CreateAsync(new Role("کاربر"));

                    result = await _userManager.AddToRoleAsync(user, "کاربر");

                    if (result.Succeeded)
                    {
                        try
                        {
                            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                            var callbackUrl = Url.Action("ConfirmEmail", "Account", values: new { userId = user.Id, code = code }, protocol: Request.Scheme);
                            await _emailSender.SendEmailAsync(viewModel.Email, "تایید حساب کاربری - سایت میزفا", $"<div dir='rtl' style='font-family:tahoma;font-size:14px'>لطفا با کلیک روی لینک رویه رو حساب کاربری خود را فعال کنید.  <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>کلیک کنید</a></div>");

                            TempData["notification"] = $" ایمیل فعال سازی حساب کاربری به {viewModel.Email} ارسال شد. ";
                        }
                        catch (Exception ex)
                        {

                        }
                    }
                }

                ModelState.AddErrorsFromResult(result);
            }

            return PartialView("_Register");
        }
        public async Task<IActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
                return RedirectToAction("Index", "Home");

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound($"Unable to load user with ID '{userId}'");

            var result = await _userManager.ConfirmEmailAsync(user, code);
            if (!result.Succeeded)
                throw new InvalidOperationException($"Error Confirming email for user with ID '{userId}'");

            return View();
        }
        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            int userId = User.Identity.GetUserId<int>();
            var user = await _userManager.FindByIdAsync(userId.ToString());
            var books = await _uw.NewsRepository.GetUserBookmarksAsync(userId);
            var profile = _mapper.Map<ProfileViewModel>(user);
            profile.BookmarkNews = books;
            return View(profile);
        }
        [HttpPost]
        public async Task<IActionResult> EditProfile(ProfileViewModel viewModel)
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

                    viewModel.BirthDate = viewModel.PersianBirthDate?.ConvertShamsiToMiladi();
                    var result = await _userManager.UpdateAsync(_mapper.Map(viewModel, user));
                    if (result.Succeeded)
                        return RedirectToAction( "Index", "Home");
                }
                return RedirectToAction(nameof(Profile));
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SignOut()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
        [HttpGet, AjaxOnly]
        public IActionResult DeleteBookmark(int userId, string newsId)
        {
            if (userId == 0 || !newsId.HasValue())
                ModelState.AddModelError(string.Empty, BookmarkNotFound);
            else
            {
                var bookmark = _uw.BaseRepository<Bookmark>().FindByConditionAsync(b => b.NewsId == newsId && b.UserId == userId).Result.FirstOrDefault();
                if (bookmark == null)
                    ModelState.AddModelError(string.Empty, BookmarkNotFound);
                else
                    return PartialView("_DeleteConfirmation", bookmark);
            }
            return PartialView("_DeleteConfirmation");
        }
        [HttpPost, ActionName("DeleteBookmark"), AjaxOnly]
        public async Task<IActionResult> DeleteBookmarkConfirmed(Bookmark model)
        {
            if (model.NewsId == null)
                ModelState.AddModelError(string.Empty, BookmarkNotFound);
            else
            {
                var bookmark = _uw.BaseRepository<Bookmark>().FindByConditionAsync(b => b.UserId == model.UserId && b.NewsId == model.NewsId).Result.FirstOrDefault();
                if (bookmark == null)
                    ModelState.AddModelError(string.Empty, BookmarkNotFound);
                else
                {
                    _uw.BaseRepository<Bookmark>().Delete(bookmark);
                    await _uw.Commit();
                    return PartialView("_Bookmarks", await _uw.NewsRepository.GetUserBookmarksAsync(model.UserId));
                }
            }
            return PartialView("_DeleteConfirmation");
        }
        [HttpGet]
        public async Task<IActionResult> ChangePassword()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return NotFound();
            return PartialView("_ChangePassword" , new ChangePasswordViewModel());
        }
        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel ViewModel)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return NotFound();

            if (ModelState.IsValid)
            {
                var ChangePassResult = await _userManager.ChangePasswordAsync(user, ViewModel.OldPassword, ViewModel.NewPassword);
                if (ChangePassResult.Succeeded)
                    return RedirectToAction("Index", "Home");
            }
            return RedirectToAction(nameof(Profile));
        }
       
    }
}