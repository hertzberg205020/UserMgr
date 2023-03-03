using UserMgr.Domain.Entities;
using UserMgr.Domain.Events;
using UserMgr.Domain.ValueObjects;

namespace UserMgr.Domain;

/// <summary>
/// 領域層服務
/// 主要的業務邏輯寫在這裡
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

    /// <summary>
    /// 使用者登入檢驗
    /// </summary>
    /// <param name="phoneNumber"></param>
    /// <param name="password"></param>
    /// <returns></returns>
    public async Task<UserAccessResult> CheckLoginAsync(PhoneNumber phoneNumber, string password)
    {
        var user = await _userRepository.FindOneAsync(phoneNumber);
        UserAccessResult result;
        // 找不到使用者
        if (user == null)
        {
            result = UserAccessResult.PhoneNumberNotFound;
        }
        // 使用者被鎖定
        else if (IsLockOut(user))
        {
            result = UserAccessResult.LockOut;
        }
        // 未設置密碼
        else if (!user.HasPassword())
        {
            result = UserAccessResult.NoPassword;
        }
        // 密碼正確
        else if (user.CheckPassword(password))
        {
            result = UserAccessResult.Ok;
            this.ResetAccessFail(user);
        }
        // 密碼錯誤
        else
        {
            result = UserAccessResult.PasswordError;
            this.AccessFail(user);
        }

        UserAccessResultEvent eventItem = new(phoneNumber, result);
        await _userRepository.PublishEventAsync(eventItem);
        return result;
    }

    /// <summary>
    /// 驗證碼檢測
    /// </summary>
    /// <param name="phoneNumber"></param>
    /// <param name="code"></param>
    /// <returns></returns>
    public async Task<CheckCodeResult> CheckPhoneCodeAsync(PhoneNumber phoneNumber, string code)
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

    /// <summary>
    /// 寄送手機登入驗證碼
    /// </summary>
    /// <param name="phoneNumber"></param>
    /// <returns></returns>
    public async Task<UserAccessResult> SendCodeAsync(PhoneNumber phoneNumber)
    {
        var user = await _userRepository.FindOneAsync(phoneNumber);
        if (user == null)
        {
            return UserAccessResult.PhoneNumberNotFound;
        }

        if (IsLockOut(user))
        {
            return UserAccessResult.LockOut;
        }

        string code = Random.Shared.Next(1000, 9999).ToString();
        await _userRepository.SavePhoneCodeAsync(phoneNumber, code);
        await _smsCodeSender.SendCodeAsync(phoneNumber, code);
        return UserAccessResult.Ok;
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