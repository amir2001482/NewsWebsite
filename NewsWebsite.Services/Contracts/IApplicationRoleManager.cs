using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NewsWebsite.ViewModels.RoleManager;
using NewsWebsite.ViewModels.UserManager;
using NewsWebsite.Entities.Identity;

namespace NewsWebsite.Services.Contracts
{
    public interface IApplicationRoleManager
    {
        #region BaseClass
        IQueryable<Role> Roles { get; }
        ILookupNormalizer KeyNormalizer { get; set; }
        IdentityErrorDescriber ErrorDescriber { get; set; }
        IList<IRoleValidator<Role>> RoleValidators { get; }
        bool SupportsQueryableRoles { get; }
        bool SupportsRoleClaims { get; }
        Task<IdentityResult> CreateAsync(Role role);
        Task<IdentityResult> DeleteAsync(Role role);
        Task<Role> FindByIdAsync(string roleId);
        Task<Role> FindByNameAsync(string roleName);
        string NormalizeKey(string key);
        Task<bool> RoleExistsAsync(string roleName);
        Task<IdentityResult> UpdateAsync(Role role);
        Task UpdateNormalizedRoleNameAsync(Role role);
        Task<string> GetRoleNameAsync(Role role);
        Task<IdentityResult> SetRoleNameAsync(Role role, string name);
        #endregion


        #region CustomMethod
        List<Role> GetAllRoles();
        List<RolesViewModel> GetAllRolesAndUsersCount();
        Task<Role> FindClaimsInRole(int RoleId);
        Task<List<UsersViewModel>> GetUsersInRoleAsync(int RoleId);
        Task<IdentityResult> AddOrUpdateClaimsAsync(int RoleId, string RoleClaimType, IList<string> SelectedRoleClaimValues);
        Task<List<RolesViewModel>> GetPaginateRolesAsync(int offset, int limit, bool? roleNameSortAsc, string searchText);
        #endregion
    }
}
