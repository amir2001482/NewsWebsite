using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NewsWebsite.Common;
using NewsWebsite.Common.Api;
using NewsWebsite.Common.Api.Attributes;
using NewsWebsite.Services.Api.Contract;
using NewsWebsite.Services.Contracts;
using NewsWebsite.ViewModels.Api.UsersApi;
using NewsWebsite.ViewModels.DynamicAccess;
using NewsWebsite.ViewModels.Models;
using NewsWebsite.ViewModels.UserManager;

namespace NewsWebsite.Areas.Api.Controllers.v1
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiResultFilter]
    [ApiVersion("1")]
    public class UsersApiController : ControllerBase
    {
        private readonly IApplicationUserManager _userManager;
        private readonly IjwtService _jwtService;
        public UsersApiController(IApplicationUserManager userManager, IjwtService jwtService)
        {
            _userManager = userManager;
            _jwtService = jwtService;
        }

        [HttpGet]
        [JwtAuthentication(Policy = ConstantPolicies.DynamicPermission)]
        public virtual async Task<ApiResult<List<UsersViewModel>>> Get(int offset, int limit, string order, string search)
        {
            if (!search.HasValue())
                search = "";
            var model = new PaginateModel()
            {
                limit = limit,
                offset = offset,
                orderBy = order,
                searchText = search
            };
            return Ok(await _userManager.GetPaginateUsersAsync(model));
        }

        [HttpGet("{id}")]
        public virtual async Task<ApiResult<UsersViewModel>> Get(int id)
        {
            var user = await _userManager.FindUserWithRolesByIdAsync(id);
            if (user == null)
                return NotFound();
            else
                return Ok(user);
        }

        [HttpPost("[action]")]
        public virtual async Task<ApiResult<string>> SignIn([FromBody] SignInBaseViewModel ViewModel)
        {
            var User = await _userManager.FindByNameAsync(ViewModel.UserName);
            if (User == null)
                return BadRequest("نام کاربری یا کلمه عبور شما صحیح نمی باشد.");
            else
            {
                var result = await _userManager.CheckPasswordAsync(User, ViewModel.Password);
                if (result)
                    return Ok(await _jwtService.GenerateTokenAsync(User));
                else
                    return BadRequest("نام کاربری یا کلمه عبور شما صحیح نمی باشد.");
            }
        }
    }
}