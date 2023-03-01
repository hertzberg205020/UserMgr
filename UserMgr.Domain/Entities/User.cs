using UserMgr.Domain.ValueObjects;
using Zack.Commons;

namespace UserMgr.Domain.Entities;

public record User: IAggregateRoot
{
    public Guid Id { get; init; }
    public PhoneNumber PhoneNumber { get; private set; }
    /// <summary>
    /// 密碼的雜湊值
    /// </summary>
    private string? _passwordHash;
    public UserAccessFail AccessFail { get; private set; }

    private User()
    {
        
    }

    public User(PhoneNumber phoneNumber)
    {
        Id = Guid.NewGuid();
        PhoneNumber = phoneNumber;
        AccessFail = new UserAccessFail(this);
    }

    /// <summary>
    /// 是否設置密碼
    /// </summary>
    /// <returns></returns>
    public bool HasPassword()
    {
        return !string.IsNullOrEmpty(_passwordHash);
    }

    public void ChangePassword(string newPassword)
    {
        if (newPassword.Length <= 3)
        {
            throw new ArgumentException("密碼長度不能低於3位");
        }

        _passwordHash = HashHelper.ComputeMd5Hash(newPassword);
    }

    /// <summary>
    /// 檢查密碼是否正確
    /// </summary>
    /// <param name="password"></param>
    /// <returns></returns>
    public bool CheckPassword(string password)
    {
        return HashHelper.ComputeMd5Hash(password) == _passwordHash;
    }

    /// <summary>
    /// 變更手機號碼
    /// </summary>
    /// <param name="phoneNumber"></param>
    public void ChangePhoneNumber(PhoneNumber phoneNumber)
    {
        PhoneNumber = phoneNumber;
    }
}