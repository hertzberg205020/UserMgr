using UserMgr.Domain.Entities;
using UserMgr.Domain.Events;
using UserMgr.Domain.ValueObjects;

namespace UserMgr.Domain;

public interface IUserRepository
{
    public Task<User?> FindOneAsync(PhoneNumber phoneNumber);
    public Task<User?> FindByOneAsync(Guid userIGuid);
    public Task AddNewLoginHistoryAsync(PhoneNumber phoneNumber, string msg);
    public Task SavePhoneCodeAsync(PhoneNumber phoneNumber, string code);
    public Task<string?> RetrievePhoneCodeAsync(PhoneNumber phoneNumber);
    public Task PublishEventAsync(UserAccessResultEvent @event);
}