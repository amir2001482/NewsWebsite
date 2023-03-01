using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using NewsWebsite.Services.Contracts;
using NewsWebsite.ViewModels.RoleManager;
using NewsWebsite.ViewModels.UserManager;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System;
using NewsWebsite.Common;
using NewsWebsite.Entities.identity;

namespace NewsWebsite.Services.Identity
{
    public class ApplicationRoleManager : RoleManager<Role> , IApplicationRoleManager
    {
        private readonly IdentityErrorDescriber _errors;
        private readonly IApplicationUserManager _userManager;
        private readonly ILookupNormalizer _keyNormalizer;
        private readonly ILogger<ApplicationRoleManager> _logger;
        private readonly IEnumerable<IRoleValidator<Role>> _roleValidators;
        private readonly IRoleStore<Role> _store;

        public ApplicationRoleManager(
            IRoleStore<Role> store,
            ILookupNormalizer keyNormalizer,
            ILogger<ApplicationRoleManager> logger,
            IEnumerable<IRoleValidator<Role>> roleValidators,
            IdentityErrorDescriber errors,
            IApplicationUserManager userManager) :
            base(store, roleValidators, keyNormalizer, errors, logger)
        {
            _errors = errors;
            _errors.CheckArgumentIsNull(nameof(_errors));

            _keyNormalizer = keyNormalizer;
            _keyNormalizer.CheckArgumentIsNull(nameof(_keyNormalizer));

            _logger = logger;
            _logger.CheckArgumentIsNull(nameof(_logger));

            _store = store;
            _store.CheckArgumentIsNull(nameof(_store));

            _roleValidators = roleValidators;
            _roleValidators.CheckArgumentIsNull(nameof(_roleValidators));

            _userManager = userManager;
            _userManager.CheckArgumentIsNull(nameof(_userManager));
        }


        public List<Role> GetAllRoles()
        {
            return Roles.ToList();
        }


        public List<RolesViewModel> GetAllRolesAndUsersCount()
        {
            return Roles.Select(role =>
                             new RolesViewModel
                             {
                                 Id = role.Id,
                                 Name = role.Name,
                                 Description = role.Description,
                                 UsersCount = role.Users.Count(),
                             }).ToList();
        }

        public Task<Role> FindClaimsInRole(int roleId)
        {
            return Roles.Include(c => c.Claims).FirstOrDefaultAsync(c => c.Id == roleId);
        }


        public async Task<IdentityResult> AddOrUpdateClaimsAsync(int roleId,string roleClaimType,IList<string> SelectedRoleClaimValues)
        {
            var Role = await FindClaimsInRole(roleId);
            if(Role==null)
            {
                return IdentityResult.Failed(new IdentityError
                {
                    Code = "NotFound",
                    Description = "نقش مورد نظر یافت نشد.",
                });
            }

            var CurrentRoleClaimValues = Role.Claims.Where(r => r.ClaimType == roleClaimType).Select(r => r.ClaimValue).ToList();
            if (SelectedRoleClaimValues == null)
                SelectedRoleClaimValues = new List<string>();

            var NewClaimValuesToAdd = SelectedRoleClaimValues.Except(CurrentRoleClaimValues).ToList();
            foreach(var claim in NewClaimValuesToAdd)
            {
                Role.Claims.Add(new RoleClaim
                {
                    RoleId=roleId,
                    ClaimType=roleClaimType,
                    ClaimValue=claim,
                });
            }

            var removedClaimValues = CurrentRoleClaimValues.Except(SelectedRoleClaimValues).ToList();
            foreach(var claim in removedClaimValues)
            {
                var roleClaim = Role.Claims.SingleOrDefault(r => r.ClaimValue == claim && r.ClaimType == roleClaimType);
                if (roleClaim != null)
                    Role.Claims.Remove(roleClaim);
            }

            return await UpdateAsync(Role);
        }

        public async Task<List<UsersViewModel>> GetUsersInRoleAsync(int RoleId)
        {
            var userIds = (from r in Roles
                           where (r.Id == RoleId)
                           from u in r.Users
                           select u.UserId).ToList();

            return await _userManager.Users.Where(user => userIds.Contains(user.Id))
                .Select(user => new UsersViewModel
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
                    Roles = user.Roles
                }).AsNoTracking().ToListAsync();
        }


        public async Task<List<RolesViewModel>> GetPaginateRolesAsync(int offset, int limit, bool? roleNameSortAsc, string searchText)
        {
            List<RolesViewModel> roles;
            roles = await Roles.Where(r => r.Name.Contains(searchText)).Select(role => new RolesViewModel
            {
                Id = role.Id,
                Name = role.Name,
                Description = role.Description,
                UsersCount = role.Users.Count()
            }).Skip(offset).Take(limit).ToListAsync();

            if (roleNameSortAsc != null)
               roles = roles.OrderBy(t => (roleNameSortAsc == true && roleNameSortAsc != null) ? t.Name : "").OrderByDescending(t => (roleNameSortAsc == false && roleNameSortAsc != null) ? t.Name : "").ToList();

            foreach (var item in roles)
                item.Row = ++offset;

            return roles;
        }
    }
}
