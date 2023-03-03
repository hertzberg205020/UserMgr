using Microsoft.Extensions.DependencyInjection;
using Zack.Commons;

namespace UserMgr.Domain;

public class ModuleInitializer: IModuleInitializer
{
    public void Initialize(IServiceCollection services)
    {
        services.AddScoped<UserDomainService>();
    }
}