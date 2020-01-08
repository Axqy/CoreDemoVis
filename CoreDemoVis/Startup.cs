﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Autofac;
using Microsoft.Extensions.DependencyModel;
using Microsoft.AspNetCore.Builder.Internal;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using log4net.Repository;
using log4net;
using log4net.Config;
using System.IO;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;

namespace CoreDemoVis
{
    public class Startup
    {

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

            //log4net
            //repository = LogManager.CreateRepository("NETCoreRepository");
            //XmlConfigurator.Configure(repository, new FileInfo("log4net.config"));
        }


        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;

            });
            //加入全局异常类
            services.AddMvc(options =>
            {
                options.Filters.Add<Models.HttpGlobalExceptionFilter>(); //加入全局异常类
            });

            //认证Cookie
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options =>
            {
                options.LoginPath = new PathString("/Login");
                options.AccessDeniedPath = new PathString("/Home");
                options.Cookie.Name = "applicationCookie";
            });

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            return AutofacConfigure.Register(services);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                //app.UseStatusCodePages(); 
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");//全局错误处理界面
                app.UseHsts();
            }

            app.ApplicationServices.GetService(typeof(ILoggerFactory));


            app.UseAuthentication();//开启权限认证

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }

    /// <summary>
    /// 注入
    /// </summary>
    public class AutofacConfigure
    {
        public static AutofacServiceProvider Register(IServiceCollection services)
        {
            ContainerBuilder builder = new Autofac.ContainerBuilder();

            //注入视图
            RegisterViewOfService(services);

            //注入log4
            Log4Register();

            builder.RegisterAssemblyTypes(Assembly.Load("CfoBusiness")).AsImplementedInterfaces().InstancePerLifetimeScope();
            builder.Populate(services);

            builder.RegisterAssemblyTypes(typeof(Program).Assembly).AsImplementedInterfaces();
            var container = builder.Build();
            return new AutofacServiceProvider(container);
        }

        /// <summary>
        /// 注入视图 Autofac/Index
        /// </summary>
        /// <param name="services"></param>
        private static void RegisterViewOfService(IServiceCollection services)
        {
            services.AddTransient<CfoBusiness.AutofacService>();
        }


        public static ILoggerRepository log4Repository { get; set; }

        /// <summary>
        /// log4日志配置项
        /// </summary>
        private static void Log4Register()
        {
            //log4net
            log4Repository = LogManager.CreateRepository("NETCoreRepository");
            XmlConfigurator.Configure(log4Repository, new FileInfo("log4net.config"));
        }
    }
}