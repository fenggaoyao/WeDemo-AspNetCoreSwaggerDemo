using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspNetCoreSwaggerDemo.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using AspNetCoreSwaggerDemo.Utils;

namespace AspNetCoreSwaggerDemo.Controllers
{
    /// <summary>
    /// 测试
    /// </summary>
    [UserAuthorize]
    public class ValuesController : BaseApiController
    {
        [HttpGet]
        public IActionResult Get()
        {
            dynamic result = MysqlHelper.Search();
            return Ok(new
            {
                UserName = result               
            });
        }

        [HttpGet]
        public IActionResult Hello()
        {
            return Ok("hello world");
        }





        [HttpPost]
        public IActionResult TestPost(string item)
        {
            int result= MysqlHelper.Insert();
            return Ok( new {
                status= result,
                msg=item                
            });

        }

    }
}