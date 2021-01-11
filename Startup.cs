using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using H4WebApi.Models;
using System;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.AspNetCore.Http;
using Quartz.Impl;
using Quartz;
using log4net.Repository;
using log4net;
using log4net.Config;
using System.IO;
using H4WebApi.Services;
using Microsoft.Extensions.FileProviders;

namespace H4WebApi
{
    public class Startup
    {
        // 跨域策略名称：可以任意取
        private readonly string OriginName = "cors";
        public static ILoggerRepository repository { get; set; }
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            repository = LogManager.CreateRepository("rollingAppender");
            XmlConfigurator.Configure(repository, new FileInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log4net.config")));
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc(options => { options.EnableEndpointRouting = false; });

            // 使用PostgreSql数据库
            services.AddDbContext<H4WebContext>(option => option.UseNpgsql(Configuration.GetConnectionString("PostgreSql")));
            // 使用MySql数据库
            //services.AddDbContext<H4WebContext>(option => option.UseMySql(Configuration.GetConnectionString("MySql")));

            // 解决时间少8小时的问题
            services.AddControllers().AddJsonOptions(opt =>
            {
                opt.JsonSerializerOptions.Encoder = JavaScriptEncoder.Create(UnicodeRanges.All);
                opt.JsonSerializerOptions.Converters.Add(new JsonDateTimeConvert());

            }).SetCompatibilityVersion(CompatibilityVersion.Version_3_0);

            // 允许任意网址跨域
            //services.AddCors(options =>
            //{
            //    options.AddPolicy(OriginName,
            //        builder =>
            //        {
            //            builder.AllowAnyMethod().AllowAnyOrigin().AllowAnyHeader();
            //        });
            //});

            // 允许指定网址的跨域请求
            services.AddCors(options =>
            {
                options.AddPolicy(OriginName,
                                  builder =>
                                  {
                                      builder.WithOrigins("http://localhost:8000",
                                                          "http://localhost:8090");
                                  });
            });

            // 配置session
            services.AddSession(options =>
            {
                options.Cookie.Name = ".AdventureWorks.Session";
                options.IdleTimeout = TimeSpan.FromSeconds(60 * 120);//设置session的过期时间
                options.Cookie.HttpOnly = true;//设置在浏览器不能通过js获得该cookie的值
            });
            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            // HttpContextAccessor 默认实现了它简化了访问HttpContext
            services.AddHttpContextAccessor();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            // 支持使用cshtml页面
            services.AddRazorPages();

            // 注册SignalR
            services.AddSignalR();

            // 注册ISchedulerFactory的实例
            services.AddSingleton<ISchedulerFactory, StdSchedulerFactory>();
            //JobSchedulerWork.Work();
        }

        public void Configure(IApplicationBuilder app)
        {
            //使用session
            app.UseSession();

            app.UseRouting();

            // 允许指定网址的跨域请求(signalr必须要指定)
            app.UseCors(builder =>
            {
                builder.WithOrigins("http://localhost:8000")
                    .AllowAnyHeader()
                    .WithMethods("GET", "POST")
                    .AllowCredentials();
            });

            // CORS 中间件必须配置为在对 UseRouting 和 UseEndpoints的调用之间执行。 配置不正确将导致中间件停止正常运行。
            app.UseCors(OriginName);

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                // SignalR 添加到 ASP.NET Core 依赖关系注入系统和中间件管道
                endpoints.MapHub<ChatHub>("/chathub");
            });

            app.UseDefaultFiles();
            app.UseStaticFiles();

            app.UseMvc();

            //DefaultFilesOptions defaultFilesOptions = new DefaultFilesOptions();
            //defaultFilesOptions.DefaultFileNames.Clear();
            //defaultFilesOptions.DefaultFileNames.Add("index.html");
            //app.UseDefaultFiles(defaultFilesOptions);

            //StaticFileOptions staticFileOptions = new StaticFileOptions();
            //staticFileOptions.FileProvider = new PhysicalFileProvider(@"D:\webapi\NFCInspectServer\wwwroot");
            //app.UseStaticFiles(staticFileOptions);//开启静态文件
        }

    }

    /// <summary>
    /// 将前端传过来的时间转换为本地时间（解决时间少8小时的问题）
    /// </summary>
    public class JsonDateTimeConvert : JsonConverter<DateTime>
    {
        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var paramString = reader.GetString();

            var localDateTime = Convert.ToDateTime(paramString);

            return localDateTime;
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString("yyyy-MM-dd HH:mm:ss"));
        }
    }
}
