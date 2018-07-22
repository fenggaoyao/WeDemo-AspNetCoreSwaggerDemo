using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Http;
using AspNetCoreSwaggerDemo.Extensions;

namespace AspNetCoreSwaggerDemo
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        private const string _Project_Name = "AspNetCoreSwagger";//nameof(AspNetCoreSwaggerDemo);

        // This method gets called by the runtime. Use this method to add services to the container.
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
                c.OperationFilter<AssignOperationVendorExtensions>();
                c.DocumentFilter<ApplyTagDescriptions>();
            });

            services.AddMvc();
            return services.BuildServiceProvider();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            //if (env.IsDevelopment())
            //{
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    typeof(ApiVersions).GetEnumNames().OrderByDescending(e => e).ToList().ForEach(version =>
                    {
                        c.SwaggerEndpoint($"/swagger/{version}/swagger.json", $"{_Project_Name} {version}");
                    });
                    //注入汉化文件
                    c.InjectOnCompleteJavaScript($"/swagger_translator.js");
                });
            //}
            ServiceLocator.Configure(app.ApplicationServices);
            app.UseStaticFiles();
            app.UseMvc();
        }
    }
}
