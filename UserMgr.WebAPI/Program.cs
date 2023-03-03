using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserMgr.Infrastructure;
using Zack.ASPNETCore;
using Zack.Commons;

namespace UserMgr.WebAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddMemoryCache();
            builder.Services.AddScoped<IMemoryCacheHelper, MemoryCacheHelper>();

            #region register dbContexts

            builder.Services.AddDbContext<UserDbContext>(opt =>
            {
                string connStr = builder.Configuration.GetConnectionString("Demo3");
                opt.UseSqlServer(connStr);
            });

            #endregion

            #region register services from referenced assemblies

            // 獲取Project所引用的所有程序集
            var assemblies = ReflectionHelper.GetAllReferencedAssemblies();
            // 執行所有程序集中的服務註冊
            builder.Services.RunModuleInitializers(assemblies);

            #endregion

            #region register filters

            builder.Services.Configure<MvcOptions>(opt => {
                opt.Filters.Add<UnitOfWorkFilter>();
            });

            #endregion

            #region register mediatr handler

            builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(Assembly.GetExecutingAssembly()));

            #endregion

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}