using UserMgr.Domain.ValueObjects;

namespace UserMgr.WebAPI;

public record LoginByPhoneAndPasswordRequest(PhoneNumber PhoneNumber, string Password);