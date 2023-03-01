using UserMgr.Domain.Entities;
using UserMgr.Domain.Events;
using UserMgr.Domain.ValueObjects;

namespace UserMgr.Domain;

/// <summary>
/// 領域層服務
/// </summary>
public class UserDomainService
{
    private IUserRepository _userRepository;
    private ISmsCodeSender _smsCodeSender;

    public UserDomainService(IUserRepository userRepository, ISmsCodeSender smsCodeSender)
    {
        _userRepository = userRepository;
        _smsCodeSender = smsCodeSender;
    }

    public async Task<UserAccessResult> CheckPassword(PhoneNumber phoneNumber, string password)
    {
        var user = await _userRepository.FindOneAsync(phoneNumber);
        UserAccessResult result;
        if (user == null)
        {
            result = UserAccessResult.PhoneNumberNotFound;
        }
        else if (IsLockOut(user))
        {
            result = UserAccessResult.LockOut;
        }
        else if (!user.HasPassword())
        {
            result = UserAccessResult.NoPassword;
        }
        else if (user.CheckPassword(password))
        {
            result = UserAccessResult.Ok;
            this.ResetAccessFail(user);
        }
        else
        {
            result = UserAccessResult.PasswordError;
            this.AccessFail(user);
        }

        UserAccessResultEvent eventItem = new(phoneNumber, result);
        await _userRepository.PublishEventAsync(eventItem);
        return result;
    }

    public async Task<CheckCodeResult> CheckCodeAsync(PhoneNumber phoneNumber, string code)
    {
        var user = await _userRepository.FindOneAsync(phoneNumber);
        if (user == null)
        {
            return CheckCodeResult.PhoneNumberNotFound;
        }

        if (IsLockOut(user))
        {
            return CheckCodeResult.LockOut;
        }

        string? codeInServer = await _userRepository.RetrievePhoneCodeAsync(phoneNumber);
        if (string.IsNullOrEmpty(codeInServer))
        {
            return CheckCodeResult.CodeError;
        }
        else if (code == codeInServer)
        {
            return CheckCodeResult.Ok;
        }
        else
        {
            AccessFail(user);
            return CheckCodeResult.CodeError;
        }
    }

    public void ResetAccessFail(User user)
    {
        user.AccessFail.Reset();
    }

    public bool IsLockOut(User user)
    {
        return user.AccessFail.IsLockedOut();
    }

    public void AccessFail(User user)
    {
        user.AccessFail.Fail();
    }
}