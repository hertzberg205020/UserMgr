using Microsoft.Extensions.DependencyInjection;
using UserMgr.Domain;
using Zack.Commons;

namespace UserMgr.Infrastructure;

public class ModuleInitializer: IModuleInitializer
{
    public void Initialize(IServiceCollection services)
    {
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<ISmsCodeSender, MockSmsCodeSender>();
    }
}