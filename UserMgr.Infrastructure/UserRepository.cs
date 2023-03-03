using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using UserMgr.Domain;
using UserMgr.Domain.Entities;
using UserMgr.Domain.Events;
using UserMgr.Domain.ValueObjects;
using Zack.ASPNETCore;
using Zack.Infrastructure.EFCore;

namespace UserMgr.Infrastructure;

public class UserRepository: IUserRepository
{
    private readonly UserDbContext _db;
    private readonly IMemoryCache _memoryCache;
    private readonly IMemoryCacheHelper _cacheHelper;
    private readonly IMediator _mediator;

    public UserRepository(UserDbContext db, IMemoryCacheHelper cacheHelper, IMemoryCache memoryCache, IMediator mediator)
    {
        _db = db;
        _cacheHelper = cacheHelper;
        _memoryCache = memoryCache;
        _mediator = mediator;
    }

    public Task<User?> FindOneAsync(PhoneNumber phoneNumber)
    {
        return _db.Set<User>()
            .Include(u => u.AccessFail)
            .SingleOrDefaultAsync(ExpressionHelper.MakeEqual((User u) => 
            u.PhoneNumber, phoneNumber));
    }

    public async Task<User?> FindOneAsync(Guid userId)
    {
        return await _db.Set<User>()
            .Include(u => u.AccessFail)
            .SingleOrDefaultAsync(u => u.Id == userId);
    }

    public async Task AddNewLoginHistoryAsync(PhoneNumber phoneNumber, string msg)
    {
        User? user = await FindOneAsync(phoneNumber);
        Guid? userId = null;

        if (user != null)
        {
            userId = user.Id;
        }
        _db.Set<UserLoginHistory>()
            .Add(
                new UserLoginHistory(userId, phoneNumber, msg)
                );
        // 這裡不負責狀態的保存
        // await _db.SaveChangesAsync();
    }

    public Task SavePhoneCodeAsync(PhoneNumber phoneNumber, string code)
    {
        string key = $"PhoneNumberCode_{phoneNumber.RegionNumber}_{phoneNumber.Number}";

        var expiredTime = 
        _memoryCache.Set(key, code, new MemoryCacheEntryOptions()
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
        });
        return Task.CompletedTask;
    }

    public Task<string?> RetrievePhoneCodeAsync(PhoneNumber phoneNumber)
    {
        string key = $"PhoneNumberCode_{phoneNumber.RegionNumber}_{phoneNumber.Number}";
        _memoryCache.TryGetValue(key, out string? code);
        _memoryCache.Remove(key);
        return Task.FromResult(code);
    }

    public Task PublishEventAsync(UserAccessResultEvent @event)
    {
        return _mediator.Publish(@event);
    }
}