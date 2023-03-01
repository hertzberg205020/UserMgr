namespace UserMgr.Domain;

public enum UserAccessResult
{
    Ok, PhoneNumberNotFound, LockOut, NoPassword, PasswordError
}