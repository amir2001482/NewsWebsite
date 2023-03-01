using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NewsWebsite.Entities.identity;
using NewsWebsite.Services.Contracts;
using NewsWebsite.ViewModels.UserManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.IO;
using NewsWebsite.Common;
using AutoMapper;
using AutoMapper.QueryableExtensions;

namespace NewsWebsite.Services.Identity
{
    public class ApplicationUserManager : UserManager<User>, IApplicationUserManager
    {
        private readonly ApplicationIdentityErrorDescriber _errors;
        private readonly ILookupNormalizer _keyNormalizer;
        private readonly ILogger<ApplicationUserManager> _logger;
        private readonly IOptions<IdentityOptions> _options;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly IEnumerable<IPasswordValidator<User>> _passwordValidators;
        private readonly IServiceProvider _services;
        private readonly IUserStore<User> _userStore;
        private readonly IEnumerable<IUserValidator<User>> _userValidators;
        private readonly IMapper _mapper;

        public ApplicationUserManager(
            ApplicationIdentityErrorDescriber errors,
            ILookupNormalizer keyNormalizer,
            ILogger<ApplicationUserManager> logger,
            IOptions<IdentityOptions> options,
            IPasswordHasher<User> passwordHasher,
            IEnumerable<IPasswordValidator<User>> passwordValidators,
            IServiceProvider services,
            IUserStore<User> userStore,
            IEnumerable<IUserValidator<User>> userValidators,
            IMapper mapper)
            : base(userStore, options, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger)
        {
            _userStore = userStore;
            _errors = errors;
            _logger = logger;
            _services = services;
            _passwordHasher = passwordHasher;
            _userValidators = userValidators;
            _options = options;
            _keyNormalizer = keyNormalizer;
            _passwordValidators = passwordValidators;
            _mapper = mapper;
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            return await Users.ToListAsync();
        }

        public async Task<List<UsersViewModel>> GetAllUsersWithRolesAsync()
        {
            return await Users.Select(user => new UsersViewModel
            {
                Id = user.Id,
                Email = user.Email,
                UserName = user.UserName,
                PhoneNumber = user.PhoneNumber,
                FirstName = user.FirstName,
                LastName = user.LastName,
                BirthDate = user.BirthDate,
                IsActive = user.IsActive,
                Image = user.Image,
                RegisterDateTime = user.RegisterDateTime,
                Roles = user.Roles,

            }).ToListAsync();
        }

        public async Task<UsersViewModel> FindUserWithRolesByIdAsync(int UserId)
        {
            return await Users.Where(u => u.Id == UserId).Select(user => new UsersViewModel
            {
                Id = user.Id,
                Email = user.Email,
                UserName = user.UserName,
                PhoneNumber = user.PhoneNumber,
                FirstName = user.FirstName,
                LastName = user.LastName,
                BirthDate = user.BirthDate,
                IsActive = user.IsActive,
                Image = user.Image,
                RegisterDateTime = user.RegisterDateTime,
                Roles = user.Roles,
                AccessFailedCount = user.AccessFailedCount,
                EmailConfirmed = user.EmailConfirmed,
                LockoutEnabled = user.LockoutEnabled,
                LockoutEnd = user.LockoutEnd,
                PhoneNumberConfirmed = user.PhoneNumberConfirmed,
                TwoFactorEnabled = user.TwoFactorEnabled,
                Gender = user.Gender,
            }).FirstOrDefaultAsync();
        }

        public async Task<string> GetFullName(ClaimsPrincipal User)
        {
            var UserInfo = await GetUserAsync(User);
            return UserInfo.FirstName + " " + UserInfo.LastName;
        }


        public async Task<List<UsersViewModel>> GetPaginateUsersAsync(int offset, int limit, bool? firstnameSortAsc, bool? lastnameSortAsc, bool? emailSortAsc, bool? usernameSortAsc,bool? registerDateTimeSortAsc, string searchText)
        {
            var users = await Users.Include(u => u.Roles).Where(t => t.FirstName.Contains(searchText) || t.LastName.Contains(searchText) || t.Email.Contains(searchText) || t.UserName.Contains(searchText) || t.RegisterDateTime.ConvertMiladiToShamsi("yyyy/MM/dd ساعت hh:mm:ss").Contains(searchText))
                    .Select(user => new UsersViewModel
                    {
                        Id = user.Id,
                        Email = user.Email,
                        UserName = user.UserName,
                        PhoneNumber = user.PhoneNumber,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        IsActive = user.IsActive,
                        Image = user.Image,
                        PersianBirthDate = user.BirthDate.ConvertMiladiToShamsi("yyyy/MM/dd"),
                        PersianRegisterDateTime = user.RegisterDateTime.ConvertMiladiToShamsi("yyyy/MM/dd ساعت hh:mm:ss"),
                        GenderName = user.Gender == GenderType.male ? "مرد" : "زن",
                        RoleId = user.Roles.Select(r => r.Role.Id).FirstOrDefault(),
                        RoleName = user.Roles.Select(r => r.Role.Name).FirstOrDefault()
                    }).Skip(offset).Take(limit).ToListAsync();

            if (firstnameSortAsc != null)
            {
                users = users.OrderBy(t => (firstnameSortAsc == true && firstnameSortAsc != null) ? t.FirstName : "").OrderByDescending(t => (firstnameSortAsc == false && firstnameSortAsc != null) ? t.FirstName : "").ToList();
            }

            else if (lastnameSortAsc != null)
            {
                users = users.OrderBy(t => (lastnameSortAsc == true && lastnameSortAsc != null) ? t.LastName : "").OrderByDescending(t => (lastnameSortAsc == false && lastnameSortAsc != null) ? t.LastName : "").ToList();
            }

            else if (emailSortAsc != null)
            {
                users = users.OrderBy(t => (emailSortAsc == true && emailSortAsc != null) ? t.Email : "").OrderByDescending(t => (emailSortAsc == false && emailSortAsc != null) ? t.Email : "").ToList();
            }

            else if (usernameSortAsc != null)
            {
                users = users.OrderBy(t => (usernameSortAsc == true && usernameSortAsc != null) ? t.PhoneNumber : "").OrderByDescending(t => (usernameSortAsc == false && usernameSortAsc != null) ? t.UserName : "").ToList();
            }

            else if (registerDateTimeSortAsc != null)
            {
                users = users.OrderBy(t => (registerDateTimeSortAsc == true && registerDateTimeSortAsc != null) ? t.PersianRegisterDateTime : "").OrderByDescending(t => (registerDateTimeSortAsc == false && registerDateTimeSortAsc != null) ? t.PersianRegisterDateTime : "").ToList();
            }

            foreach (var item in users)
                item.Row = ++offset;

            return users;
        }

        public string CheckAvatarFileName(string fileName)
        {
            string fileExtension = Path.GetExtension(fileName);
            int fileNameCount = Users.Where(f => f.Image == fileName).Count();
            int j = 1;
            while (fileNameCount != 0)
            {
                fileName = fileName.Replace(fileExtension, "") + j + fileExtension;
                fileNameCount = Users.Where(f => f.Image == fileName).Count();
                j++;
            }

            return fileName;
        }
    }
}
