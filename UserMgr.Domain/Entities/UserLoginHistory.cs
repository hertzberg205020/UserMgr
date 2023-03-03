using UserMgr.Domain.ValueObjects;

namespace UserMgr.Domain.Entities;

public record UserLoginHistory
{
    public long Id { get; init; }
    /// <summary>
    /// UserId是一個指向User實體類別外鍵
    /// </summary>
    public Guid? UserId { get; init; }
    public PhoneNumber PhoneNumber { get; init; }
    public DateTime CreateDateTime { get; init; }
    public string Message { get; init; }
    private UserLoginHistory(){}

    public UserLoginHistory(Guid? userId, PhoneNumber phoneNumber, string message)
    {
        UserId = userId;
        PhoneNumber = phoneNumber;
        CreateDateTime = DateTime.Now;
        Message = message;
    }
}