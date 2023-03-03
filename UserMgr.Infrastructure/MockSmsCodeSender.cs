using UserMgr.Domain;
using UserMgr.Domain.ValueObjects;

namespace UserMgr.Infrastructure;

public class MockSmsCodeSender: ISmsCodeSender
{
    public Task SendCodeAsync(PhoneNumber phoneNumber, string code)
    {
        Console.WriteLine($"向 {phoneNumber.RegionNumber}-{phoneNumber.Number} 發送驗證碼");
        return Task.CompletedTask;
    }
}