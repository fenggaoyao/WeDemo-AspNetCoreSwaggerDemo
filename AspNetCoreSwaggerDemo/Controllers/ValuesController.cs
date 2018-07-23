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
        public IActionResult HelloWorld()
        {
            return Ok("hello world");
        }

        [HttpGet]
        public IActionResult Jenkins()
        {
            return Ok("Jenkins+Docker自动化");
        }

        [HttpGet]
        public IActionResult test()
        {
            Random rnext = new Random();            
            return Ok(rnext.Next(1, 100));
        }

        [HttpGet]
        public IActionResult Gauge()
        {
            return Ok(new {
                width=800,
                height =800,
                stateData =new [] {
                    new {
                    state="停机",
                    value=0.3,
                    color= "'#F00"
                    },

                     new {
                    state="换模",
                    value=0.5,
                    color= "'#63F"
                    },

                      new {
                    state="生产",
                    value=0.2,
                    color= "'#3E3"
                    }

                } ,
                curState=new {
                    state="生产",
                    value=60,
                    color= "#3E3"
                },
                curProgress=30,
                delivery= "3day"
            });
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