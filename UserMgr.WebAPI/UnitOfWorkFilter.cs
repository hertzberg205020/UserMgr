using System.Reflection;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;

namespace UserMgr.WebAPI;

public class UnitOfWorkFilter: IAsyncActionFilter
{
    private static UnitOfWorkAttribute? GetUowAttr(ActionDescriptor actionDescriptor)
    {
        var ctrlDescriptor = actionDescriptor as ControllerActionDescriptor;
        if (ctrlDescriptor == null)
        {
            return null;
        }

        var uowAttr = ctrlDescriptor.
                      ControllerTypeInfo.
                      GetCustomAttribute<UnitOfWorkAttribute>();

        if (uowAttr != null)
        {
            return uowAttr;
        }

        return ctrlDescriptor.
               MethodInfo.
               GetCustomAttribute<UnitOfWorkAttribute>();
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var uowAttr = GetUowAttr(context.ActionDescriptor);

        if (uowAttr == null)
        {
            await next();
            return;
        }
        List<DbContext> dbContexts = new List<DbContext>();
        foreach (var dbCtxType in uowAttr.DbContextTypes)
        {
            var provider = context.HttpContext.RequestServices;
            DbContext dbCtx = (DbContext)provider.GetRequiredService(dbCtxType);
            dbContexts.Add(dbCtx);
        }
        var result = await next();
        if (result.Exception == null)
        {
            foreach (var dbContext in dbContexts)
            {
                await dbContext.SaveChangesAsync();
            }
        }
    }
}