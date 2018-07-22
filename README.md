## asp.net core中使用Swashbuckle.AspNetCore(swagger)生成接口文档        
> Swashbuckle.AspNetCore:swagger的asp.net core实现          

### demo预览
![图片](https://dn-coding-net-production-pp.qbox.me/0772a4d2-6949-4888-948a-ba98a34d840d.png)       
如上图所示，包含功能如下（完整示例见文末）      
1. 基础使用,添加controler的说明(`IDocumentFilter`)      
2. 汉化操作按钮     
3. 添加通用参数(header)-实现`IOperationFilter`      
4. 多版本控制(暂时见demo)       
5. 使用JWT的简单接口验证(暂时见demo)    


## 构建一个webapi项目并使用Swashbuckle.AspNetCore(swagger)      
1. 新建asp.net core webapi项目 `dotnet new webapi`      
2. 安装nuget包：[`Swashbuckle.AspNetCore`](https://github.com/domaindrivendev/Swashbuckle.AspNetCore),本文使用版本1.1.0,.net core版本2.0+       
3. 编辑解决方案添加(或者在vs中项目属性->生成->勾选生成xml文档文件)如下配置片段      
```
      <PropertyGroup Condition="'$(Configuration)|$(Platform)'=='Debug|AnyCPU'">
        <DocumentationFile>bin\Debug\netcoreapp2.0\项目名称.xml</DocumentationFile>
      </PropertyGroup>
```
    
4. 使用Swagger并注入汉化脚本

> `c.SwaggerDoc`配置接口描述信息        
> `c.OperationFilter`可通过`IOperationFilter`接口去添加一些公共的参数       
> `c.DocumentFilter`通过`IDocumentFilter`接口去生成控制器的标签(描述)       
> 注：`ConfigureServices`的方法返回值修改了，为了能够正常的使用`ServiceLocator`获取服务     

``` csharp
private const string _Project_Name = "AspNetCoreSwaggerDemo";//nameof(AspNetCoreSwaggerDemo);
public IServiceProvider ConfigureServices(IServiceCollection services)
{
    services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
    services.AddSingleton(new ApiTokenConfig("A3FFB16D-D2C0-4F25-BACE-1B9E5AB614A6"));
    services.AddScoped<IApiTokenService, ApiTokenService>();
    services.AddSwaggerGen(c =>
    {
        typeof(ApiVersions).GetEnumNames().ToList().ForEach(version =>
        {
            c.SwaggerDoc(version, new Swashbuckle.AspNetCore.Swagger.Info
             {
                 Version = version,
                 Title = $"{_Project_Name} 接口文档",
                 Description = $"{_Project_Name} HTTP API " + version,
                 TermsOfService = "None"
             });
        });
        var basePath = Microsoft.Extensions.PlatformAbstractions.PlatformServices.Default.Application.ApplicationBasePath;
        var xmlPath = System.IO.Path.Combine(basePath, $"{_Project_Name}.xml");
        c.IncludeXmlComments(xmlPath);
        //添加自定义参数，可通过一些特性标记去判断是否添加
        c.OperationFilter<AssignOperationVendorExtensions>();
        //添加对控制器的标签(描述)
        c.DocumentFilter<ApplyTagDescriptions>();
    });

    services.AddMvc();
    return services.BuildServiceProvider();
}
```
    
> 使用`InjectOnCompleteJavaScript`注入[汉化js脚本](https://coding.net/u/yimocoding/p/WeDemo/git/blob/AspNetCoreSwaggerDemo/AspNetCoreSwaggerDemo/wwwroot/swagger_translator.js)即可      
> 注：我在这个汉化脚本中添加了保存和读取赋值token/版本的js代码      
> `ApiVersions`为枚举，配置api版本，以期通过`CustomRoute`特性标记解决历史api问题。     

``` csharp
public void Configure(IApplicationBuilder app, IHostingEnvironment env)
{
    
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        //ApiVersions为自定义的版本枚举
        typeof(ApiVersions)
        .GetEnumNames()
        .OrderByDescending(e => e)
        .ToList()
        .ForEach(version =>
        {
            //版本控制
            c.SwaggerEndpoint($"/swagger/{version}/swagger.json", $"{_Project_Name} {version}");
         });
         //注入汉化脚本
         c.InjectOnCompleteJavaScript($"/swagger_translator.js");
    });
    //通过ServiceLocator.Resolve<Service接口>()获取注入的服务
    ServiceLocator.Configure(app.ApplicationServices);
    app.UseStaticFiles();
    app.UseMvc();
}
```     

## 实现`IDocumentFilter`及`IOperationFilter`                
> 通过`IOperationFilter`接口可以添加一些公共的参数,添加参数到header或者上传图片等                  
> 通过`IDocumentFilter`接口可以生成控制器的标签(描述)                   
> 调用方式分别为：          
>     `c.OperationFilter<AssignOperationVendorExtensions>();`       
>      `c.DocumentFilter<ApplyTagDescriptions>();`

``` csharp
public class ApplyTagDescriptions : IDocumentFilter
{
    public void Apply(SwaggerDocument swaggerDoc, DocumentFilterContext context)
    {
        swaggerDoc.Tags = new List<Tag>
        {
            //添加对应的控制器描述 这个是好不容易在issues里面翻到的
            new Tag { Name = "Account", Description = "登录相关接口" },
            new Tag { Name = "UserCenter", Description = "用户中心接}
        };
    }
}
//添加通用参数，若in='header'则添加到header中,默认query参数
public class AssignOperationVendorExtensions : IOperationFilter
{
    public void Apply(Operation operation, OperationFilterContext context)
    {
        operation.Parameters = operation.Parameters ?? new List<IParameter>();
        //MemberAuthorizeAttribute 自定义的身份验证特性标记
        var isAuthor = operation != null && context != null && context.ApiDescription.ControllerAttributes().Any(e => e.GetType() == typeof(MemberAuthorizeAttribute)) || context.ApiDescription.ActionAttributes().Any(e => e.GetType() == typeof(MemberAuthorizeAttribute));
        if (isAuthor)
        {
            //in query header 
            operation.Parameters.Add(new NonBodyParameter() { 
                    Name = "x-token", 
                    In = "header", //query formData ..
                    Description = "身份验证票据", 
                    Required = false, 
                    Type = "string" 
           });
        }
    }
}
```

配置完成后，给Controler，Action添加上注释和请求类型就可以访问/swagger查看你的api文档了~     
注：    
1. action方法或者控制器(或者继承的)必须有一个包含`[Route("")]`特性标记      
2. action方法必须添加请求类型`[HttpGet]/[HttpPost]/..`      
    
### 如何自动将token保存并赋值       
> 使用js生成了文本框到`.authorize-wrapper`,将值保存到了本地存储中,然后会根据接口版本将版本号参数进行复制        

``` js
$(function () {
    //汉化
    window.SwaggerTranslator.translate();
    /***************下面是添加的token相关代码*******************/
    window.saveToken=function() {
        var test_token = $("#customtoken").val();
        localStorage.setItem("test_x_token", $("#customtoken").val());
        $("input[name='X-Token']").val(test_token)
    }
    //token保存
    var test_token = localStorage.getItem("test_x_token")||"";
    $(".authorize-wrapper").append("X-Token：<input type='text' id='customtoken' value='" + test_token +"' style='width:50%' /> <button onClick='saveToken()'>保存</button>")
    $("input[name='X-Token']").val(test_token)
    $("input[name='X-Version']").val(swaggerUi.api.info.version)

});
```
