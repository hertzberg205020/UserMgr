namespace UserMgr.Domain.Entities;

public record UserAccessFail
{
    public Guid Id { get; init; }
    public User User { get; init; }
    public Guid UserId { get; init; }
    private bool _isLockedOut;
    public DateTime? LockOutEnd { get; private set; }
    public int AccessFailedCount { get; private set; }

    private UserAccessFail() { }

    public UserAccessFail(User user)
    {
        Id = Guid.NewGuid();
        User = user;
    }


    #region 狀態變更

    /// <summary>
    /// 重置登入失敗鎖定訊息
    /// </summary>
    public void Reset()
    {
        _isLockedOut = false;
        LockOutEnd = null;
        AccessFailedCount = 0;
    }

    /// <summary>
    /// 登入失敗時，累計登入失敗次數
    /// </summary>
    public void Fail()
    {
        AccessFailedCount++;
        if (AccessFailedCount >= 3)
        {
            _isLockedOut =true;
            LockOutEnd = DateTime.Now.AddMinutes(5);
        }
    }

    #endregion

    /// <summary>
    /// 用戶是否為鎖定狀態
    /// </summary>
    /// <returns></returns>
    public bool IsLockedOut()
    {
        if (!_isLockedOut)
        {
            return false;
        }

        if (LockOutEnd >= DateTime.Now)
        {
            return true;
        }

        // 已經超過鎖定時間
        AccessFailedCount = 0;
        LockOutEnd = null;
        return false;
    }
}