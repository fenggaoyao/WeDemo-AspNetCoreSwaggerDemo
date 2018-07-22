using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AspNetCoreSwaggerDemo.Extensions;

namespace AspNetCoreSwaggerDemo.Controllers
{
    /// <summary>
    /// 登录接口
    /// </summary>
    public class AccountController : BaseApiController
    {
        private readonly IApiTokenService _apiTokenService;
        public AccountController(IApiTokenService apiTokenService)
        {
            _apiTokenService = apiTokenService;
        }
        /// <summary>
        /// 登录
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Login(string userName, string userPwd)
        {
            //login...
            if (userName != "test" && userPwd != "1234")
                return Ok(new { status = 0, msg = "登录失败" });
            var userId = 1;
            var token = _apiTokenService.ConvertLoginToken(userId, userName);
            //登录成功后返回token
            return Ok(new { status = 1, msg = "登录成功", data = token });
        }

    }
}
